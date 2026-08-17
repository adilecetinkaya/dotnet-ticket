using FluentValidation;

public record CreateTicketCommand(
    string Subject,
    string Description,
    TicketPriority Priority);

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

public static class CreateTicketHandler
{
    public static async Task<TicketResponseDto> Handle(
        CreateTicketCommand command,
        ITicketRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateTicketCommand> logger,
        CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Subject = command.Subject.Trim(),
            Description = command.Description.Trim(),
            Priority = command.Priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        repository.Add(ticket);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Talep olusturuldu: {TicketId}", ticket.Id);

        return ticket.ToResponseDto();
    }
}
