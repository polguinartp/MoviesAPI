using AutoMapper;
using Domain.Entities;
using MoviesAPI.DTOs.IMDB;
using MoviesAPI.DTOs.Requests;
using MoviesAPI.DTOs.Responses;
using System;
using System.Linq;

namespace MoviesAPI.Profilers;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
		CreateMap<Showtime, ShowtimeResponse>()
				.ForMember(dest => dest.StartDate, o => o.MapFrom(src => src.StartDate.ToString()))
				.ForMember(dest => dest.EndDate, o => o.MapFrom(src => src.EndDate.ToString()))
				.ForMember(dest => dest.Schedule, o => o.MapFrom(src => string.Join(',', src.Schedule)));

		CreateMap<ShowtimeRequest, Showtime>()
				.ForMember(dest => dest.StartDate, o => o.MapFrom(src => DateTime.Parse(src.StartDate)))
				.ForMember(dest => dest.EndDate, o => o.MapFrom(src => DateTime.Parse(src.EndDate)))
				.ForMember(dest => dest.Schedule, o => o.MapFrom(src => src.Schedule.Split(',', StringSplitOptions.None).ToList()));

		CreateMap<Movie, MovieResponse>();

		CreateMap<IMDBMovieInfo, Movie>();
	}
}
