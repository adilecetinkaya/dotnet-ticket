public class TicketService : ITicketService
{
    private readonly ITicketRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TicketService> _logger;

    public TicketService(
        ITicketRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<TicketService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<TicketResponseDto>> GetPagedAsync(
        TicketQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var page = await _repository.GetPagedAsync(parameters, cancellationToken);

        return new PagedResult<TicketResponseDto>(
            page.Items.Select(ToResponseDto).ToList(),
            page.Page,
            page.PageSize,
            page.TotalCount);
    }

    public async Task<TicketResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(id, cancellationToken);

        return ticket is null ? null : ToResponseDto(ticket);
    }

    public async Task<TicketResponseDto> CreateAsync(
        TicketCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Subject = dto.Subject.Trim(),
            Description = dto.Description.Trim(),
            Priority = dto.Priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _repository.Add(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Talep olusturuldu: {TicketId}", ticket.Id);

        return ToResponseDto(ticket);
    }

    public async Task<TicketResponseDto?> UpdateAsync(
        Guid id,
        TicketUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(id, cancellationToken);

        if (ticket is null)
        {
            return null;
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException("Kapanmis talep guncellenemez.");
        }

        var oncekiDurum = ticket.Status;

        ticket.Subject = dto.Subject.Trim();
        ticket.Description = dto.Description.Trim();
        ticket.Status = dto.Status;
        ticket.Priority = dto.Priority;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (oncekiDurum != ticket.Status)
        {
            _logger.LogInformation(
                "Talep durumu degisti: {TicketId} {Onceki} -> {Yeni}",
                ticket.Id, oncekiDurum, ticket.Status);
        }

        return ToResponseDto(ticket);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await _repository.GetByIdAsync(id, cancellationToken);

        if (ticket is null)
        {
            return false;
        }

        _repository.Remove(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Talep silindi: {TicketId}", id);

        return true;
    }

    private static TicketResponseDto ToResponseDto(Ticket ticket) =>
        new(
            Id: ticket.Id,
            Subject: ticket.Subject,
            Description: ticket.Description,
            CreatedAt: ticket.CreatedAt,
            Status: ticket.Status,
            Priority: ticket.Priority);
}
