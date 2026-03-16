using System;

namespace MoviesAPI.Extensions;

public static class ExceptionExtensions
{
	public static void ThrowIfNull(this object obj)
	{
		ArgumentNullException.ThrowIfNull(obj);
	}
}
