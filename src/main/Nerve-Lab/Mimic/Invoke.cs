namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;

	public class Invoke<T>
	{
		public static Invocation Using(Expression<Action<T>> expr)
		{
			var methodInfo = (MethodCallExpression)expr.Body;
			return new Invocation(
				methodInfo.Arguments.Select(a =>
					new InvocationParam(((ConstantExpression)a).Value)));
		}
	}
}