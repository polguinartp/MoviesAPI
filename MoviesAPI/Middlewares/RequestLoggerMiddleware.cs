using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MoviesAPI.Extensions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MoviesAPI.Middlewares;

/// <summary>
/// Middleware class that, if injected in the pipeline, logs all the requests times.
/// </summary>
public class RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
{
	public async Task InvokeAsync(HttpContext context)
	{
		var watch = new Stopwatch();
		watch.Start();

		context.Response.OnStarting(() =>
		{
			watch.Stop();
			logger.LogExecutionTime(context.Request.Path, context.Request.Method, watch.ElapsedMilliseconds);

			return Task.CompletedTask;
		});

		await next(context);
	}
}
