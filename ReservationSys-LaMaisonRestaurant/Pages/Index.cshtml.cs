using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using ReservationSys_LaMaisonRestaurant.Models;

namespace ReservationSys_LaMaisonRestaurant.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext _context;
    public IndexModel(ILogger<IndexModel> logger, ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Reservation Reservation { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Reservation.Status != "Pending" || Reservation.ReferenceCode != "defaultValue")
        {
            return Page();
        }

        int uniqueCode = Guid.NewGuid().ToString().GetHashCode();
        Reservation.ReferenceCode = $"LM-{uniqueCode.ToString("X")[0..5]}";

        _context.Reservation.Add(Reservation);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
