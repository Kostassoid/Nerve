namespace Kostassoid.Nerve.Lab.Mimic
{
	using System.Linq;
	using System.Reflection;
	using Core;
	using Core.Processing.Operators;

	public static class CellEx
	{
		private static ProxyGenerator _generator = new ProxyGenerator("Nerve-Gen");

		public static void Wrap<T>(this ICell cell, T obj)
		{
			var type = typeof(T);
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();

			methods.ForEach(mi =>
				{
					cell.OnStream()
					    .Of<Invocation>()
					    .Where(i => i.Params.Count == mi.GetParameters().Count())
					    .ReactWith(i =>
						    {
							    mi.Invoke(
									obj,
									BindingFlags.InvokeMethod,
									null,
									i.Payload.Params.Select(p => p.Value).ToArray(),
									null);
						    });
				});
		}

		public static T ProxyOf<T>(this ICell cell) where T : class
		{
			return _generator.Generate<T>(cell);
		}
	}
}