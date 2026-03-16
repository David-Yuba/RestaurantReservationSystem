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
-- From the inserted rows calculate the increase in guests per date
	DECLARE @GuestSumOverDates Table (
		Date DATE,
		TimeSlot TIME,
		PartySizeSum INT
	)
	INSERT INTO @GuestSumOverDates 
	SELECT Date, TimeSlot, SUM(PartySize) AS PartySizeSum 
	FROM inserted 
	GROUP BY Date, TimeSlot

-- UPDATE the rows for the dates that already exist in the table by adding the sum of guests per date to the current state.
    UPDATE dbo.RestaurantState
    SET Guests = Guests + i.PartySizeSum
    FROM dbo.RestaurantState rs
    INNER JOIN @GuestSumOverDates i ON rs.Date = i.Date

-- INSERT new rows for non existing date rows
	INSERT INTO dbo.RestaurantState (Date, Guests)
	SELECT i.Date, i.PartySizeSum
	FROM @GuestSumOverDates i
	LEFT JOIN dbo.RestaurantState rs ON rs.Date = i.Date
	WHERE rs.Date IS NULL

-- SELECT from the inserted rows only the rows that are IsPrivateDining=0
	DELETE FROM @GuestSumOverDates
	INSERT INTO @GuestSumOverDates 
	SELECT Date, TimeSlot, SUM(PartySize) AS PartySizeSum 
	FROM inserted AS i
	WHERE i.IsPrivateDining = 0
	GROUP BY Date, TimeSlot
	
	DECLARE @GuestSumJsonOverDate Table (
		Date DATE,
		OccupancySlotsJson nvarchar(max)
	)
	
	DECLARE @CurrentState Table (
		Date DATE,
		TimeSlot TIME,
		PartySizeSum INT
	)

-- Take into account the current state of the field
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

-- UPDATE the values in the JSON field OccupancyPerTimeSlot 
	UPDATE dbo.RestaurantState
	SET OccupancyPerTimeSlot = JsonTable.OccupancySlotsJson
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
