using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.DTOs.API;
using MoviesAPI.DTOs.IMDB;
using MoviesAPI.Services;
using MoviesAPI.WebClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoviesAPIUnitTests;

[TestClass]
public class ShowtimeServiceUnitTests
{
    private Mock<IRepository<ShowtimeEntity>> _mockShowtimesRepository;
    private Mock<IRepository<MovieEntity>> _mockMoviesRepository;
    private Mock<IIMDBWebApiClient> _mockWebApiClient;
    private Mock<IMapper> _mockMapper;
    private Mock<IQueueService> _mockQueueService;

    private List<IMDBMovieInfo> _moviesInfos = TestDataProvider.Instance.GetMoviesInfos();
    private List<MovieEntity> _moviesEntities = TestDataProvider.Instance.GetMovies();
    private List<ShowtimeEntity> _showtimesEntities = TestDataProvider.Instance.GetShowtimes();

    [TestInitialize]
    public void Initialize()
    {
        _mockShowtimesRepository = new Mock<IRepository<ShowtimeEntity>>();
        _mockShowtimesRepository.Setup(repository => repository.GetCollectionAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
            It.IsAny<Func<IQueryable<ShowtimeEntity>, IOrderedQueryable<ShowtimeEntity>>>(),
            It.IsAny<string>()))
            .ReturnsAsync(_showtimesEntities);

        _mockShowtimesRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((int id, string includeProperties) =>
        {
            return _showtimesEntities.FirstOrDefault(showtime => showtime.Id == id);
        });
        _mockShowtimesRepository.Setup(x => x.UpdateAsync(It.IsAny<ShowtimeEntity>())).ReturnsAsync((ShowtimeEntity entity) =>
        {
            var index = _showtimesEntities.FindIndex(showtime => showtime.Id == entity.Id);
            _showtimesEntities[index] = entity;

            return entity;
        });

        _mockMoviesRepository = new Mock<IRepository<MovieEntity>>();
        _mockMoviesRepository.Setup(repository => repository.GetCollectionAsync(It.IsAny<Expression<Func<MovieEntity, bool>>>(),
            It.IsAny<Func<IQueryable<MovieEntity>, IOrderedQueryable<MovieEntity>>>(),
            It.IsAny<string>()))
            .ReturnsAsync(_moviesEntities);

        _mockMoviesRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((int id, string includeProperties) =>
        {
            return _moviesEntities.FirstOrDefault(showtime => showtime.Id == id);
        });
        _mockMoviesRepository.Setup(x => x.UpdateAsync(It.IsAny<MovieEntity>())).ReturnsAsync((MovieEntity entity) =>
        {
            var index = _moviesEntities.FindIndex(showtime => showtime.Id == entity.Id);
            _moviesEntities[index] = entity;

            return entity;
        });

        _mockWebApiClient = new Mock<IIMDBWebApiClient>();
        _mockWebApiClient.Setup(webApiClient => webApiClient.GetMovieInfoAsync(It.Is<string>(imbdId => _moviesInfos.Exists(movieInfo => movieInfo.ImdbId == imbdId)))).ReturnsAsync((string imbdId) =>
        {
            return _moviesInfos.First(movieInfo => movieInfo.ImdbId == imbdId);
        });

        _mockMapper = new Mock<IMapper>();
        _mockMapper.Setup(mapper => mapper.Map<MovieEntity>(It.Is<IMDBMovieInfo>(movieInfo => _moviesEntities.Exists(movie => movie.ImdbId == movieInfo.ImdbId)))).Returns((IMDBMovieInfo movie) =>
        {
            return _moviesEntities.First(movieEntity => movieEntity.ImdbId.Equals(movie.ImdbId));
        });

        _mockMapper.Setup(mapper => mapper.Map<ShowtimeEntity>(It.Is<Showtime>(showtime => _showtimesEntities.Exists(showtimeEntity => showtimeEntity.Id == showtime.Id)))).Returns((Showtime showtime) =>
        {
            return _showtimesEntities.First(showtimeEntity => showtimeEntity.Id == showtime.Id);
        });

        _mockMapper.Setup(mapper => mapper.Map(It.IsAny<Showtime>(), It.IsAny<ShowtimeEntity>()))
            .Callback((Showtime showtime, ShowtimeEntity showtimeEntity) =>
            {
                showtimeEntity.Id = showtime.Id;
                showtimeEntity.StartDate = DateTime.Parse(showtime.StartDate);
                showtimeEntity.EndDate = DateTime.Parse(showtime.EndDate);
                showtimeEntity.Schedule = showtime.Schedule.Split(',', StringSplitOptions.None);
                showtimeEntity.AuditoriumId = showtime.AuditoriumId;
            });

        _mockQueueService = new Mock<IQueueService>();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ShouldThrowArgumentNullException_WhenShowtimesRepositoryIsNull()
    {
        // Arrange
        _ = new ShowtimeService(null, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ShouldThrowArgumentNullException_WhenMoviesRepositoryIsNull()
    {
        // Arrange
        _ = new ShowtimeService(_mockShowtimesRepository.Object, null, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ShouldThrowArgumentNullException_WhenWebApiClientIsNull()
    {
        // Arrange
        _ = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, null, _mockMapper.Object, _mockQueueService.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
    {
        // Arrange
        _ = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, null, _mockQueueService.Object);
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnExpectedResult_WhenNoFilters()
    {
        // Arrange
        var subjectUnderTest = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);

        // Act
        var result = await subjectUnderTest.GetAsync();

        // Assert
        CollectionAssert.AreEquivalent(_showtimesEntities, result.ToList());
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnExpectedResult_WhenDateFilter()
    {
        // Arrange
        var subjectUnderTest = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);
        var date = DateTime.Parse("2022-05-04");
        var expectedResult = _showtimesEntities.Where(showtime => date >= showtime.StartDate && date <= showtime.EndDate).ToList();

        _mockShowtimesRepository.Setup(repository => repository.GetCollectionAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
            It.IsAny<Func<IQueryable<ShowtimeEntity>, IOrderedQueryable<ShowtimeEntity>>>(),
            It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        // Act            
        var result = await subjectUnderTest.GetAsync(date);

        // Assert            
        CollectionAssert.AreEquivalent(expectedResult, result.ToList());
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnExpectedResult_WhenMovieTitleFilter()
    {
        // Arrange
        var subjectUnderTest = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);
        var title = "The Best Movie Ever 3";
        var expectedResult = _showtimesEntities.Where(showtime => string.Equals(showtime.Movie.Title, title, StringComparison.OrdinalIgnoreCase)).ToList();

        _mockShowtimesRepository.Setup(repository => repository.GetCollectionAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
            It.IsAny<Func<IQueryable<ShowtimeEntity>, IOrderedQueryable<ShowtimeEntity>>>(),
            It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await subjectUnderTest.GetAsync(movieTitle: title);

        // Assert            
        CollectionAssert.AreEquivalent(expectedResult, result.ToList());
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturnExpectedResult_WhenAllFilters()
    {
        // Arrange
        var subjectUnderTest = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);
        var date = DateTime.Parse("2022-05-06");
        var title = "The Best Movie Ever 3";
        var expectedResult = _showtimesEntities.Where(showtime => string.Equals(showtime.Movie.Title, title, StringComparison.OrdinalIgnoreCase) && date >= showtime.StartDate && date <= showtime.EndDate).ToList();

        _mockShowtimesRepository.Setup(repository => repository.GetCollectionAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
            It.IsAny<Func<IQueryable<ShowtimeEntity>, IOrderedQueryable<ShowtimeEntity>>>(),
            It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await subjectUnderTest.GetAsync(date, title);

        // Assert            
        CollectionAssert.AreEquivalent(expectedResult, result.ToList());
    }

    [TestMethod]
    public async Task CreateAsync_ShouldReturnExpectedResult()
    {
        // Arrange
        IMDBMovieInfo _newMovieInfo = TestDataProvider.Instance.GetNewMovieInfo();
        Movie _newMovie = TestDataProvider.Instance.GetNewMovie();
        MovieEntity _newMovieEntity = TestDataProvider.Instance.GetNewMovieEntity();
        Showtime _newShowtime = TestDataProvider.Instance.GetNewShowtime();
        ShowtimeEntity _newShowtimeEntity = TestDataProvider.Instance.GetNewShowtimeEntity();

        _mockShowtimesRepository.Setup(repository => repository.AddAsync(It.IsAny<ShowtimeEntity>())).ReturnsAsync((ShowtimeEntity entity) =>
        {
            entity.Id = _showtimesEntities.Count;
            _showtimesEntities.Add(entity);

            return entity;
        });

        _mockWebApiClient.Setup(webApiClient => webApiClient.GetMovieInfoAsync(It.Is<string>(imbdId => imbdId == _newMovie.ImdbId))).ReturnsAsync(_newMovieInfo);

        _mockMapper.Setup(mapper => mapper.Map<MovieEntity>(It.Is<IMDBMovieInfo>(movieInfo => movieInfo == _newMovieInfo))).Returns(_newMovieEntity);
        _mockMapper.Setup(mapper => mapper.Map<MovieEntity>(It.Is<Movie>(movie => movie == _newMovie))).Returns(_newMovieEntity);
        _mockMapper.Setup(mapper => mapper.Map<ShowtimeEntity>(It.Is<Showtime>(shwotime => shwotime.Id == _newShowtime.Id))).Returns(_newShowtimeEntity);

        var subjectUnderTest = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);

        // Act
        var result = await subjectUnderTest.CreateAsync(_newShowtime);

        // Assert
        Assert.AreEqual(result, _newShowtimeEntity);
    }

    [TestMethod]
    public async Task PutAsync_ShouldWorkAsExpected_WhenMovieIsNull()
    {
        // Arrange
        var updatedShowtime = new Showtime()
        {
            Id = _showtimesEntities[0].Id,
            StartDate = "2023-08-15",
            EndDate = "2023-08-25",
            Movie = null,
            AuditoriumId = 3,
            Schedule = "18:00"
        };

        var subjectUnderTest = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);

        // Act
        var result = await subjectUnderTest.UpdateAsync(updatedShowtime);

        // Assert
        var expectedMovie = _showtimesEntities[0].Movie;

        Assert.AreEqual(DateTime.Parse(updatedShowtime.StartDate), result.StartDate);
        Assert.AreEqual(DateTime.Parse(updatedShowtime.EndDate), result.EndDate);
        Assert.AreEqual(expectedMovie, result.Movie);
        Assert.AreEqual(updatedShowtime.AuditoriumId, result.AuditoriumId);
        Assert.AreEqual(updatedShowtime.Schedule, string.Join(",", result.Schedule));
    }

    [TestMethod]
    public async Task PutAsync_ShouldWorkAsExpected_WhenMovieIsNotNull()
    {
        // Arrange
        var subjectUnderTest = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object);

        // Act
        var showtime = new Showtime()
        {
            Id = _showtimesEntities[0].Id,
            StartDate = "2023-08-15",
            EndDate = "2023-08-25",
            Movie = new Movie()
            {
                ImdbId = _showtimesEntities[1].Movie.ImdbId,
            },
            AuditoriumId = 3,
            Schedule = "18:00"
        };

        // Assert
        var result = await subjectUnderTest.UpdateAsync(showtime);
        var expectedMovie = _showtimesEntities[1].Movie;

        Assert.AreEqual(DateTime.Parse(showtime.StartDate), result.StartDate);
        Assert.AreEqual(DateTime.Parse(showtime.EndDate), result.EndDate);
        Assert.AreSame(expectedMovie, result.Movie);
        Assert.AreEqual(showtime.AuditoriumId, result.AuditoriumId);
        Assert.AreEqual(showtime.Schedule, string.Join(",", result.Schedule));
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldWorkAsExpected()
    {
        // Arrange
        var subjectUnderTest = new ShowtimeService(_mockShowtimesRepository.Object, _mockMoviesRepository.Object, _mockWebApiClient.Object, _mockMapper.Object, _mockQueueService.Object    );

        _mockShowtimesRepository.Setup(repository => repository.DeleteAsync(It.IsAny<int>()))
            .Callback((int id) =>
        {
            var index = _showtimesEntities.FindIndex(showtime => showtime.Id == id);
            _showtimesEntities.RemoveAt(index);
        });

        // Act
        var count = _showtimesEntities.Count();
        await subjectUnderTest.DeleteAsync(_showtimesEntities[2].Id);

        // Assert
        var result = await subjectUnderTest.GetAsync();
        CollectionAssert.AreEquivalent(_showtimesEntities, result.ToList());
        Assert.AreEqual(count - 1, _showtimesEntities.Count);
    }
}
