using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Models;

namespace ReservationSys_LaMaisonRestaurant.Pages;

public class SuccessfulReservationModel : PageModel
{
    private readonly ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext _context;
    public readonly RestaurantInfo RestaurantInfo;
    public SuccessfulReservationModel(ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext context)
    {
        _context = context;
        RestaurantInfo = (from r in _context.RestaurantInfo
                          where r.Name == "LaMaison"
                          select r).First();
    }

    public Reservation Reservation { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservation.FirstOrDefaultAsync(m => m.Id == id);
        if (reservation == null)
        {
            return NotFound();
        }
        else
        {
            Reservation = reservation;
        }
        return Page();
    }
}
