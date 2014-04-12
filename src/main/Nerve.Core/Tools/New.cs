namespace Kostassoid.Nerve.Core.Tools
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	internal static class New
	{
		private static readonly IDictionary<Type, Func<object>> Cache
 			= new Dictionary<Type, Func<object>>();

		public static object InstanceOf(Type type)
		{
			Func<object> factory;
			if (!Cache.TryGetValue(type, out factory))
			{
				factory = Expression
				.Lambda<Func<object>>(Expression.New(type))
				.Compile();

				Cache[type] = factory;
			}

			return factory();
		}
	}
}