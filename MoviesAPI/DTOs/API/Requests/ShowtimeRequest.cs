using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs.API.Requests;

public class ShowtimeRequest
{
	[Required]
	public string StartDate { get; set; }

	[Required]
	public string EndDate { get; set; }

	[Required]
	public string Schedule { get; set; }

	public string MovieId { get; set; }

	[Required]
	public int AuditoriumId { get; set; }
}
