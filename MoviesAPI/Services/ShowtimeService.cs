using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Domain.Specifications;
//using Infrastructure.SQS.Services;
using MoviesAPI.DTOs.API;
using MoviesAPI.Specifications;
using MoviesAPI.WebClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services;

public class ShowtimeService : IShowtimeService
{
    private readonly IRepository<ShowtimeEntity> _showtimeEntitiesRepository;
    private readonly IRepository<MovieEntity> _movieEntitiesRepository;
    private readonly IIMDBWebApiClient _webClient;
    private readonly IMapper _mapper;
    //private readonly IQueueService _queueService;

    public ShowtimeService(IRepository<ShowtimeEntity> showtimeEntitiesRepository,
        IRepository<MovieEntity> movieEntitiesRepository,
        IIMDBWebApiClient webClient,
        IMapper mapper,
        IQueueService queueService)
    {
        ArgumentNullException.ThrowIfNull(showtimeEntitiesRepository);
        ArgumentNullException.ThrowIfNull(webClient);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(movieEntitiesRepository);
        ArgumentNullException.ThrowIfNull(queueService);

        _showtimeEntitiesRepository = showtimeEntitiesRepository;
        _webClient = webClient;
        _mapper = mapper;
        _movieEntitiesRepository = movieEntitiesRepository;
        //_queueService = queueService;
    }

    public async Task<IEnumerable<ShowtimeEntity>> GetAsync(DateTime date = default, string movieTitle = null)
    {
        IEnumerable<ShowtimeEntity> entities;
        var includeProperties = "Movie";

        if (date == default && string.IsNullOrEmpty(movieTitle))
        {
            entities = await _showtimeEntitiesRepository.GetCollectionAsync(includeProperties: includeProperties);
        }
        else
        {
            var specification = Specification<ShowtimeEntity>.All;
            if (date != default)
            {
                specification = specification.And(new ReleaseDateOlderThanSixMontsSpecification(date));
            }
            if (!string.IsNullOrEmpty(movieTitle))
            {
                specification = specification.And(new MovieTitleSpecification(movieTitle));
            }

            entities = await _showtimeEntitiesRepository.GetCollectionAsync(filter: specification.ToExpression(),
                includeProperties: includeProperties);
        }

        return entities;
    }

    public async Task<ShowtimeEntity> GetByIdAsync(int id)
    {
        var includeProperties = "Movie";
        return await _showtimeEntitiesRepository.GetByIdAsync(id, includeProperties);
    }

    public async Task<ShowtimeEntity> CreateAsync(Showtime showtime)
    {
        var showtimeEntity = _mapper.Map<ShowtimeEntity>(showtime);

        var movieInfo = await _webClient.GetMovieInfoAsync(showtime.Movie.ImdbId);
        showtimeEntity.Movie = _mapper.Map<MovieEntity>(movieInfo);

        var movies = await _movieEntitiesRepository.GetCollectionAsync();
        var existingMovie = movies.FirstOrDefault(x => x.Equals(showtimeEntity.Movie));
        if (existingMovie != null)
        {
            showtimeEntity.Movie = existingMovie;
        }

        return await _showtimeEntitiesRepository.AddAsync(showtimeEntity);
    }

    public async Task<ShowtimeEntity> UpdateAsync(Showtime showtime)
    {
        var existingEntity = await _showtimeEntitiesRepository.GetByIdAsync(showtime.Id);
        _mapper.Map(showtime, existingEntity);

        if (showtime.Movie != null && showtime.Movie.ImdbId != existingEntity.Movie.ImdbId)
        {
            var movieInfo = await _webClient.GetMovieInfoAsync(showtime.Movie.ImdbId);
            existingEntity.Movie = _mapper.Map<MovieEntity>(movieInfo);
        }

        return await _showtimeEntitiesRepository.UpdateAsync(existingEntity);
    }

    public async Task DeleteAsync(int id)
    {
        await _showtimeEntitiesRepository.DeleteAsync(id);

        // aws free tier finished -> every 1M messages costs
        //QueueMessage queueMessage = new QueueMessage()
        //{
        //    Id = Guid.NewGuid(),
        //    Message = $"Showtime {id} has been deleted.",
        //    DateTime = DateTime.Now
        //};
        //await _queueService.SendAsync(queueMessage);
    }
}
