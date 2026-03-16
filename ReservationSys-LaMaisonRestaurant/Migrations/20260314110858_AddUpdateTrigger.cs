using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSys_LaMaisonRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                    @"
CREATE TRIGGER UpdateStatusReservationTrigger
ON dbo.Reservation
AFTER UPDATE
AS
BEGIN
-- Terminating if
	IF NOT EXISTS (SELECT 1 FROM inserted WHERE Status='Cancelled')
		RETURN

	DECLARE @ReservationOccupancy TABLE (
		Date DATE,
		TimeSlot TIME,
		PartySize INT
	)
	INSERT INTO @ReservationOccupancy
	SELECT Date, TimeSlot, PartySize
	FROM inserted

-- UPDATES the value of guests on a given day in the table dbo.RestaurantState 
-- by subtracting the number of guests in the cancelled reservation.
	UPDATE rs
	SET rs.Guests = rs.Guests - cas.PartySize
	FROM dbo.RestaurantState AS rs
		INNER JOIN @ReservationOccupancy AS cas
			ON cas.Date = rs.Date 

-- Terminating if protecting the guests per time slot state if
-- the cancelled reservation is private dining
	IF EXISTS (SELECT 1 FROM inserted WHERE IsPrivateDining=1)
		RETURN

-- Build a table from the JSON value
	DECLARE @CurrentState TABLE (
		TimeSlot TIME,
		PartySizeSum INT
	)
	INSERT INTO @CurrentState
	SELECT 
		CAST(JSON.TimeSlot AS TIME),
		CAST(JSON.PartySizeSum AS INT)
	FROM dbo.RestaurantState AS rs
		INNER JOIN @ReservationOccupancy as cas
			ON cas.Date = rs.Date
	CROSS APPLY OPENJSON(rs.OccupancyPerTimeSlot)
		WITH (
			TimeSlot VARCHAR(20) '$.TimeSlot',
			PartySizeSum INT '$.PartySizeSum'
		) AS JSON
	
-- UPDATES the value of guest amount per timeslot by subtracting
-- the number of guests in the cancelled reservation
	UPDATE cs
	SET cs.PartySizeSum = cs.PartySizeSum - cas.PartySize
	FROM @CurrentState AS cs
		INNER JOIN @ReservationOccupancy  AS cas
			ON cas.TimeSlot = cs.TimeSlot 
	
	DECLARE @SelectedDate DATE
	SELECT @SelectedDate = cas.Date
	FROM @ReservationOccupancy AS cas

-- Build a JSON value from the table
	DECLARE @NewJson varchar(MAX)
	SELECT @NewJson = (
		SELECT TimeSlot, PartySizeSum
		FROM @CurrentState
		FOR JSON PATH
	)

-- UPDATES the table with the new JSON value
	UPDATE rs
	SET rs.OccupancyPerTimeSlot = @NewJson
	FROM dbo.RestaurantState AS rs
	WHERE @SelectedDate = rs.Date
END
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
