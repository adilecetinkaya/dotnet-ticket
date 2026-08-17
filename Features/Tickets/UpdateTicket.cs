using FluentValidation;

public record UpdateTicketCommand(
    Guid Id,
    string Subject,
    string Description,
    TicketStatus Status,
    TicketPriority Priority);

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

public static class UpdateTicketHandler
{
    public static async Task<TicketResponseDto?> Handle(
        UpdateTicketCommand command,
        ITicketRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTicketCommand> logger,
        CancellationToken cancellationToken)
    {
        var ticket = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (ticket is null)
        {
            return null;
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException("Kapanmis talep guncellenemez.");
        }

        var oncekiDurum = ticket.Status;

        ticket.Subject = command.Subject.Trim();
        ticket.Description = command.Description.Trim();
        ticket.Status = command.Status;
        ticket.Priority = command.Priority;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (oncekiDurum != ticket.Status)
        {
            logger.LogInformation(
                "Talep durumu degisti: {TicketId} {Onceki} -> {Yeni}",
                ticket.Id, oncekiDurum, ticket.Status);
        }

        return ticket.ToResponseDto();
    }
}
