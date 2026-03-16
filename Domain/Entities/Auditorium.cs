using System.Collections.Generic;

namespace Domain.Entities;

public class Auditorium
{
	public int Id { get; set; }
	public List<Showtime> Showtimes { get; set; } = [];
	public int Seats { get; set; }
}
