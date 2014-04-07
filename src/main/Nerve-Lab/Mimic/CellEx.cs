namespace Kostassoid.Nerve.Lab.Mimic
{
	using System.Linq;
	using System.Reflection;
	using Core;
	using Core.Processing.Operators;
	using NProxy;

	public static class CellEx
	{
		private static readonly IProxyBuilder Builder = new NProxyBuilder();

		public static void BindTo<T>(this ICell cell, T obj)
		{
			var type = typeof(T);
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();

			methods.ForEach(mi => cell.OnStream()
			                          .Of<Invocation>()
			                          .Where(i =>
			                                 i.Method == mi.Name &&
			                                 i.Params.Count == mi.GetParameters().Count())
			                          .ReactWith(i => mi.Invoke(
				                          obj,
				                          BindingFlags.InvokeMethod,
				                          null,
				                          i.Payload.Params.ToArray(),
				                          null)));
		}

		public static T ProxyOf<T>(this ICell cell) where T : class
		{
			return Builder.Build<T>(cell);
		}
	}
}