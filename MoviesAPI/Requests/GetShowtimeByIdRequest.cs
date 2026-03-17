using Mediator;
using MoviesAPI.DTOs.Responses;

namespace MoviesAPI.Requests;

public record GetShowtimeByIdRequest(int Id) : IRequest<ShowtimeResponse>;
