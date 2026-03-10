using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using ReservationSys_LaMaisonRestaurant.Models;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservationSys_LaMaisonRestaurant.Pages;

public class IndexModel : PageModel
{
    private readonly ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext _context;
    public IndexModel(ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }
    [BindProperty(SupportsGet = true)]
    public int? Size { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateOnly? Date { get; set; }
    [BindProperty(SupportsGet = true)]
    public TimeOnly? TimeSlot { get; set; }
    [BindProperty(SupportsGet = true)]
    public bool PrivateDining { get; set; }
    public IActionResult OnGetSize()
    {
        List<OccupancySlot> occupancySlots = ReturnOccupancySlots();

        return new JsonResult(occupancySlots);
    }
    public IActionResult OnGetDate()
    {
        List<OccupancySlot> occupancySlots = ReturnOccupancySlots();
        return new JsonResult(occupancySlots);
    }
    public IActionResult OnGetTime()
    {
        int maximumPartySize = CalulateMaximumPartySize();
        return new JsonResult(maximumPartySize);
    }
    public IActionResult OnGetPrivateDining()
    {
        if (Date is null) return new JsonResult(new int[0]);
        List<TimeOnly> privateDiningReservations = getAllPrivateDiningReservations();
        return new JsonResult(privateDiningReservations);
    }

    
    [BindProperty]
    public Reservation Reservation { get; set; } = default!;
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Reservation.Status != "Pending" || Reservation.ReferenceCode != "defaultValue")
        {
            return Page();
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        if (Reservation.Date < today || Reservation.Date > today.AddDays(29))
        {
            return Page();
        }
        if (!isValidTimeSlot(Reservation.TimeSlot))
        {
            return Page();
        }

        int uniqueCode = Guid.NewGuid().ToString().GetHashCode();
        Reservation.ReferenceCode = $"LM-{uniqueCode.ToString("X")[0..5]}";

        if (Reservation.IsPrivateDining)
        {
            var privateDiningReservations = from r in _context.Reservation
                                            where r.Date == Reservation.Date
                                            select r.TimeSlot;

            if (privateDiningReservations.ToList().Contains(Reservation.TimeSlot))
                return Page();
            if (Reservation.PartySize < 6 || Reservation.PartySize > 12)
                return Page();

            await _context.SaveChangesAsync();

            return RedirectToPage("SuccessfulReservation", new { id = Reservation.Id });
        }

        int occupancySum = ReturnOccupancySum();
        if(occupancySum + Reservation.PartySize > 20) {
            return Page();
        }

        _context.Reservation.Add(Reservation);
        await _context.SaveChangesAsync();

        return RedirectToPage("SuccessfulReservation", new { id = Reservation.Id });
    }

    private int CalulateMaximumPartySize()
    {
        if (Date is null) return 10;

        int maximumTimeSlotOccupancy = 20;

        var reservations = from r in _context.Reservation
                           where r.IsPrivateDining == false
                           select r;

        reservations = reservations.Where(res => res.TimeSlot == TimeSlot);

        int maximumPartySize = maximumTimeSlotOccupancy - reservations.ToList().Aggregate(0, (sum, res) => sum + res.PartySize);

        if (maximumPartySize > 10) maximumPartySize = 10;
        return maximumPartySize;
    }
    private List<OccupancySlot> ReturnOccupancySlots()
    {
        var reservations = from r in _context.Reservation
                           where r.IsPrivateDining == false
                           select r;
        reservations = reservations.Where(res => res.Date == Date);
        List<OccupancySlot> occupancy = reservations.ToList()
            .Select(res => new OccupancySlot(res.TimeSlot, res.PartySize))
            .OrderBy(occupancy => occupancy.TimeSlot).ToList();

        var temp = new List<OccupancySlot>();
        for (int i = 0; i < occupancy.Count(); i++)
        {
            if (i == 0) temp.Add(occupancy[i]);
            else if (temp.Last().TimeSlot == occupancy[i].TimeSlot)
            {
                temp.Last().PartySize += occupancy[i].PartySize;
            }
            else
            {
                temp.Add(occupancy[i]);
            }
        }

        return temp;
    }
    private int ReturnOccupancySum()
    {
        var occupancySum = from r in _context.Reservation
                           where (r.Date == Reservation.Date) && (r.TimeSlot == Reservation.TimeSlot) && r.IsPrivateDining == false
                           select r.PartySize;
        int sum = 0;
        foreach (var PartySize in occupancySum)
        {
            sum += PartySize;
        }
        return sum;
    }
    private List<TimeOnly> getAllPrivateDiningReservations()
    {
        var privateDiningReservations = from r in _context.Reservation
                           where (r.Date == Date) && (r.IsPrivateDining == true)
                           select r.TimeSlot;

        return privateDiningReservations.ToList();
    }
    private class OccupancySlot {
        public TimeOnly TimeSlot { get; set; }
        public int PartySize { get; set; }
        public OccupancySlot(TimeOnly t, int s)
        {
            TimeSlot = t;
            PartySize = s;
        }
    };
    private bool isValidTimeSlot(TimeOnly timeSlot)
    {
        List<TimeOnly> validTimeslots;
        if (!Reservation.IsPrivateDining)
            validTimeslots =
                [new TimeOnly(12, 00), new TimeOnly(12, 30), new TimeOnly(13, 00), new TimeOnly(13, 30),
                new TimeOnly(14, 00), new TimeOnly(14, 30), new TimeOnly(15, 00), new TimeOnly(15, 30),
                new TimeOnly(16, 00), new TimeOnly(16, 30), new TimeOnly(17, 00), new TimeOnly(17, 30),
                new TimeOnly(18, 00), new TimeOnly(18, 30), new TimeOnly(19, 00), new TimeOnly(19, 30),
                new TimeOnly(20, 00), new TimeOnly(20, 30), new TimeOnly(21, 00)];
        else validTimeslots =
                [new TimeOnly(16, 00), new TimeOnly(16, 30), new TimeOnly(17, 00), new TimeOnly(17, 30),
                new TimeOnly(18, 00), new TimeOnly(18, 30), new TimeOnly(19, 00), new TimeOnly(19, 30),
                new TimeOnly(20, 00), new TimeOnly(20, 30), new TimeOnly(21, 00)];

        bool isValid = validTimeslots.Contains(timeSlot);

        return isValid;
    }
}
