namespace EventStore
{
	using System.Threading.Tasks;

	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Tpl;

	public static class EventBus
	{
		static readonly ICell Cell = new Cell("EventBus");

		public static ILinkJunction OnStream()
		{
			return Cell.OnStream();
		}

		public static void Raise<T>(T ev) where T : class
		{
			Cell.Send(ev);
		}

		public static Task RaiseWithTask<T>(T ev) where T : class
		{
			var taskHandler = new TaskResultHandlerOf<object>();
			Cell.Send(ev, taskHandler);
			return taskHandler.Task;
		}
	}
}