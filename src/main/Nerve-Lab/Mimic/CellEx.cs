namespace Kostassoid.Nerve.Lab.Mimic
{
	using System.Linq;
	using System.Reflection;
	using Core;
	using Core.Processing.Operators;
	using LinFu.DynamicProxy;

	public static class CellEx
	{
		private static readonly ProxyFactory Generator = new ProxyFactory();

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
			return Generator.CreateProxy<T>(new RelayingInterceptor(cell));
		}
	}
}