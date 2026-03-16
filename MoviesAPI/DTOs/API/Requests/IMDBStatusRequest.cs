using System;

namespace MoviesAPI.DTOs.API.Requests;

public class IMDBStatusRequest
{
	public bool Up { get; set; }

	public DateTime LastCall { get; set; }
}
