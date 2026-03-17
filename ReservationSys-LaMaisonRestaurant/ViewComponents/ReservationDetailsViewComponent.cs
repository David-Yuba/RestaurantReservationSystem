using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Data;
using ReservationSys_LaMaisonRestaurant.Models;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace ReservationSys_LaMaisonRestaurant.ViewComponents;

public class ReservationDetailsViewComponent : ViewComponent
{
    private readonly ReservationSys_LaMaisonRestaurantContext _context;
    public readonly RestaurantInfo RestaurantInfo;

    public ReservationDetailsViewComponent(ReservationSys_LaMaisonRestaurantContext context)
    {
        _context = context;
        RestaurantInfo = (from r in _context.RestaurantInfo
                          where r.Name == "LaMaison"
                          select r).First();
    }
    public async Task<IViewComponentResult> InvokeAsync(int id)
    {
        var reservation = await GetItemsAsync(id);
        return View(reservation);
    }
    private Task<Reservation> GetItemsAsync(int id)
    {

        var reservation = _context.Reservation.FirstOrDefaultAsync(m => m.Id == id);

        return reservation;
    }
}
