using Microsoft.Extensions.Logging;

namespace MoviesAPI.Extensions;

internal static partial class LogMessageExtensions
{
	[LoggerMessage(LogLevel.Information, message: "Execution time for call to {requestPath}/{requestMethod}: {ms} ms")]
	public static partial void LogExecutionTime(this ILogger logger, string requestPath, string requestMethod, long ms);
}
