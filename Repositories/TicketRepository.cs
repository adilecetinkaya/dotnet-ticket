
using Microsoft.EntityFrameworkCore;

public class TicketRepository: ITicketRepository
{
    private readonly TicketingDbContext dbContext;

    public TicketRepository(TicketingDbContext dbContext)
    {
        this.dbContext = dbContext;    
    }

    public async Task<PagedResult<Ticket>> GetPagedAsync(TicketQueryParameters parameters, CancellationToken cancellationToken)
    {

        IQueryable<Ticket> query = dbContext.Tickets.AsNoTracking();

        query = ApplyFilters(query, parameters);

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, parameters.Sort);


        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Ticket>(items, parameters.Page, parameters.PageSize, totalCount);
    }
    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Tickets
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void Add(Ticket ticket) => dbContext.Tickets.Add(ticket);

    public void Remove(Ticket ticket) => dbContext.Tickets.Remove(ticket);

    private static IQueryable<Ticket> ApplyFilters(IQueryable<Ticket> query, TicketQueryParameters p)
    {
        if (p.Status.HasValue)
        {
            query = query.Where(d => d.Status == p.Status.Value);
        }

        if (p.Priority.HasValue)
        {
            query = query.Where(d => d.Priority == p.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            var search = $"%{p.Search.Trim()}%";
            query = query.Where(d => EF.Functions.ILike(d.Subject, search));
        }

        return query;
    }


    private static IQueryable<Ticket> ApplySorting(IQueryable<Ticket> query, string? sort) => sort switch
    {
        "createdAt_asc" => query.OrderBy(d => d.CreatedAt),
        _ => query.OrderByDescending(d => d.CreatedAt).ThenBy(d => d.Id)
    };
}