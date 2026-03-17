namespace MoviesAPI.DTOs.Requests;

public record ShowtimeRequest(
	string StartDate,
	string EndDate,
	string Schedule,
	string MovieId,
	int AuditoriumId);
