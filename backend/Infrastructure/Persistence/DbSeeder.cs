namespace Infrastructure.Persistence;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        // Prevent duplicate seed records
        if (await dbContext.Registrations.AnyAsync())
            return;

        var types = new[]
        {
            RegistrationType.Exhibitor, RegistrationType.Exhibitor,
            RegistrationType.Exhibitor, RegistrationType.Exhibitor,
            RegistrationType.Exhibitor, RegistrationType.Exhibitor,

            RegistrationType.Competitor, RegistrationType.Competitor,
            RegistrationType.Competitor, RegistrationType.Competitor,
            RegistrationType.Competitor, RegistrationType.Competitor,
            RegistrationType.Competitor, RegistrationType.Competitor,
            RegistrationType.Competitor, RegistrationType.Competitor,
            RegistrationType.Competitor,

            RegistrationType.VIP, RegistrationType.VIP,
            RegistrationType.VIP, RegistrationType.VIP,
            RegistrationType.VIP,

            RegistrationType.Exhibitor,
            RegistrationType.Competitor,
            RegistrationType.VIP
        };

        var statuses = new[]
        {
            RegistrationStatus.Confirmed, RegistrationStatus.Confirmed,
            RegistrationStatus.Confirmed, RegistrationStatus.Confirmed,
            RegistrationStatus.Confirmed, RegistrationStatus.Confirmed,
            RegistrationStatus.Confirmed, RegistrationStatus.Confirmed,
            RegistrationStatus.Confirmed, RegistrationStatus.Confirmed,

            RegistrationStatus.CheckedIn, RegistrationStatus.CheckedIn,
            RegistrationStatus.CheckedIn, RegistrationStatus.CheckedIn,
            RegistrationStatus.CheckedIn, RegistrationStatus.CheckedIn,

            RegistrationStatus.Pending, RegistrationStatus.Pending,
            RegistrationStatus.Pending, RegistrationStatus.Pending,
            RegistrationStatus.Pending, RegistrationStatus.Pending,

            RegistrationStatus.Cancelled, RegistrationStatus.Cancelled,
            RegistrationStatus.Cancelled
        };

        var payments = new decimal[]
        {
            1500, 1500, 1500, 1500, 0, 0,
            2500, 2500, 2500, 2500, 2500,
            2000, 2000, 0, 0, 0, 0,
            5000, 5000, 5000, 2000, 2500,
            0, 0, 0
        };

        var registrations = new List<Registration>();

        for (int i = 0; i < 25; i++)
        {
            var fee = types[i] switch
            {
                RegistrationType.Exhibitor => 1500m,
                RegistrationType.Competitor => 2500m,
                RegistrationType.VIP => 5000m,
                _ => 0m
            };

            registrations.Add(new Registration
            {
                Id = Guid.NewGuid(),
                RegistrationNumber = $"CS27-{i + 1:D4}",
                RegistrationDate = DateTime.UtcNow.AddDays(-i),
                ParticipantName = $"Participant {i + 1}",
                ContactNumber = $"09{170000000 + i:D9}",
                Email = $"participant{i + 1}@example.com",
                Address = $"Street {i + 1}, Sorsogon City",
                CarMakeModel = i % 2 == 0 ? "Toyota Supra MK4" : "Honda Civic",
                PlateNumber = $"CS27-{i + 1:D4}",
                RegistrationType = types[i],
                Status = statuses[i],
                RegistrationFee = fee,
                AmountPaid = payments[i]
            });
        }

        await dbContext.Registrations.AddRangeAsync(registrations);
        await dbContext.SaveChangesAsync();
    }
}