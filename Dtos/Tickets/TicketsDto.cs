using System.ComponentModel.DataAnnotations;

public record TicketResponseDto(
    Guid Id,
    string Subject,
    string Description,
    DateTime CreatedAt,
    TicketStatus Status,
    TicketPriority Priority
);


public class TicketQueryParameters
{
    private const int MaxPageSize = 100;

    private int _pageSize = 20;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public TicketStatus? Status { get; set; }

    public TicketPriority? Priority { get; set; }

    public string? Search { get; set; }

    public string? Sort { get; set; }
}
