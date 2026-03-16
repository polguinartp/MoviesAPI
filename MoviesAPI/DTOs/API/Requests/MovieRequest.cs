using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs.API.Requests;

public class MovieRequest
{
	public string Title { get; set; }

	[Required]
	public string ImdbId { get; set; }

	public string Stars { get; set; }

	public DateTime ReleaseDate { get; set; }
}
