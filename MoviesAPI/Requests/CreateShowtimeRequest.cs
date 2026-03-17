using Mediator;
using MoviesAPI.DTOs.Requests;
using MoviesAPI.DTOs.Responses;

namespace MoviesAPI.Requests;

public record CreateShowtimeRequest(ShowtimeRequest ShowtimeRequest) : IRequest<ShowtimeResponse>;
