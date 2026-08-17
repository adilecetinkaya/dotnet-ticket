public interface ITicketService
{
    Task<PagedResult<TicketResponseDto>> GetPagedAsync(
        TicketQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<TicketResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TicketResponseDto> CreateAsync(TicketCreateDto dto, CancellationToken cancellationToken = default);

    Task<TicketResponseDto?> UpdateAsync(
        Guid id,
        TicketUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
