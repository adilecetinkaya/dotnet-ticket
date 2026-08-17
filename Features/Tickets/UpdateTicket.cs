using FluentValidation;
using MediatR;

public record UpdateTicketCommand(
    Guid Id,
    string Subject,
    string Description,
    TicketStatus Status,
    TicketPriority Priority) : ICommand<TicketResponseDto?>;

public class UpdateTicketValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Subject)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Priority).IsInEnum();
    }
}

public class UpdateTicketHandler : IRequestHandler<UpdateTicketCommand, TicketResponseDto?>
{
    private readonly ITicketRepository _repository;
    private readonly ILogger<UpdateTicketHandler> _logger;

    public UpdateTicketHandler(ITicketRepository repository, ILogger<UpdateTicketHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TicketResponseDto?> Handle(
        UpdateTicketCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (ticket is null)
        {
            return null;
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException("Kapanmis talep guncellenemez.");
        }

        var oncekiDurum = ticket.Status;

        ticket.Subject = request.Subject.Trim();
        ticket.Description = request.Description.Trim();
        ticket.Status = request.Status;
        ticket.Priority = request.Priority;


        if (oncekiDurum != ticket.Status)
        {
            _logger.LogInformation(
                "Talep durumu degisti: {TicketId} {Onceki} -> {Yeni}",
                ticket.Id, oncekiDurum, ticket.Status);
        }

        return ticket.ToResponseDto();
    }
}
