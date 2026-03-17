using System;

namespace MoviesAPI.DTOs.IMDB;

public record IMDBMovieInfo(string Title, DateTime ReleaseDate, string ImdbId, string Stars);
