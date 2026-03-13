using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSys_LaMaisonRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class AddInsertReservationTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(
                    @"
CREATE TRIGGER InsertReservationTrigger
ON dbo.Reservation
AFTER INSERT
AS
BEGIN
	DECLARE @GuestSumOverDates Table (
		Date DATE,
		TimeSlot TIME,
		PartySizeSum INT
	)
	INSERT INTO @GuestSumOverDates 
	SELECT Date, TimeSlot, SUM(PartySize) AS PartySizeSum 
	FROM inserted 
	GROUP BY Date, TimeSlot

    UPDATE dbo.RestaurantState
    SET Guests = Guests + i.PartySizeSum
    FROM dbo.RestaurantState rs
    INNER JOIN @GuestSumOverDates i ON rs.Date = i.Date

	INSERT INTO dbo.RestaurantState (Date, Guests)
	SELECT i.Date, i.PartySizeSum
	FROM @GuestSumOverDates i
	LEFT JOIN dbo.RestaurantState rs ON rs.Date = i.Date
	WHERE rs.Date IS NULL

	
	DECLARE @GuestSumJsonOverDate Table (
		Date DATE,
		OccupancySlotsJson nvarchar(max)
	)
	
	DECLARE @CurrentState Table (
		Date DATE,
		TimeSlot TIME,
		PartySizeSum INT
	)

	INSERT INTO @CurrentState
	SELECT Date, TimeSlots.TimeSlot, TimeSlots.PartySizeSum
	FROM dbo.RestaurantState rs
	CROSS APPLY OPENJSON(rs.OccupancyPerTimeSlot)
	WITH (
		TimeSlot TIME '$.TimeSlot',
		PartySizeSum INT '$.PartySizeSum'
	) AS TimeSlots

	UPDATE ns
	SET ns.PartySizeSum = ns.PartySizeSum + cs.PartySizeSum
	FROM @GuestSumOverDates ns
	INNER JOIN @CurrentState cs
		ON cs.Date = ns.Date AND cs.TimeSlot = ns.TimeSlot

	INSERT INTO @GuestSumOverDates
	SELECT cs.Date, cs.TimeSlot, cs.PartySizeSum
	FROM @CurrentState cs
	LEFT JOIN @GuestSumOverDates ns
		ON cs.Date = ns.Date AND cs.TimeSlot = ns.TimeSlot
	WHERE ns.TimeSlot IS NULL

	INSERT INTO @GuestSumJsonOverDate
	SELECT 
		Date,
		(
			SELECT TimeSlot, PartySizeSum
			FROM @GuestSumOverDates inner_t
			WHERE inner_t.Date = outer_t.Date
			ORDER BY TimeSlot ASC
			FOR JSON PATH
		)
	FROM @GuestSumOverDates outer_t
	GROUP BY Date

	UPDATE dbo.RestaurantState
	SET OccupancyPerTimeSLot = JsonTable.OccupancySlotsJson
	FROM RestaurantState rs
	INNER JOIN @GuestSumJsonOverDate JsonTable ON rs.Date = JsonTable.Date
END
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
