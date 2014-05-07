namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Core;
	using Core.Processing.Operators;
	using Core.Proxy;
	using Core.Tools;

	public static class CellEx
	{
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
	}
}