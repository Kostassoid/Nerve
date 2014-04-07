namespace Kostassoid.Nerve.Lab.Mimic.NProxy
{
	using System.Reflection;
	using Core;
	using global::NProxy.Core;

	public class NProxyInterceptor : AbstractInvoker, IInvocationHandler
	{
		public NProxyInterceptor(ICell cell):base(cell)
		{
		}

		public object Invoke(object target, MethodInfo methodInfo, object[] parameters)
		{
			var i = new Invocation(
				methodInfo.Name,
				methodInfo.ReturnType,
				parameters);

			return Invoke(i);
		}
	}
}