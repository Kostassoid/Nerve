namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	public class Invoke<T>
	{
		// ReSharper disable StaticFieldInGenericType
		private static readonly IDictionary<string, Func<object>> CompiledCache
		// ReSharper restore StaticFieldInGenericType
			= new Dictionary<string, Func<object>>(); 

		public static Invocation Using(Expression<Action<T>> expr)
		{
			var methodInfo = (MethodCallExpression)expr.Body;
			return new Invocation(methodInfo.Method.Name,
				methodInfo.Method.ReturnType,
				methodInfo.Arguments
				.Select(Evaluate)
				.ToArray());
		}

		private static object Evaluate(Expression e)
		{
			var key = e.ToString();
			Func<object> resolver;
			if (!CompiledCache.TryGetValue(key, out resolver))
			{
				resolver = Expression.Lambda<Func<object>>(Expression.Convert(e, typeof(object))).Compile();
				CompiledCache.Add(key, resolver);
			}

			return resolver();

/*
			var constant = e as ConstantExpression;
			if (constant != null)
			{
				return constant.Value;
			}
*/

		}
	}
}