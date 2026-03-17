using System;

namespace MoviesAPI.DTOs.Responses;

public record MovieResponse(string Title, string ImdbId, string Stars, DateTime ReleaseDate);
