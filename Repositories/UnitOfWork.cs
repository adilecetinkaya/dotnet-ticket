public class UnitOfWork : IUnitOfWork
{
    private readonly TicketingDbContext _context;

    public UnitOfWork(TicketingDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
