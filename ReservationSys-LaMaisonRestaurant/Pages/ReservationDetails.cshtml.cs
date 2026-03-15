using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Data;
using ReservationSys_LaMaisonRestaurant.Models;

namespace ReservationSys_LaMaisonRestaurant.Pages;

public class ReservationDetailsModel : PageModel
{
    private readonly ReservationSys_LaMaisonRestaurantContext _context;
    public readonly RestaurantInfo RestaurantInfo;

    public ReservationDetailsModel(ReservationSys_LaMaisonRestaurantContext context)
    {
        _context = context;
        RestaurantInfo = (from r in _context.RestaurantInfo
                          where r.Name == "LaMaison"
                          select r).First();
    }

    [BindProperty]
    public Reservation Reservation { get; set; } = default!;
    [BindProperty]
    public int Id { get; set; }
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
    public async Task<IActionResult> OnPostAsync()
    {
        // Check if the Status value is supported
        if (Reservation.Status != "Pending" && Reservation.Status != "Confirmed" && Reservation.Status != "Cancelled" && Reservation.Status != "Completed")
        {
            return Page();
        }
        var reservation = _context.Reservation.FirstOrDefault(x => x.Id == Id);
        if (reservation is not null){
            reservation.Status = Reservation.Status;
            _context.SaveChanges();
        }

        return RedirectToPage("ReservationDetails", new { id = Id });
    }
}
