public static class TicketMappings
{
    public static TicketResponseDto ToResponseDto(this Ticket ticket) =>
        new(
            Id: ticket.Id,
            Subject: ticket.Subject,
            Description: ticket.Description,
            CreatedAt: ticket.CreatedAt,
            Status: ticket.Status,
            Priority: ticket.Priority);
}
