using MoviesAPI.DTOs.API.Responses;

namespace MoviesAPI.Background;

public class IMDBStatusSingleton
{
	private readonly static IMDBStatusSingleton _instance = new();

	private IMDBStatusSingleton() { }

	public static IMDBStatusSingleton Instance => _instance;

	public IMDBStatusResponse Status { get; set; }
}
