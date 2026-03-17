using System;

namespace MoviesAPI.DTOs.Responses;

public record IMDBStatusResponse(bool Up, DateTime LastCall);
