using System.Text.Json.Nodes;

namespace ReservationSys_LaMaisonRestaurant.Models;

public class RestaurantState
{
    public int Id { get; set; }

    // The date for which the state is calculated
    public DateOnly Date { get; set; }

    // The sum of all guests on the given date
    public int Guests { get; set; } = 0;

    // A list of the individual timeslot states excluding the private dining reservations
    public List<TimeSlotOccupancy>? OccupancyPerTimeSlot { get; set; }
}

public class TimeSlotOccupancy
{
    public TimeOnly TimeSlot { get; set; }

    // The sum of all guests on the same day in the same timeslot
    public int PartySizeSum { get; set; }
}
