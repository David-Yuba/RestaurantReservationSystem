using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Data;
using ReservationSys_LaMaisonRestaurant.Models;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace ReservationSys_LaMaisonRestaurant.ViewComponents;

public class ReservationListViewComponent : ViewComponent
{
    private readonly ReservationSys_LaMaisonRestaurantContext _context;
    public readonly RestaurantInfo RestaurantInfo;

    public ReservationListViewComponent(ReservationSys_LaMaisonRestaurantContext context)
    {
        _context = context;
        RestaurantInfo = (from r in _context.RestaurantInfo
                          where r.Name == "LaMaison"
                          select r).First();
    }
    public async Task<IViewComponentResult> InvokeAsync(DateOnly? filterDate, string? filterStatus, string? sortDateBy)
    {
        var reservations = await GetItemsAsync(filterDate, filterStatus, sortDateBy);
        return View(reservations);
    }
    private Task<List<Reservation>> GetItemsAsync(DateOnly? filterDate, string? filterStatus, string? sortDateBy)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        TimeOnly timeNow = TimeOnly.FromDateTime(DateTime.Now);

        var reservations = from r in _context.Reservation select r;
        if (filterDate is not null)
        {
            reservations = from r in reservations
                           where r.Date == filterDate
                           select r;
        }
        if (filterStatus is not null)
        {
            reservations = from r in reservations
                           where r.Status == filterStatus
                           select r;
        }
        if (sortDateBy == "Ascending")
        {
            reservations = from r in reservations
                           orderby r.Date ascending, r.TimeSlot ascending
                           select r;
        }
        else if (sortDateBy == "Descending")
        {
            reservations = from r in reservations
                           orderby r.Date descending, r.TimeSlot descending
                           select r;
        }
        else
        {
            reservations = (from r in reservations
                            orderby (r.Date >= today && r.TimeSlot >= timeNow) ? 0 : 1, r.Date ascending, r.TimeSlot ascending
                            select r);
        }

        return reservations.ToListAsync();
    }
    public bool IsSlotFull(DateOnly date, TimeOnly time)
    {
        var sumOfPeopleOnSameDateTime = (from r in _context.Reservation
                                         where r.Date == date && r.TimeSlot == time && r.IsPrivateDining == false
                                         select r.PartySize).Sum();

        if (sumOfPeopleOnSameDateTime >= RestaurantInfo.TotalGuestsPerSlot)
        {
            return true;
        }
        else return false;
    }
}
