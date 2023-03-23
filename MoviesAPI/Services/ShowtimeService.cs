using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Domain.Specifications;
using MoviesAPI.DTOs.API;
using MoviesAPI.Specifications;
using MoviesAPI.WebClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IRepository<ShowtimeEntity> _showtimeEntitiesRepository;
        private readonly IRepository<MovieEntity> _movieEntitiesRepository;
        private readonly IIMDBWebApiClient _webClient;
        private readonly IMapper _mapper;

        public ShowtimeService(IRepository<ShowtimeEntity> showtimeEntitiesRepository,
            IRepository<MovieEntity> movieEntitiesRepository,
            IIMDBWebApiClient webClient,
            IMapper mapper)
        {
            _showtimeEntitiesRepository = showtimeEntitiesRepository ?? throw new ArgumentNullException(nameof(showtimeEntitiesRepository));
            _webClient = webClient ?? throw new ArgumentNullException(nameof(webClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _movieEntitiesRepository = movieEntitiesRepository ?? throw new ArgumentNullException(nameof(movieEntitiesRepository));
        }

        public async Task<IEnumerable<ShowtimeEntity>> GetAsync(DateTime date = default, string movieTitle = null)
        {
            IEnumerable<ShowtimeEntity> entities = new List<ShowtimeEntity>();
            var includeProperties = "Movie";

            if (date == default && movieTitle == null)
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
                if (movieTitle != null)
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
        }
    }
}
