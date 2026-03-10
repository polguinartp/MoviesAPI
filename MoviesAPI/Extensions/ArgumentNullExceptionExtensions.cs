using System;

namespace MoviesAPI.Extensions
{
	public static class ArgumentNullExceptionExtensions
	{
		public static void ThrowIfNull(this object obj)
		{
			ArgumentNullException.ThrowIfNull(obj);
		}
	}
}
