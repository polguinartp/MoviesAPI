using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MoviesAPI.DTOs.API;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.ActionFilters
{
    public class ShowtimeActionFilter : IAsyncActionFilter
    {
        private readonly IRepository<AuditoriumEntity> _repository;

        public ShowtimeActionFilter(IRepository<AuditoriumEntity> repository)
        {
            _repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var param = context.ActionArguments.SingleOrDefault(p => p.Value is Showtime);
            if (param.Value == null)
            {
                context.Result = new BadRequestObjectResult("Null Showtime object");
                return;
            }

            var showtime = param.Value as Showtime;
            if (await _repository.GetByIdAsync(showtime.AuditoriumId) == null)
            {
                context.Result = new NotFoundObjectResult($"Not found Auditorium with id {showtime.AuditoriumId}");
                return;
            }

            await next();
        }
    }
}
