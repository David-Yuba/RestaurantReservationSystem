using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Models;

namespace ReservationSys_LaMaisonRestaurant.Data
{
    public class ReservationSys_LaMaisonRestaurantContext : DbContext
    {
        public ReservationSys_LaMaisonRestaurantContext (DbContextOptions<ReservationSys_LaMaisonRestaurantContext> options)
            : base(options)
        {
        }

        public DbSet<ReservationSys_LaMaisonRestaurant.Models.Reservation> Reservation { get; set; } = default!;
    }
}
