using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservationSys_LaMaisonRestaurant.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Environment.IsDevelopment() ?
    builder.Configuration.GetConnectionString("ReservationSys_LaMaisonRestaurantContext")
        ?? throw new InvalidOperationException("Connection string 'ReservationSys_LaMaisonRestaurantContext' not found.") :
    builder.Configuration.GetConnectionString("DockerConnectionString")
        ?? throw new InvalidOperationException("Connection string 'DockerConnectionString' not found.");

builder.Services.AddDbContext<ReservationSys_LaMaisonRestaurantContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();