using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Models;

namespace ReservationSys_LaMaisonRestaurant.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext _context;
        public readonly RestaurantInfo RestaurantInfo;
        public AdminModel(ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext context)
        {
            _context = context;
            RestaurantInfo = (from r in _context.RestaurantInfo
                              where r.Name == "LaMaison"
                              select r).First();
        }

        public IList<Reservation> Reservation { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public DateOnly? FilterDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortDateBy { get; set; }

        public int GuestNumber { get; set; }
        public async Task OnGetAsync()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            TimeOnly timeNow = TimeOnly.FromDateTime(DateTime.Now);

            var reservations = from r in _context.Reservation select r;
            if (FilterDate is not null)
            {
                reservations = from r in reservations
                               where r.Date == FilterDate
                               select r;
            }
            if (FilterStatus is not null)
            {
                reservations = from r in reservations
                               where r.Status == FilterStatus
                               select r;
            }
            if (SortDateBy == "Ascending") {
                reservations = from r in reservations
                               orderby r.Date ascending, r.TimeSlot ascending
                               select r;
            }
            else if (SortDateBy == "Descending")
            {
                reservations = from r in reservations
                               orderby r.Date descending, r.TimeSlot descending
                               select r;
            }
            else 
            {
                reservations = from r in reservations
                           orderby (r.Date > today || (r.Date == today && r.TimeSlot >= timeNow)) ? 0 : 1, r.Date ascending , r.TimeSlot ascending
                           select r;
            }

            Reservation = await reservations.ToListAsync();

            if(FilterDate is null)
            {
                var sum = from r in _context.RestaurantState
                                         where r.Date == today
                                         select r.Guests;

                GuestNumber = sum.FirstOrDefault();
            }
            else
            {
                var sum = from r in _context.RestaurantState
                          where r.Date == FilterDate
                          select r.Guests;

                GuestNumber = sum.FirstOrDefault();
            }
        }

        /// <summary>
        /// Query the database for maximum capacity in the givent DateTime
        /// </summary>
        /// <param name="date">The Date to query over</param>
        /// <param name="time">The Time to query over</param>
        /// <returns>True or false</returns>
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
}
