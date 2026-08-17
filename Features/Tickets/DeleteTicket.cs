using MediatR;

public record DeleteTicketCommand(Guid Id) : ICommand<bool>;

public class DeleteTicketHandler : IRequestHandler<DeleteTicketCommand, bool>
{
    private readonly ITicketRepository _repository;
    private readonly ILogger<DeleteTicketHandler> _logger;

    public DeleteTicketHandler(ITicketRepository repository, ILogger<DeleteTicketHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (ticket is null)
        {
            return false;
        }

        _repository.Remove(ticket);

        _logger.LogInformation("Talep silindi: {TicketId}", request.Id);

        return true;
    }
}
