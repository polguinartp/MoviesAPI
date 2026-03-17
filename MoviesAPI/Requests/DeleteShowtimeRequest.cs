using Mediator;

namespace MoviesAPI.Requests;

public record DeleteShowtimeRequest(int Id) : IRequest;
