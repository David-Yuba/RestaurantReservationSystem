using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ReservationSys_LaMaisonRestaurant.Data;
using ReservationSys_LaMaisonRestaurant.Models;
using ReservationSys_LaMaisonRestaurant.Pages;
using Xunit;

namespace ReservationSys_LaMaisonRestaurant.test;

public class IndexModelTests
{
    #region Helper methods
    private ReservationSys_LaMaisonRestaurantContext CreateDatabaseContext()
    {
        var connectionString = Environment.GetEnvironmentVariable("ReservationSys_LaMaisonRestaurantContext");
        if (connectionString.IsNullOrEmpty())
        {
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=ReservationSys_LaMaisonRestaurantContext-850da8c0-aea8-406c-aa63-e086e40a9c01;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        var options = new DbContextOptionsBuilder<ReservationSys_LaMaisonRestaurantContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ReservationSys_LaMaisonRestaurantContext(options);
    }
    public static TempDataDictionary CreateTempData()
    {
        var context = new DefaultHttpContext();
        return new TempDataDictionary(context, new TestTempDataProvider());
    }
    #endregion

    #region Trivial tests
    [Fact]
    public async Task OnPostAsync_InvalidModelState_ReturnsPage()
    {
        // Arrange
        var context = CreateDatabaseContext();
        var model = new IndexModel(context);
        model.TempData = CreateTempData();
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
        model.TempData = CreateTempData();

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
        model.TempData = CreateTempData();

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
        model.TempData = CreateTempData();

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
        model.TempData = CreateTempData();


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
    #endregion

    [Fact]
    public async Task OnPostAsync_FailedReservation_TimeslotFull_ReturnsPage()
    {
        // Arrange
        using var context = CreateDatabaseContext();
        var model = new IndexModel(context);
        model.TempData = CreateTempData();

        DateTime testingDate = new DateTime(2026, 3, 26);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            FullName = "Duplicate Timeslots",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(testingDate),
            TimeSlot = new TimeOnly(18, 00),
            PartySize = 6,
            IsPrivateDining = true
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<PageResult>(result);
    }

    #region IsPrivateDining tests
    [Fact]
    public async Task OnPostAsync_FailedReservation_SameTimeslotIsPrivateDining_ReturnsPage()
    {
        // Arrange
        using var context = CreateDatabaseContext();
        var model = new IndexModel(context);
        model.TempData = CreateTempData();

        DateTime testingDate = new DateTime(2026, 3, 28);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            FullName = "Duplicate Timeslots",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(testingDate),
            TimeSlot = new TimeOnly(19, 30),
            PartySize = 6,
            IsPrivateDining = true
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_FailedReservation_SmallPartySizeIsPrivateDining_ReturnsPage()
    {
        // Arrange
        using var context = CreateDatabaseContext();
        var model = new IndexModel(context);
        model.TempData = CreateTempData();

        DateTime testingDate = new DateTime(2026, 3, 25);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            FullName = "Small Party",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(testingDate),
            TimeSlot = new TimeOnly(18, 30),
            PartySize = 2,
            IsPrivateDining = true
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_FailedReservation_LargePartySizeIsPrivateDining_ReturnsPage()
    {
        // Arrange
        using var context = CreateDatabaseContext();
        var model = new IndexModel(context);
        model.TempData = CreateTempData();

        DateTime testingDate = new DateTime(2026, 3, 27);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            FullName = "Large Party",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(testingDate),
            TimeSlot = new TimeOnly(18, 30),
            PartySize = 16,
            IsPrivateDining = true
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<PageResult>(result);

        // Verify the reservation was not saved in the database
        var savedReservation = await context.Reservation.FirstOrDefaultAsync(r => r.Id == model.Reservation.Id);
        Assert.Null(savedReservation);
    }

    [Fact]
    public async Task OnPostAsync_ValidReservation_MaxPartySizeIsPrivateDining_RedirectsToSuccessPage()
    {
        // Arrange
        using var context = CreateDatabaseContext();

        var model = new IndexModel(context);
        model.TempData = CreateTempData();

        DateTime testingDate = new DateTime(2026, 4, 3);

        model.Reservation = new Reservation
        {
            Status = "Pending",
            ReferenceCode = "defaultValue",
            FullName = "Large Party",
            Email = "alice.johnson@example.com",
            PhoneNumber = "555-123-4567",
            SpecialRequest = "Window seat, please.",
            Date = DateOnly.FromDateTime(testingDate),
            TimeSlot = new TimeOnly(19, 30),
            PartySize = 12,
            IsPrivateDining = true
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
    #endregion
}
public class TestTempDataProvider : ITempDataProvider
{
    private Dictionary<string, object> _data = new();

    public IDictionary<string, object> LoadTempData(HttpContext context)
        => _data;

    public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        => _data = new Dictionary<string, object>(values);
}