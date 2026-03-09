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
    private ReservationSys_LaMaisonRestaurantContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ReservationSys_LaMaisonRestaurantContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ReservationSys_LaMaisonRestaurantContext(options);
    }

[Fact]
    public async Task OnPostAsync_InvalidModelState_ReturnsPage()
    {
        // Arrange
        var context = CreateInMemoryContext();
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
        var context = CreateInMemoryContext();
        var model = new IndexModel(context);

        model.Reservation = new Reservation
        {
            Status = "Confirmed", // Invalid status
            ReferenceCode = "defaultValue",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            TimeSlot = new TimeOnly(18, 0),
            PartySize = 4,
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
        var context = CreateInMemoryContext();
        var model = new IndexModel(context);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), // Past date
            TimeSlot = new TimeOnly(18, 0),
            PartySize = 4,
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
        var context = CreateInMemoryContext();
        var model = new IndexModel(context);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(35)), // Beyond 29 days
            TimeSlot = new TimeOnly(18, 0),
            PartySize = 4,
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
        using var context = CreateInMemoryContext();
        var model = new IndexModel(context);

        model.Reservation = new Reservation
        {
            Id = 1,
            Status = "Pending",
            ReferenceCode = "defaultValue",
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            TimeSlot = new TimeOnly(18, 0),
            PartySize = 4,
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