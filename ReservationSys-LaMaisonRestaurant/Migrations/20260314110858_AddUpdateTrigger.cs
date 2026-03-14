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

	UPDATE rs
	SET rs.Guests = rs.Guests - cas.PartySize
	FROM dbo.RestaurantState AS rs
		INNER JOIN @ReservationOccupancy AS cas
			ON cas.Date = rs.Date 

	IF EXISTS (SELECT 1 FROM inserted WHERE IsPrivateDining=1)
		RETURN

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
	
	UPDATE cs
	SET cs.PartySizeSum = cs.PartySizeSum - cas.PartySize
	FROM @CurrentState AS cs
		INNER JOIN @ReservationOccupancy  AS cas
			ON cas.TimeSlot = cs.TimeSlot 
	
	DECLARE @SelectedDate DATE
	SELECT @SelectedDate = cas.Date
	FROM @ReservationOccupancy AS cas

	DECLARE @NewJson varchar(MAX)
	SELECT @NewJson = (
		SELECT TimeSlot, PartySizeSum
		FROM @CurrentState
		FOR JSON PATH
	)

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
