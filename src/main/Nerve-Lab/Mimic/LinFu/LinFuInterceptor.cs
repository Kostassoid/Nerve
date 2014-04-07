namespace Kostassoid.Nerve.Lab.Mimic.LinFu
{
	using Core;
	using global::LinFu.DynamicProxy;

	public class LinFuInterceptor : AbstractInvoker, IInterceptor
	{
		public LinFuInterceptor(ICell cell):base(cell)
		{
		}

		public object Intercept(InvocationInfo info)
		{
			var i = new Invocation(
				info.TargetMethod.Name,
				info.TargetMethod.ReturnType,
				info.Arguments);

			return Invoke(i);
		}
	}
}