using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MoviesAPI.Extensions;

public static class MockDatabaseExtension
{
	public static void InitializeMockDatabase(this IServiceCollection services)
	{
		using var serviceScope = services.BuildServiceProvider().CreateScope();
		var dbContext = serviceScope.ServiceProvider.GetService<CinemaDbContext>();

		dbContext!.Database.EnsureCreated();

		dbContext.Auditoriums.Add(new()
		{
			Showtimes =
						[
								new()
								{
										StartDate = new DateTime(2026, 1, 1),
										EndDate = new DateTime(2026, 4, 1),
										Movie = new()
										{
												Title = "Inception",
												ImdbId = "tt1375666",
												ReleaseDate = new DateTime(2010, 01, 14),
												Stars = "Leonardo DiCaprio, Joseph Gordon-Levitt, Ellen Page, Ken Watanabe"
										},
										Schedule = ["16:00", "17:00", "18:00", "18:30", "19:00", "22:00"]
								}
						],
			Seats = 56
		});

		dbContext.Auditoriums.Add(new()
		{
			Seats = 78
		});

		dbContext.Auditoriums.Add(new()
		{
			Seats = 48
		});

		dbContext.SaveChanges();
	}
}
