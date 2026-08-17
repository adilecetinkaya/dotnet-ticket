using MediatR;

public record GetTicketsQuery(TicketQueryParameters Parameters)
    : IQuery<PagedResult<TicketResponseDto>>;

public class GetTicketsHandler : IRequestHandler<GetTicketsQuery, PagedResult<TicketResponseDto>>
{
    private readonly ITicketRepository _repository;

    public GetTicketsHandler(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TicketResponseDto>> Handle(
        GetTicketsQuery request,
        CancellationToken cancellationToken)
    {
        var page = await _repository.GetPagedAsync(request.Parameters, cancellationToken);

        return new PagedResult<TicketResponseDto>(
            page.Items.Select(t => t.ToResponseDto()).ToList(),
            page.Page,
            page.PageSize,
            page.TotalCount);
    }
}
