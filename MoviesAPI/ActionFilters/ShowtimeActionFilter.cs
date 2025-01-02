using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MoviesAPI.DTOs.API;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.ActionFilters;

public class ShowtimeActionFilter : IAsyncActionFilter
{
    private readonly IRepository<AuditoriumEntity> _auditoriumRepository;

    public ShowtimeActionFilter(IRepository<AuditoriumEntity> auditoriumRepository)
    {
        ArgumentNullException.ThrowIfNull(auditoriumRepository);
        _auditoriumRepository = auditoriumRepository;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var showtime = context.ActionArguments.SingleOrDefault(p => p.Value is Showtime).Value as Showtime;
        if (showtime == null)
        {
            context.Result = new BadRequestObjectResult("Null Showtime object");
            return;
        }
        if (showtime?.Movie?.ImdbId == null)
        {
            context.Result = new NotFoundObjectResult("Movie.ImdbId missing");
            return;
        }
        if (await _auditoriumRepository.GetByIdAsync(showtime.AuditoriumId) == null)
        {
            context.Result = new NotFoundObjectResult($"Not found Auditorium with id {showtime.AuditoriumId}");
            return;
        }

        await next();
    }
}
