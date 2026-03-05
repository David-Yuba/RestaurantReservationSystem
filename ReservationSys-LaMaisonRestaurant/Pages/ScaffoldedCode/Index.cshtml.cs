using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Data;
using ReservationSys_LaMaisonRestaurant.Models;

namespace ReservationSys_LaMaisonRestaurant.Pages.ScaffoldedCode
{
    public class IndexModel : PageModel
    {
        private readonly ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext _context;

        public IndexModel(ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext context)
        {
            _context = context;
        }

        public IList<Reservation> Reservation { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Reservation = await _context.Reservation.ToListAsync();
        }
    }
}
