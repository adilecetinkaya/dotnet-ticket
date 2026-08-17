public interface ITicketRepository
{
    Task<PagedResult<Ticket>> GetPagedAsync(
        TicketQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(Ticket ticket);

    void Remove(Ticket ticket);
}