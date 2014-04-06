namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Castle.DynamicProxy;
	using Core;
	using Core.Tpl;

	public class CastleInterceptor : IInterceptor
	{
		private static readonly IDictionary<Type, Func<TaskResultHandler>> HandlerTypeMap
			= new Dictionary<Type, Func<TaskResultHandler>>();

		readonly ICell _cell;

		public CastleInterceptor(ICell cell)
		{
			_cell = cell;
		}

		public void Intercept(IInvocation invocation)
		{
			var i = new Invocation(
				invocation.Method.Name,
				invocation.Method.ReturnType,
				invocation.Arguments);

			if (i.Expects == typeof(void))
			{
				_cell.Send(i);
				return;
			}

			//TODO: improve
			Func<TaskResultHandler> handlerFactory;
			if (!HandlerTypeMap.TryGetValue(i.Expects, out handlerFactory))
			{
				if (typeof(Task).IsAssignableFrom(i.Expects))
				{
					handlerFactory = () => TaskResultHandler.Of(i.Expects.GetGenericArguments().Single());
					HandlerTypeMap[i.Expects] = handlerFactory;
				}
			}

			if (handlerFactory != null)
			{
				var taskResultHandler = handlerFactory();
				_cell.Send(i, taskResultHandler);
				invocation.ReturnValue = taskResultHandler.Task;
				return;
			}

			var resultHandler = new TaskResultHandlerOf<object>();
			_cell.Send(i, resultHandler);
			invocation.ReturnValue = resultHandler.TypedTask.Result;
		}
	}
}