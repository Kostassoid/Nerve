namespace EventStore
{
	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Linking;

	public static class EventBus
	{
		static readonly ICell Cell = new Cell("EventBus");

		public static void Raise<T>(T ev) where T : class
		{
			Cell.Send(ev);
		}

		public static ILinkJunction OnStream()
		{
			return Cell.OnStream();
		}
	}
}