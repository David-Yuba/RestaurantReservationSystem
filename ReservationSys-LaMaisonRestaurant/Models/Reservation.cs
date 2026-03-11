using System.ComponentModel.DataAnnotations;
namespace ReservationSys_LaMaisonRestaurant.Models;

public class Reservation
{
    public int Id { get; set; }

    public string ReferenceCode { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Please enter a valid email address")]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateOnly Date { get; set; }
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public TimeOnly TimeSlot { get; set; }

    [Range(0,12)]
    public int PartySize { get; set; }

    [StringLength(500)]
    public string? SpecialRequest { get; set; }

    public string Status { get; set; } = "Pending";
    public bool IsPrivateDining { get; set; } = false;
}
