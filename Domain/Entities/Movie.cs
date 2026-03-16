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

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (ReferenceEquals(obj, this))
		{
			return true;
		}
		if (obj is not Movie)
		{
			return false;
		}

		return ImdbId == ((Movie)obj).ImdbId;
	}

	public override int GetHashCode()
	{
		return ImdbId.GetHashCode();
	}
}
