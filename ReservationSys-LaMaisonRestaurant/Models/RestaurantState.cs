using System.Text.Json.Nodes;

namespace ReservationSys_LaMaisonRestaurant.Models;

public class RestaurantState
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int Guests { get; set; } = 0;

    public List<TimeSlotOccupancy>? OccupancyPerTimeSlot { get; set; }
}

public class TimeSlotOccupancy
{
    public TimeOnly TimeSlot { get; set; }

    public int PartySizeSum { get; set; }
}
