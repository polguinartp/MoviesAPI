using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MoviesAPI.Extensions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MoviesAPI.Middlewares;

/// <summary>
/// Middleware class that, if injected in the pipeline, it logs all the requests times.
/// </summary>
public class RequestLoggerMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<RequestLoggerMiddleware> _logger;

	public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
	{
		_next = next ?? throw new ArgumentNullException(nameof(next));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var watch = new Stopwatch();
		watch.Start();

		context.Response.OnStarting(() =>
		{
			watch.Stop();
			_logger.LogExecutionTime(context.Request.Path, context.Request.Method, watch.ElapsedMilliseconds);

			return Task.CompletedTask;
		});

		await _next(context);
	}
}
