using Microsoft.EntityFrameworkCore;
using ReservationSys_LaMaisonRestaurant.Data;

namespace ReservationSys_LaMaisonRestaurant.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ReservationSys_LaMaisonRestaurantContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<ReservationSys_LaMaisonRestaurantContext>>()))
        {
            if (context == null || context.Reservation == null)
            {
                throw new ArgumentNullException("Null ReservationSys_LaMaisonRestaurantContext");
            }

            if (context.Reservation.Any())
            {
                return;
            }

            context.Reservation.AddRange(
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "ABC123",
                    FullName = "Alice Johnson",
                    Email = "alice.johnson@example.com",
                    PhoneNumber = "555-123-4567",
                    SpecialRequest = "Window seat, please.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                    TimeSlot = new TimeOnly(18, 0),
                    PartySize = 2,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Confirmed",
                    ReferenceCode = "DEF456",
                    FullName = "Bob Smith",
                    Email = "bob.smith@example.com",
                    PhoneNumber = "555-234-5678",
                    SpecialRequest = "High chair for toddler.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                    TimeSlot = new TimeOnly(19, 30),
                    PartySize = 4,
                    IsPrivateDining = true
                },
                new Reservation
                {
                    Status = "Cancelled",
                    ReferenceCode = "GHI789",
                    FullName = "Carol Lee",
                    Email = "carol.lee@example.com",
                    PhoneNumber = "555-345-6789",
                    SpecialRequest = "Allergic to peanuts.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    TimeSlot = new TimeOnly(20, 0),
                    PartySize = 3,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "JKL012",
                    FullName = "David Kim",
                    Email = "david.kim@example.com",
                    PhoneNumber = "555-456-7890",
                    SpecialRequest = "Celebrating birthday.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                    TimeSlot = new TimeOnly(17, 0),
                    PartySize = 6,
                    IsPrivateDining = true
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "MNO345",
                    FullName = "Emma Brown",
                    Email = "emma.brown@example.com",
                    PhoneNumber = "555-567-8901",
                    SpecialRequest = "Quiet corner table.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                    TimeSlot = new TimeOnly(18, 30),
                    PartySize = 5,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Confirmed",
                    ReferenceCode = "PQR678",
                    FullName = "Frank Green",
                    Email = "frank.green@example.com",
                    PhoneNumber = "555-678-9012",
                    SpecialRequest = "Vegan menu options.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                    TimeSlot = new TimeOnly(20, 0),
                    PartySize = 2,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "STU901",
                    FullName = "Grace Miller",
                    Email = "grace.miller@example.com",
                    PhoneNumber = "555-789-0123",
                    SpecialRequest = "Wheelchair accessible table.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(12)),
                    TimeSlot = new TimeOnly(19, 0),
                    PartySize = 8,
                    IsPrivateDining = true
                },
                new Reservation
                {
                    Status = "Cancelled",
                    ReferenceCode = "VWX234",
                    FullName = "Henry Wilson",
                    Email = "henry.wilson@example.com",
                    PhoneNumber = "555-890-1234",
                    SpecialRequest = "No special requests.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                    TimeSlot = new TimeOnly(17, 30),
                    PartySize = 3,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Confirmed",
                    ReferenceCode = "YZA567",
                    FullName = "Ivy Martinez",
                    Email = "ivy.martinez@example.com",
                    PhoneNumber = "555-901-2345",
                    SpecialRequest = "Anniversary celebration.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(8)),
                    TimeSlot = new TimeOnly(21, 0),
                    PartySize = 6,
                    IsPrivateDining = true
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "BCD890",
                    FullName = "Jack Turner",
                    Email = "jack.turner@example.com",
                    PhoneNumber = "555-012-3456",
                    SpecialRequest = "Gluten-free options.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(6)),
                    TimeSlot = new TimeOnly(18, 30),
                    PartySize = 4,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "LMN234",
                    FullName = "Sophie Carter",
                    Email = "sophie.carter@example.com",
                    PhoneNumber = "555-234-1111",
                    SpecialRequest = "Near the entrance.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                    TimeSlot = new TimeOnly(18, 0),
                    PartySize = 2,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "OPQ567",
                    FullName = "Liam Walker",
                    Email = "liam.walker@example.com",
                    PhoneNumber = "555-345-2222",
                    SpecialRequest = "Table with a view.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                    TimeSlot = new TimeOnly(18, 0),
                    PartySize = 2,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "RST890",
                    FullName = "Mia Evans",
                    Email = "mia.evans@example.com",
                    PhoneNumber = "555-456-3333",
                    SpecialRequest = "No special requests.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                    TimeSlot = new TimeOnly(18, 0),
                    PartySize = 2,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "UVW123",
                    FullName = "Noah Scott",
                    Email = "noah.scott@example.com",
                    PhoneNumber = "555-567-4444",
                    SpecialRequest = "Prefer booth seating.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                    TimeSlot = new TimeOnly(18, 0),
                    PartySize = 2,
                    IsPrivateDining = false
                },
                new Reservation
                {
                    Status = "Pending",
                    ReferenceCode = "XYZ456",
                    FullName = "Olivia King",
                    Email = "olivia.king@example.com",
                    PhoneNumber = "555-678-5555",
                    SpecialRequest = "Close to the bar.",
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                    TimeSlot = new TimeOnly(18, 0),
                    PartySize = 5,
                    IsPrivateDining = false
                }
            );
            context.SaveChanges();
        }
    }
}
