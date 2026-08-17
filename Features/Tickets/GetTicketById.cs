public record GetTicketByIdQuery(Guid Id);

public static class GetTicketByIdHandler
{
    public static async Task<TicketResponseDto?> Handle(
        GetTicketByIdQuery query,
        ITicketRepository repository,
        CancellationToken cancellationToken)
    {
        var ticket = await repository.GetByIdAsync(query.Id, cancellationToken);

        return ticket?.ToResponseDto();
    }
}
