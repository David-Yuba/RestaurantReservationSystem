using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservationSys_LaMaisonRestaurant.Data;
using ReservationSys_LaMaisonRestaurant.Models;
using ReservationSys_LaMaisonRestaurant.Pages;
using Xunit;

namespace ReservationSys_LaMaisonRestaurant.test;

public class IndexModelTests
{
    private ReservationSys_LaMaisonRestaurantContext CreateDatabaseContext()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        string connectionString;

        if (env == "Production")
        {
            connectionString = "Server=db,1433;Database=ReservationSys_LaMaisonRestaurantContext-850da8c0-aea8-406c-aa63-e086e40a9c01;User Id=sa;Password=ASDkjkfsjd@.DKfj23dk;TrustServerCertificate=True";
        }
        else
        {
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=ReservationSys_LaMaisonRestaurantContext-Dev;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        var options = new DbContextOptionsBuilder<ReservationSys_LaMaisonRestaurantContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ReservationSys_LaMaisonRestaurantContext(options);
    }

    [Fact]
    public async Task OnPostAsync_InvalidModelState_ReturnsPage()
    {
        // Arrange
        var context = CreateDatabaseContext();
        var model = new IndexModel(context);
        model.ModelState.AddModelError("Reservation.Name", "Name is required");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }
    [Fact]
    public async Task OnPostAsync_InvalidStatus_ReturnsPage()
    {
        // Arrange
        var context = CreateDatabaseContext();
        var model = new IndexModel(context);

        model.Reservation = new Reservation
        {
            Status = "Confirmed",
            ReferenceCode = "defaultValue",
            FullName = "Alice Johnson",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            TimeSlot = new TimeOnly(18, 0),
            PartySize = 2,
            IsPrivateDining = false
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_DateInPast_ReturnsPage()
    {
        // Arrange
        var context = CreateDatabaseContext();
        var model = new IndexModel(context);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            FullName = "Alice Johnson",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
            TimeSlot = new TimeOnly(18, 0),
            PartySize = 2,
            IsPrivateDining = false
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_DateTooFarInFuture_ReturnsPage()
    {
        // Arrange
        var context = CreateDatabaseContext();
        var model = new IndexModel(context);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            FullName = "Alice Johnson",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(35)),
            TimeSlot = new TimeOnly(18, 0),
            PartySize = 2,
            IsPrivateDining = false
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ValidReservation_RedirectsToSuccessPage()
    {
        // Arrange
        using var context = CreateDatabaseContext();
        var model = new IndexModel(context);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            FullName = "Alice Johnson",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(17)),
            TimeSlot = new TimeOnly(18, 0),
            PartySize = 2,
            IsPrivateDining = false
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("SuccessfulReservation", redirectResult.PageName);

        // Verify the reservation was saved in the database
        var savedReservation = await context.Reservation.FirstOrDefaultAsync(r => r.Id == model.Reservation.Id);
        Assert.NotNull(savedReservation);
        Assert.Equal("Pending", savedReservation.Status);
    }
}