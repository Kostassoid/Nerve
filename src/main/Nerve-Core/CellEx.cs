namespace Kostassoid.Nerve.Core
{
	using System;
	using System.Threading.Tasks;
	using Tpl;

	public static class CellEx
	{
		public static Task<T> SendFor<T>(this ICell cell, object payload)
			where T : class
		{
			var handler = new TaskResultHandlerOf<T>();
			cell.Send(payload);
			return handler.TypedTask;
		}
	}
}