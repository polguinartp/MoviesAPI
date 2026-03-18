using Microsoft.Extensions.Hosting;
using MoviesAPI.DTOs.Responses;
using MoviesAPI.Options;
using MoviesAPI.WebClients;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Background;

/// <summary>
/// Represents a class that checks the status for the IMDB Api in a background task.
/// </summary>
public class IMDBStatusBackgroundTask(IIMDBWebApiClient webApiClient, IMDBWebApiClientOptions options, IMDBStatusProvider iMDBStatusService) : IHostedService, IDisposable
{
	private Timer _timer;

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_timer = new Timer(async o =>
		{
			var lastCall = DateTime.Now;
			var status = await webApiClient.GetStatusAsync();
			
			var newStatus = new IMDBStatusResponse(
				Up: status == System.Net.HttpStatusCode.OK,
				LastCall: lastCall
			);

			iMDBStatusService.Status = newStatus;
		},
		null,
		TimeSpan.Zero,
		TimeSpan.FromSeconds(options.StatusTimestamp));

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_timer?.Change(Timeout.Infinite, 0);
		return Task.CompletedTask;
	}

	public void Dispose()
	{
		_timer?.Dispose();
	}
}
