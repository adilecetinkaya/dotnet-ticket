using MediatR;

public record GetTicketByIdQuery(Guid Id) : IQuery<TicketResponseDto?>;

public class GetTicketByIdHandler : IRequestHandler<GetTicketByIdQuery, TicketResponseDto?>
{
    private readonly ITicketRepository _repository;

    public GetTicketByIdHandler(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<TicketResponseDto?> Handle(
        GetTicketByIdQuery request,
        CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.Id, cancellationToken);

        return ticket?.ToResponseDto();
    }
}
