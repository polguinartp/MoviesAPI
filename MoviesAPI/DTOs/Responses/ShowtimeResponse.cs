namespace MoviesAPI.DTOs.Responses;

public record ShowtimeResponse(int Id, string StartDate, string EndDate, string Schedule, MovieResponse Movie, int AuditoriumId);
