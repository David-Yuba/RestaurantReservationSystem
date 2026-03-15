namespace ReservationSys_LaMaisonRestaurant.Models;

public class RestaurantInfo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public TimeOnly OpeningHours { get; set; }
    public TimeOnly ClosingHours { get; set; }
    
    // Defines the time span between individual reservation slots
    public TimeSpan ReservationSlotIncrements { get; set; }

    // Defines the number of guests the restaurant can accommodate in a given timeslot
    public int TotalGuestsPerSlot { get; set; }

    public ExtraInfo? ExtraInfo { get; set; }
}

// A class that is stored in a json format in the database used to define specificities unique to individual restaurants
public class ExtraInfo
{
    public TimeOnly PrivateDiningStartTime { get; set; }
}
