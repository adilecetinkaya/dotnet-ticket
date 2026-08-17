public record DeleteTicketCommand(Guid Id);

public static class DeleteTicketHandler
{
    public static async Task<bool> Handle(
        DeleteTicketCommand command,
        ITicketRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteTicketCommand> logger,
        CancellationToken cancellationToken)
    {
        var ticket = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (ticket is null)
        {
            return false;
        }

        repository.Remove(ticket);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Talep silindi: {TicketId}", command.Id);

        return true;
    }
}
