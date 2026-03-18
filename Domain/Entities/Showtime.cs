using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Showtime
{
	public int Id { get; set; }
	public Movie Movie { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public List<string> Schedule { get; set; } = [];
	public int AuditoriumId { get; set; }
	public Auditorium Auditorium { get; set; }
}
