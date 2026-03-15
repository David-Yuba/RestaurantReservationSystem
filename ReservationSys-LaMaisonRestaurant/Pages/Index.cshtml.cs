using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Logging;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using ReservationSys_LaMaisonRestaurant.Models;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservationSys_LaMaisonRestaurant.Pages;

public class IndexModel : PageModel
{
    #region Constructor and readonly variables
    private readonly ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext _context;

    public readonly RestaurantInfo RestaurantInfo;
    public IndexModel(ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext context)
    {
        _context = context;
        RestaurantInfo = (from r in _context.RestaurantInfo
                          where r.Name == "LaMaison"
                          select r).First();
    }
    #endregion

    public IActionResult OnGet()
    {
        return Page();
    }

    #region Handler functions and query string bound variables
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
        List<TimeSlotOccupancy> occupancySlots = ReturnOccupancySlots();

        return new JsonResult(occupancySlots);
    }
    public IActionResult OnGetDate()
    {
        List<TimeSlotOccupancy> occupancySlots = ReturnOccupancySlots();
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
    #endregion

    [BindProperty]
    public Reservation Reservation { get; set; } = default!;
    public async Task<IActionResult> OnPostAsync()
    {
        // Validate the conditions set by the attributes set in the model, and the default values of non user set variables
        if (!ModelState.IsValid || Reservation.Status != "Pending" || Reservation.ReferenceCode != "defaultValue")
        {
            return Page();
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        // Validate that the date isn't in the past and that it is at max 30 days in the future counting today
        // The condition above is expressed in the if using De Morgan's law to avoid negation in the statement
        if (Reservation.Date < today || Reservation.Date > today.AddDays(29))
        {
            return Page();
        }
        // Validate that the timeslot is a multiple of 00:30 and in the [12:00,21:30> range
        if (!isValidTimeSlot(Reservation.TimeSlot))
        {
            return Page();
        }

        // Generate the ReferenceCode. The generation happens here to avoid code duplicaion, even if it might be useless
        int uniqueCode = Guid.NewGuid().ToString().GetHashCode();
        Reservation.ReferenceCode = $"LM-{uniqueCode.ToString("X")[0..5]}";

        // Branch into validation specific to private dining
        if (Reservation.IsPrivateDining)
        {
            var privateDiningReservations = from r in _context.Reservation
                                            where r.Date == Reservation.Date
                                            select r.TimeSlot;

            // Validate that the private dining time slot is not taken and 
            // that the private dining time slot is in the required time frame of [18:00,~ >, the upper bound is validated by a previous condition
            if (privateDiningReservations.ToList().Contains(Reservation.TimeSlot) && Reservation.TimeSlot >= RestaurantInfo.ExtraInfo?.PrivateDiningStartTime)
                return Page();
            // Validate that the party size is in the [6, 12] range
            // The condition above is expressed in the if using De Morgan's law to avoid negation in the statement
            if (Reservation.PartySize < 6 || Reservation.PartySize > 12)
                return Page();

            _context.Reservation.Add(Reservation);
            await _context.SaveChangesAsync();

            // Set a temporary flag making the Redirect page only accessible on the initial redirect
            TempData["ReservationSuccess"] = true;
            return RedirectToPage("SuccessfulReservation", new { id = Reservation.Id });
        }

        int occupancySum = ReturnOccupancySum();
        // Confirm that with the added party size, the amount of guests wont excede the maximum capacity
        if(occupancySum + Reservation.PartySize > RestaurantInfo.TotalGuestsPerSlot) {
            return Page();
        }

        _context.Reservation.Add(Reservation);
        await _context.SaveChangesAsync();

        // Set a temporary flag making the Redirect page only accessible on the initial redirect
        TempData["ReservationSuccess"] = true;
        return RedirectToPage("SuccessfulReservation", new { id = Reservation.Id });
    }

    #region Helper functions
    private int CalulateMaximumPartySize()
    {
        if (Date is null) return 10;

        int maximumTimeSlotOccupancy = RestaurantInfo.TotalGuestsPerSlot;

        var reservations = from r in _context.Reservation
                           where r.IsPrivateDining == false
                           select r;

        reservations = reservations.Where(res => res.TimeSlot == TimeSlot);

        int maximumPartySize = maximumTimeSlotOccupancy - reservations.ToList().Aggregate(0, (sum, res) => sum + res.PartySize);

        if (maximumPartySize > 10) maximumPartySize = 10;
        return maximumPartySize;
    }
    private List<TimeSlotOccupancy> ReturnOccupancySlots()
    {
        var res = from r in _context.RestaurantState.AsNoTracking()
                  where r.Date == Date
                  select r.OccupancyPerTimeSlot;

        return res.FirstOrDefault() ?? new List<TimeSlotOccupancy>();
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
        List<TimeOnly> validTimeslots = new List<TimeOnly>();
        if (!Reservation.IsPrivateDining)
        {
            TimeOnly closingHours = RestaurantInfo.ClosingHours.Add(-RestaurantInfo.ReservationSlotIncrements);

            for (TimeOnly i=RestaurantInfo.OpeningHours ; i<closingHours ; i=i.Add(RestaurantInfo.ReservationSlotIncrements))
                validTimeslots.Add(i);
        }
        else
        {
            TimeOnly closingHours = RestaurantInfo.ClosingHours.Add(-RestaurantInfo.ReservationSlotIncrements);
            TimeOnly startTime = RestaurantInfo.ExtraInfo is not null ? RestaurantInfo.ExtraInfo.PrivateDiningStartTime : RestaurantInfo.OpeningHours;

            for (TimeOnly i=startTime ; i<closingHours ; i=i.Add(RestaurantInfo.ReservationSlotIncrements))
                validTimeslots.Add(i);
        }

        bool isValid = validTimeslots.Contains(timeSlot);

        return isValid;
    }
    #endregion
}
