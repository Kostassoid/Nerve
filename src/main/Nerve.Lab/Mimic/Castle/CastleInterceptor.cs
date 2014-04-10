namespace Kostassoid.Nerve.Lab.Mimic.Castle
{
	using Core;
	using global::Castle.DynamicProxy;

	public class CastleInterceptor : AbstractInvoker, IInterceptor
	{
		public CastleInterceptor(ICell cell):base(cell)
		{
		}

		public void Intercept(IInvocation invocation)
		{
			var i = new Invocation(
				invocation.Method.Name,
				invocation.Method.ReturnType,
				invocation.Arguments);

			invocation.ReturnValue = Invoke(i);
		}
	}
}