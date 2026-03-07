using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Models;

namespace ReservationSys_LaMaisonRestaurant.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext _context;

        public AdminModel(ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext context)
        {
            _context = context;
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
                               orderby r.Date ascending
                               select r;
            }
            else if (SortDateBy == "Descending")
            {
                reservations = from r in reservations
                               orderby r.Date descending
                               select r;
            }
            else 
            {
                reservations = (from r in reservations
                           orderby r.Date > today ? 0 : 1, r.Date ascending
                           select r);
            }

            Reservation = await reservations.ToListAsync();

            if(FilterDate is null)
            {
                var sum = from r in _context.Reservation
                                         where r.Date == today
                                         select r.PartySize;

                GuestNumber = sum.ToList().Sum();
            }
            else
            {
                var sum = from r in _context.Reservation
                                         where r.Date == FilterDate
                                         select r.PartySize;

                GuestNumber = sum.ToList().Sum();
            }
        }
        public bool IsSlotFull(DateOnly date, TimeOnly time)
        {
            var sumOfPeopleOnSameDateTime = (from r in _context.Reservation
                                             where r.Date == date && r.TimeSlot == time
                                             select r.PartySize).Sum();

            if (sumOfPeopleOnSameDateTime >= 20)
            {
                return true;
            }
            else return false;
        }
    }
}
