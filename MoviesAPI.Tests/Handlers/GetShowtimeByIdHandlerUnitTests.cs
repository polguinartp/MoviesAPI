using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Handlers;
using MoviesAPI.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPIUnitTests.Handlers;

[TestClass]
public class GetShowtimeByIdHandlerUnitTests
{
	private IMapper _mapper;
	private CinemaDbContext dbContext;
	private int _existingShowtimeId;

	[TestInitialize]
	public void Initialize()
	{
		var options = new DbContextOptionsBuilder<CinemaDbContext>()
						.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
						.Options;
		dbContext = new CinemaDbContext(options);
		
		var auditorium = new Auditorium
		{
			Showtimes =
						[
								new()
								{
										StartDate = new DateTime(2026, 3, 15),
										EndDate = new DateTime(2026, 6, 30),
										Movie = new()
										{
												Title = "The Dark Knight",
												ImdbId = "tt0468569",
												ReleaseDate = new DateTime(2008, 07, 18),
												Stars = "Christian Bale, Heath Ledger, Aaron Eckhart, Michael Caine"
										},
										Schedule = ["14:00", "17:30", "20:00", "21:30", "23:00"]
								},
								new()
								{
										StartDate = new DateTime(2026, 2, 10),
										EndDate = new DateTime(2026, 5, 20),
										Movie = new()
										{
												Title = "Interstellar",
												ImdbId = "tt0816692",
												ReleaseDate = new DateTime(2014, 11, 07),
												Stars = "Matthew McConaughey, Anne Hathaway, Jessica Chastain, Michael Caine"
										},
										Schedule = ["13:00", "16:30", "19:45", "22:30"]
								}
						],
			Seats = 56
		};
		
		dbContext.Auditoriums.Add(auditorium);
		dbContext.SaveChanges();
		
		_existingShowtimeId = auditorium.Showtimes[0].Id;

		_mapper = new Mapper(new MapperConfiguration(c => c.AddMaps(AppDomain.CurrentDomain.GetAssemblies())));
	}

	[TestMethod]
	public async Task GetShowtimeById_ShouldReturnShowtime_WhenShowtimeExists()
	{
		var handler = new GetShowtimeByIdHandler(_mapper, dbContext);

		var result = await handler.Handle(new GetShowtimeByIdRequest(_existingShowtimeId), CancellationToken.None);

		Assert.IsNotNull(result);
		Assert.AreEqual(_existingShowtimeId, result.Id);
		Assert.AreEqual("The Dark Knight", result.Movie.Title);
		Assert.AreEqual("tt0468569", result.Movie.ImdbId);
		Assert.AreEqual(new DateTime(2008, 07, 18), result.Movie.ReleaseDate);
		Assert.AreEqual("Christian Bale, Heath Ledger, Aaron Eckhart, Michael Caine", result.Movie.Stars);
		Assert.AreEqual(new DateTime(2026, 3, 15).ToString(), result.StartDate);
		Assert.AreEqual(new DateTime(2026, 6, 30).ToString(), result.EndDate);
	}

	[TestMethod]
	public async Task GetShowtimeById_ShouldReturnNull_WhenShowtimeNotFound()
	{
		var handler = new GetShowtimeByIdHandler(_mapper, dbContext);
		var nonExistentId = 99999;

		var result = await handler.Handle(new GetShowtimeByIdRequest(nonExistentId), CancellationToken.None);

		Assert.IsNull(result);
	}
}
