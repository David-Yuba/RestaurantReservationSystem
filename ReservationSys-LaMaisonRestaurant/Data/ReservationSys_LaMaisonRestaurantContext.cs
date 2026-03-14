using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace ReservationSys_LaMaisonRestaurant.Data
{
    public class ReservationSys_LaMaisonRestaurantContext : DbContext
    {
        public ReservationSys_LaMaisonRestaurantContext (DbContextOptions<ReservationSys_LaMaisonRestaurantContext> options)
            : base(options)
        {
        }

        public DbSet<Reservation> Reservation { get; set; } = default!;
        public DbSet<RestaurantState> RestaurantState { get; set; } = default!;
        public DbSet<RestaurantInfo> RestaurantInfo { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RestaurantInfo>()
                .OwnsOne(RestaurantState => RestaurantState.ExtraInfo, builder => builder.ToJson());
            modelBuilder.Entity<Reservation>()
                .ToTable(tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<RestaurantState>()
                .OwnsMany(RestaurantState => RestaurantState.OccupancyPerTimeSlot, builder => builder.ToJson());
        }
    }
}
