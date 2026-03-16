using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs.API.Responses;

public class ShowtimeResponse
{
	public int Id { get; set; }

	public string StartDate { get; set; }

	public string EndDate { get; set; }

	public string Schedule { get; set; }

	public MovieResponse Movie { get; set; }

	public int AuditoriumId { get; set; }
}
