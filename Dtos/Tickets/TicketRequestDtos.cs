using System.ComponentModel.DataAnnotations;

public class TicketCreateDto
{
    [Required(ErrorMessage = "Subject zorunludur.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Subject 3-200 karakter olmalidir.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description zorunludur.")]
    [StringLength(4000, MinimumLength = 3)]
    public string Description { get; set; } = string.Empty;

    [EnumDataType(typeof(TicketPriority), ErrorMessage = "Gecersiz oncelik.")]
    public TicketPriority Priority { get; set; } = TicketPriority.Normal;
}

public class TicketUpdateDto
{
    [Required(ErrorMessage = "Subject zorunludur.")]
    [StringLength(200, MinimumLength = 3)]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description zorunludur.")]
    [StringLength(4000, MinimumLength = 3)]
    public string Description { get; set; } = string.Empty;

    [EnumDataType(typeof(TicketStatus), ErrorMessage = "Gecersiz durum.")]
    public TicketStatus Status { get; set; }

    [EnumDataType(typeof(TicketPriority), ErrorMessage = "Gecersiz oncelik.")]
    public TicketPriority Priority { get; set; }
}
