namespace Kostassoid.Nerve.Core.Tools
{
	using System;
	using System.Collections.Generic;

	public static class EnumerableEx
	{
		public static IEnumerable<T> SelectDeep<T>(
			this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
		{
			if (source == null)
				yield break;

			foreach (T item in source)
			{
				yield return item;
				foreach (T subItem in SelectDeep(selector(item), selector))
				{
					yield return subItem;
				}
			}
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source) action(item);
		}
	}
}