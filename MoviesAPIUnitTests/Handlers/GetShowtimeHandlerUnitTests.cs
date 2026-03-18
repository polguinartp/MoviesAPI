using AutoMapper;
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
public class GetShowtimeHandlerUnitTests
{
	private IMapper _mapper;
	private CinemaDbContext dbContext;

	[TestInitialize]
	public void Initialize()
	{
		var options = new DbContextOptionsBuilder<CinemaDbContext>()
						.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
						.Options;
		dbContext = new CinemaDbContext(options);
		dbContext.Auditoriums.Add(new()
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

		_mapper = new Mapper(new MapperConfiguration(c => c.AddMaps(AppDomain.CurrentDomain.GetAssemblies())));
	}

	[TestMethod]
	public async Task GetShowtimes_ShouldReturnShowtimes_WhenShowtimesExist()
	{
		var handler = new GetShowtimeHandler(_mapper, dbContext);

		var result = await handler.Handle(new GetShowtimeRequest(null, null), CancellationToken.None);

		Assert.IsNotNull(result);
		Assert.AreEqual(2, result.Count);
		
		Assert.AreEqual(new DateTime(2026, 3, 15).ToString(), result[0].StartDate);
		Assert.AreEqual(new DateTime(2026, 6, 30).ToString(), result[0].EndDate);
		Assert.AreEqual("The Dark Knight", result[0].Movie.Title);
		Assert.AreEqual("tt0468569", result[0].Movie.ImdbId);
		Assert.AreEqual(new DateTime(2008, 07, 18), result[0].Movie.ReleaseDate);
		Assert.AreEqual("Christian Bale, Heath Ledger, Aaron Eckhart, Michael Caine", result[0].Movie.Stars);
		Assert.IsTrue(result[0].Schedule.Contains("14:00"));
		Assert.IsTrue(result[0].Schedule.Contains("17:30"));
		Assert.IsTrue(result[0].Schedule.Contains("20:00"));
		Assert.IsTrue(result[0].Schedule.Contains("21:30"));
		Assert.IsTrue(result[0].Schedule.Contains("23:00"));
		
		Assert.AreEqual(new DateTime(2026, 2, 10).ToString(), result[1].StartDate);
		Assert.AreEqual(new DateTime(2026, 5, 20).ToString(), result[1].EndDate);
		Assert.AreEqual("Interstellar", result[1].Movie.Title);
		Assert.AreEqual("tt0816692", result[1].Movie.ImdbId);
		Assert.AreEqual(new DateTime(2014, 11, 07), result[1].Movie.ReleaseDate);
		Assert.AreEqual("Matthew McConaughey, Anne Hathaway, Jessica Chastain, Michael Caine", result[1].Movie.Stars);
		Assert.IsTrue(result[1].Schedule.Contains("13:00"));
		Assert.IsTrue(result[1].Schedule.Contains("16:30"));
		Assert.IsTrue(result[1].Schedule.Contains("19:45"));
		Assert.IsTrue(result[1].Schedule.Contains("22:30"));
	}

	[TestMethod]
	public async Task GetShowtimes_ShouldReturnEmptyList_WhenNoShowtimesExist()
	{
		var options = new DbContextOptionsBuilder<CinemaDbContext>()
						.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
						.Options;
		var emptyDbContext = new CinemaDbContext(options);
		
		var handler = new GetShowtimeHandler(_mapper, emptyDbContext);

		var result = await handler.Handle(new GetShowtimeRequest(null, null), CancellationToken.None);

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task GetShowtimes_ShouldReturnFilteredShowtimes_WhenFilteringByDate()
	{
		var handler = new GetShowtimeHandler(_mapper, dbContext);
		var filterDate = new DateTime(2026, 6, 20);

		var result = await handler.Handle(new GetShowtimeRequest(filterDate, null), CancellationToken.None);

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
		Assert.AreEqual(new DateTime(2026, 3, 15).ToString(), result[0].StartDate);
		Assert.AreEqual(new DateTime(2026, 6, 30).ToString(), result[0].EndDate);
		Assert.AreEqual("The Dark Knight", result[0].Movie.Title);
		Assert.AreEqual("tt0468569", result[0].Movie.ImdbId);
		Assert.AreEqual(new DateTime(2008, 07, 18), result[0].Movie.ReleaseDate);
		Assert.AreEqual("Christian Bale, Heath Ledger, Aaron Eckhart, Michael Caine", result[0].Movie.Stars);
		Assert.IsTrue(result[0].Schedule.Contains("14:00"));
		Assert.IsTrue(result[0].Schedule.Contains("17:30"));
		Assert.IsTrue(result[0].Schedule.Contains("20:00"));
		Assert.IsTrue(result[0].Schedule.Contains("21:30"));
		Assert.IsTrue(result[0].Schedule.Contains("23:00"));
	}

	[TestMethod]
	public async Task GetShowtimes_ShouldReturnFilteredShowtimes_WhenFilteringByMovieTitle()
	{
		var handler = new GetShowtimeHandler(_mapper, dbContext);

		var result = await handler.Handle(new GetShowtimeRequest(null, "Interstellar"), CancellationToken.None);

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
		Assert.AreEqual(new DateTime(2026, 2, 10).ToString(), result[0].StartDate);
		Assert.AreEqual(new DateTime(2026, 5, 20).ToString(), result[0].EndDate);
		Assert.AreEqual("Interstellar", result[0].Movie.Title);
		Assert.AreEqual("tt0816692", result[0].Movie.ImdbId);
		Assert.AreEqual(new DateTime(2014, 11, 07), result[0].Movie.ReleaseDate);
		Assert.AreEqual("Matthew McConaughey, Anne Hathaway, Jessica Chastain, Michael Caine", result[0].Movie.Stars);
		Assert.IsTrue(result[0].Schedule.Contains("13:00"));
		Assert.IsTrue(result[0].Schedule.Contains("16:30"));
		Assert.IsTrue(result[0].Schedule.Contains("19:45"));
		Assert.IsTrue(result[0].Schedule.Contains("22:30"));
	}

	[TestMethod]
	public async Task GetShowtimes_ShouldReturnFilteredShowtimes_WhenFilteringByDateAndMovieTitle()
	{
		var handler = new GetShowtimeHandler(_mapper, dbContext);
		var filterDate = new DateTime(2026, 4, 15);

		var result = await handler.Handle(new GetShowtimeRequest(filterDate, "Dark Knight"), CancellationToken.None);

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
		Assert.AreEqual(new DateTime(2026, 3, 15).ToString(), result[0].StartDate);
		Assert.AreEqual(new DateTime(2026, 6, 30).ToString(), result[0].EndDate);
		Assert.AreEqual("The Dark Knight", result[0].Movie.Title);
		Assert.AreEqual("tt0468569", result[0].Movie.ImdbId);
		Assert.AreEqual(new DateTime(2008, 07, 18), result[0].Movie.ReleaseDate);
		Assert.AreEqual("Christian Bale, Heath Ledger, Aaron Eckhart, Michael Caine", result[0].Movie.Stars);
		Assert.IsTrue(result[0].Schedule.Contains("14:00"));
		Assert.IsTrue(result[0].Schedule.Contains("17:30"));
		Assert.IsTrue(result[0].Schedule.Contains("20:00"));
		Assert.IsTrue(result[0].Schedule.Contains("21:30"));
		Assert.IsTrue(result[0].Schedule.Contains("23:00"));
	}
}
