using AutoMapper;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.DTOs.IMDB;
using MoviesAPI.DTOs.Requests;
using MoviesAPI.Handlers;
using MoviesAPI.Requests;
using MoviesAPI.WebClients;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPIUnitTests.Handlers;

[TestClass]
public class ShowtimeServiceUnitTests
{
	private IMapper _mapper;
	private CinemaDbContext _dbContext;
	private Mock<IIMDBWebApiClient> _mockWebApiClient;

	[TestInitialize]
	public void Initialize()
	{
		_mockWebApiClient = new();

		var options = new DbContextOptionsBuilder<CinemaDbContext>()
						.UseInMemoryDatabase(databaseName: "TestDb")
						.Options;
		_dbContext = new CinemaDbContext(options);
		_dbContext.Auditoriums.Add(new()
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

		_dbContext.Auditoriums.Add(new()
		{
			Seats = 78
		});

		_dbContext.Auditoriums.Add(new()
		{
			Seats = 48
		});
		_dbContext.SaveChanges();

		_mapper = new Mapper(new MapperConfiguration(c => c.AddMaps(AppDomain.CurrentDomain.GetAssemblies())));
	}

	[TestMethod]
	public async Task GetMovieInfoAsync_ShouldReturnMovieInfo_WhenImdbIdExists()
	{
		var movieId = "movA1";
		_mockWebApiClient
			.Setup(x => x.GetMovieInfoAsync(It.Is<string>(id => id == movieId)))
			.ReturnsAsync(new IMDBMovieInfo("The Lord of the Rings", DateTime.Parse("01/01/2020"), movieId, "Vigo Mortensen, Ian McKellen"));

		var handler = new CreateShowtimeHandler(_mapper, _dbContext, _mockWebApiClient.Object);

		var result = await handler.Handle(new CreateShowtimeRequest(
			new ShowtimeRequest("12/31/2019", "12/31/2021", "08:00,10:00", movieId, 1)),
			CancellationToken.None
		);

		Assert.IsNotNull(result);
		Assert.AreEqual("The Lord of the Rings", result.Movie.Title);
		Assert.AreEqual(movieId, result.Movie.ImdbId);
		Assert.AreEqual("Vigo Mortensen, Ian McKellen", result.Movie.Stars);
		Assert.AreEqual(DateTime.Parse("01/01/2020"), result.Movie.ReleaseDate);

		var showtimeInDb = _dbContext.Showtimes.Include(s => s.Movie).FirstOrDefault(s => s.Id == result.Id);
		Assert.IsNotNull(showtimeInDb);
		Assert.AreEqual("The Lord of the Rings", showtimeInDb.Movie.Title);
		Assert.AreEqual(movieId, showtimeInDb.Movie.ImdbId);
		Assert.AreEqual("Vigo Mortensen, Ian McKellen", showtimeInDb.Movie.Stars);
		Assert.AreEqual(DateTime.Parse("01/01/2020"), showtimeInDb.Movie.ReleaseDate);
	}
}
