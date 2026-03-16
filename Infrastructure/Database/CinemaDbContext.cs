using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Infrastructure.Database;

public class CinemaDbContext(DbContextOptions<CinemaDbContext> options) : DbContext(options)
{
	public DbSet<Auditorium> Auditoriums { get; set; }
	public DbSet<Showtime> Showtimes { get; set; }
	public DbSet<Movie> Movies { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Auditorium>(build =>
		{
			build.HasKey(a => a.Id);
			build.Property(a => a.Id).ValueGeneratedOnAdd();
			build.HasMany(a => a.Showtimes).WithOne(a => a.Auditorium).HasForeignKey(s => s.AuditoriumId);
		});

		modelBuilder.Entity<Showtime>(build =>
		{
			build.HasKey(s => s.Id);
			build.Property(s => s.Id).ValueGeneratedOnAdd();
			build.Property(s => s.Schedule).HasConversion(x => string.Join(",", x), y => y.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList());
			build.HasOne(s => s.Movie).WithOne(m => m.Showtime);
		});

		modelBuilder.Entity<Movie>(build =>
		{
			build.HasKey(entry => entry.Id);
			build.Property(entry => entry.Id).ValueGeneratedOnAdd();
		});
	}
}
