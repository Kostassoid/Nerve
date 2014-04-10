namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Core;
	using Core.Tools;
	using Core.Tpl;

	public abstract class AbstractInvoker
	{
		private static readonly IDictionary<Type, Func<ITaskResultHandler>> HandlerTypeMap
			= new Dictionary<Type, Func<ITaskResultHandler>>();

		readonly ICell _cell;

		protected AbstractInvoker(ICell cell)
		{
			_cell = cell;
		}

		public object Invoke(Invocation invocation)
		{
			if (invocation.Expects == typeof(void))
			{
				_cell.Send(invocation);
				return null;
			}

			//TODO: improve
			Func<ITaskResultHandler> handlerFactory;
			if (!HandlerTypeMap.TryGetValue(invocation.Expects, out handlerFactory))
			{
				if (typeof(Task).IsAssignableFrom(invocation.Expects))
				{
					var typeArg = invocation.Expects.GetGenericArguments().Single();
					var handlerType = typeof (TaskResultHandlerOf<>).MakeGenericType(typeArg);
					handlerFactory = () => (ITaskResultHandler)New.InstanceOf(handlerType);
					HandlerTypeMap[invocation.Expects] = handlerFactory;
				}
			}

			if (handlerFactory != null)
			{
				var taskResultHandler = handlerFactory();
				_cell.Send(invocation, taskResultHandler.Processor);
				return taskResultHandler.Task;
			}

			var resultHandler = new TaskResultHandlerOf<object>();
			_cell.Send(invocation, resultHandler);
			return resultHandler.TypedTask.Result;
		}
	}
}