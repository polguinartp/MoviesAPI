using System;

namespace Domain.Entities;

public class Movie
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string ImdbId { get; set; }
	public string Stars { get; set; }
	public DateTime ReleaseDate { get; set; }
	public int ShowtimeId { get; set; }
	public Showtime Showtime { get; set; }

	public override int GetHashCode()
	{
		return ImdbId.GetHashCode();
	}
}
