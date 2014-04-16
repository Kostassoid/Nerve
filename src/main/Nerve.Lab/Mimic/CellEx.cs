namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Core;
	using Core.Processing.Operators;
	using Core.Tools;
	using NProxy;

	public static class CellEx
	{
		private static readonly IProxyBuilder Builder = new NProxyBuilder();

		public static void BindTo<T>(this ICell cell, T obj)
		{
			var type = typeof(T);
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();

			methods.ForEach(mi =>
							{
								var invokeAction = FastInvoker.GetMethodInvoker(mi);
								var name = mi.Name;
								var argsCount = mi.GetParameters().Length;

								cell.OnStream()
									.Of<Invocation>()
									.Where(i =>
										i.Method.Equals(name, StringComparison.Ordinal) &&
										i.Params.Length == argsCount)
									.ReactWith(i => invokeAction(obj, i.Payload.Params));
							});
		}

		public static T ProxyOf<T>(this ICell cell) where T : class
		{
			return Builder.Build<T>(cell);
		}
	}
}