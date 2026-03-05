using System.ComponentModel.DataAnnotations;
namespace ReservationSys_LaMaisonRestaurant.Models;

public class Reservation
{
    public int Id { get; set; }

    public string ReferenceCode { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public TimeOnly TimeSlot { get; set; }

    public int PartySize { get; set; }

    public string? SpecialRequest { get; set; }
}
