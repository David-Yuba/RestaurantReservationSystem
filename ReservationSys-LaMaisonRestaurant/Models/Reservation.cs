using System.ComponentModel.DataAnnotations;
namespace ReservationSys_LaMaisonRestaurant.Models;

public class Reservation
{
    public int Id { get; set; }

    [Display(Name = "Reference Code")]
    public string ReferenceCode { get; set; } = string.Empty;

    [Display(Name = "Full Name")]
    [Required]
    public string FullName { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateOnly Date { get; set; }

    [Display(Name = "Time Slot")]
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public TimeOnly TimeSlot { get; set; }

    [Display(Name = "Party Size")]
    [Range(0,12)]
    public int PartySize { get; set; }

    [Display(Name = "Special Request")]
    [StringLength(500)]
    public string? SpecialRequest { get; set; }

    public string Status { get; set; } = "Pending";

    [Display(Name = "Private Dining")]
    public bool IsPrivateDining { get; set; } = false;
}
