using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MoviesAPI.Extensions;

public static class ServiceRegisterExtension
{
	public static void RegisterOptions<T>(this IServiceCollection services, IConfiguration section) where T : class
	{
		services
		.AddOptions<T>()
		.Bind(section)
		.ValidateDataAnnotations()
		.ValidateOnStart();

		services.TryAddSingleton(sp => sp.GetRequiredService<IOptions<T>>().Value);
	}
}