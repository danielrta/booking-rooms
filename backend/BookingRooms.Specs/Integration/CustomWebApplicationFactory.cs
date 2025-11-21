using BookingRooms.API.Data;
using BookingRooms.API.Infrastructure;
using BookingRooms.Specs.Integration.Reservations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BookingRooms.Specs.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestConnectionString = "Data Source=booking_integration_tests.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<BookingContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<BookingContext>(options =>
            {
                options.UseSqlite(TestConnectionString);
            });
            
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.TestAuthenticationScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.TestAuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.TestAuthenticationScheme,
                    options => { });
            
            services.RemoveAll<IPublicHolidayService>();
            services.AddSingleton<FakePublicHolidayService>();
            services.AddSingleton<IPublicHolidayService>(sp => sp.GetRequiredService<FakePublicHolidayService>());
        });
    }
}