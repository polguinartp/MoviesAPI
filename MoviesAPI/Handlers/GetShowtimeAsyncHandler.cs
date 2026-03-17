using AutoMapper;
using Infrastructure.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs.Responses;
using MoviesAPI.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Handlers
{
	public class GetShowtimeHandler(IMapper mapper, CinemaDbContext dbContext) : IRequestHandler<GetShowtimeRequest, List<ShowtimeResponse>>
	{
		public async ValueTask<List<ShowtimeResponse>> Handle(GetShowtimeRequest request, CancellationToken cancellationToken)
		{
			if (request.Date is null && string.IsNullOrEmpty(request.MovieTitle))
			{
				return mapper.Map<List<ShowtimeResponse>>(await dbContext.Showtimes.AsNoTracking().Include(x => x.Movie).ToListAsync(cancellationToken));
			}

			var query = dbContext.Showtimes.AsNoTracking().Include(x => x.Movie).AsQueryable();
			if (request.Date.HasValue)
			{
				query = query.Where(x => x.StartDate.Date >= request.Date && x.EndDate.Date <= request.Date);
			}
			if (!string.IsNullOrEmpty(request.MovieTitle))
			{
				query = query.Where(x => x.Movie.Title.Contains(request.MovieTitle, StringComparison.OrdinalIgnoreCase));
			}

			return mapper.Map<List<ShowtimeResponse>>(await query.ToListAsync(cancellationToken));
		}
	}
}
