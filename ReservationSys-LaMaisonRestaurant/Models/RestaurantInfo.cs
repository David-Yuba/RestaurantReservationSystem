namespace ReservationSys_LaMaisonRestaurant.Models;

public class RestaurantInfo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public TimeOnly OpeningHours { get; set; }
    public TimeOnly ClosingHours { get; set; }
    public TimeSpan ReservationSlotIncrements { get; set; }
    public int TotalGuestsPerSlot { get; set; }
    public ExtraInfo? ExtraInfo { get; set; }
}

public class ExtraInfo
{
    public TimeOnly PrivateDiningStartTime { get; set; }
}
