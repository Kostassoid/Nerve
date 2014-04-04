namespace Kostassoid.Nerve.Lab.Mimic
{
	using System.Linq;
	using System.Reflection;
	using Castle.DynamicProxy;
	using Core;
	using Core.Processing.Operators;

	public static class CellEx
	{
		private static readonly ProxyGenerator Generator = new ProxyGenerator();

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
			var target = typeof (T);
			if (target.IsInterface)
			{
				return Generator
					.CreateInterfaceProxyWithoutTarget<T>(new RelayingInterceptor(cell));
			}
			else
			{
				return Generator
					.CreateClassProxy<T>(new RelayingInterceptor(cell));
			}
		}
	}
}