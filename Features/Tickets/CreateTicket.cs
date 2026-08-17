using FluentValidation;
using MediatR;


public record CreateTicketCommand(
    string Subject,
    string Description,
    TicketPriority Priority) : ICommand<TicketResponseDto>;

public class CreateTicketValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketValidator()
    {
        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject zorunludur.")
            .Length(3, 200).WithMessage("Subject 3-200 karakter olmalidir.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description zorunludur.")
            .MinimumLength(3);

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Gecersiz oncelik.");
    }
}

public class CreateTicketHandler : IRequestHandler<CreateTicketCommand, TicketResponseDto>
{
    private readonly ITicketRepository _repository;

    public CreateTicketHandler(ITicketRepository repository)
    {
        _repository = repository;
    }

    public Task<TicketResponseDto> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Subject = request.Subject.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _repository.Add(ticket);

        return Task.FromResult(ticket.ToResponseDto());
    }
}
