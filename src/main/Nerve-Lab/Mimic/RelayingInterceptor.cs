namespace Kostassoid.Nerve.Lab.Mimic
{
	using System.Linq;
	using Castle.DynamicProxy;
	using Core;

	public class RelayingInterceptor : IInterceptor
	{
		readonly ICell _cell;

		public RelayingInterceptor(ICell cell)
		{
			_cell = cell;
		}

		public void Intercept(IInvocation invocation)
		{
			var i = new Invocation(
				invocation.Method.Name,
				invocation.Method.ReturnType,
				invocation.Arguments);

			_cell.Send(i);
		}
	}
}