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

			methods.ForEach(mi =>
							{
								//var target = Expression.Parameter(typeof (T), "x");
								//var args = mi.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
								//var call = Expression.Call(Expression.Constant(obj), mi, args);
								//var invokeAction = Expression.Lambda(call, args).Compile();

								var invokeAction = FastInvoker.GetMethodInvoker(mi);

								cell.OnStream()
									.Of<Invocation>()
									.Where(i =>
										i.Method == mi.Name &&
										i.Params.Count == mi.GetParameters().Count())
/*
									.ReactWith(i => mi.Invoke(
										obj,
										BindingFlags.InvokeMethod,
										null,
										i.Payload.Params.ToArray(),
										null));
*/
									.ReactWith(i => invokeAction(obj, i.Payload.Params.ToArray()));
							});
		}

		public static T ProxyOf<T>(this ICell cell) where T : class
		{
			return Builder.Build<T>(cell);
		}
	}
}