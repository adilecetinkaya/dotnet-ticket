public record GetTicketsQuery(TicketQueryParameters Parameters);

public static class GetTicketsHandler
{
    public static async Task<PagedResult<TicketResponseDto>> Handle(
        GetTicketsQuery query,
        ITicketRepository repository,
        CancellationToken cancellationToken)
    {
        var page = await repository.GetPagedAsync(query.Parameters, cancellationToken);

        return new PagedResult<TicketResponseDto>(
            page.Items.Select(t => t.ToResponseDto()).ToList(),
            page.Page,
            page.PageSize,
            page.TotalCount);
    }
}
