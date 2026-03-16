using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace MoviesAPI.Exceptions;

public record ErrorInfo (int StatusCode, string Message);

/// <summary>
/// Represents a class that configures the Exception middleware.
/// </summary>
public static class ExceptionMiddlewareExtensions
{
	/// <summary>
	/// Method that ensures that the Exception middelware in the pipeline captures all the exceptions and puts them in the Response object in the required way.
	/// </summary>
	/// <param name="app"></param>
	public static void ConfigureExceptionHandler(this IApplicationBuilder app)
	{
		app.UseExceptionHandler(appError =>
		{
			appError.Run(async context =>
					{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					context.Response.ContentType = "application/json";

					var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
					if (contextFeature != null)
					{
						var errorInfo = new ErrorInfo(context.Response.StatusCode, $"Internal Server Error: {contextFeature.Error.Message}");
						await context.Response.WriteAsync(errorInfo.ToString());
					}
				});
		});
	}
}
