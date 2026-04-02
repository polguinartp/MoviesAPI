using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Extensions;

public record ErrorInfo(int StatusCode, string Message);

public class ExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		await httpContext.Response.WriteAsJsonAsync(new ErrorInfo(httpContext.Response.StatusCode, exception.Message), cancellationToken);

		return true;
	}
}
