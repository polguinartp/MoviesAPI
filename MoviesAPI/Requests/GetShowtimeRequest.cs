using Mediator;
using MoviesAPI.DTOs.Responses;
using System;
using System.Collections.Generic;

namespace MoviesAPI.Requests;

public record GetShowtimeRequest(DateTime? Date, string? MovieTitle) : IRequest<List<ShowtimeResponse>>;
