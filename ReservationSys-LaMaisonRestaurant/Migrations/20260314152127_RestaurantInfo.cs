using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSys_LaMaisonRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpeningHours = table.Column<TimeOnly>(type: "time", nullable: false),
                    ClosingHours = table.Column<TimeOnly>(type: "time", nullable: false),
                    ReservationSlotIncrements = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalGuestsPerSlot = table.Column<int>(type: "int", nullable: false),
                    ExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantInfo");
        }
    }
}
