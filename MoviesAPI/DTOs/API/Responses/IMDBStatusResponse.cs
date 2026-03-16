using System;

namespace MoviesAPI.DTOs.API.Responses;

public class IMDBStatusResponse
{
	public bool Up { get; set; }

	public DateTime LastCall { get; set; }
}
