using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSys_LaMaisonRestaurant.Data;
using ReservationSys_LaMaisonRestaurant.Models;

namespace ReservationSys_LaMaisonRestaurant.Pages.ScaffoldedCode
{
    public class CreateModel : PageModel
    {
        private readonly ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext _context;

        public CreateModel(ReservationSys_LaMaisonRestaurant.Data.ReservationSys_LaMaisonRestaurantContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Reservation.Add(Reservation);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
