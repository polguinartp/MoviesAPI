using Mediator;
using MoviesAPI.DTOs.Requests;
using MoviesAPI.DTOs.Responses;

namespace MoviesAPI.Requests;

public record UpdateShowtimeRequest(int Id, ShowtimeRequest ShowtimeRequest) : IRequest<ShowtimeResponse>;
