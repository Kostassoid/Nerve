﻿namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Core;
	using Core.Processing;
	using Core.Tpl;
	using LinFu.DynamicProxy;

	public class RelayingInterceptor : IInterceptor
	{
		readonly ICell _cell;

		public RelayingInterceptor(ICell cell)
		{
			_cell = cell;
		}

		public object Intercept(InvocationInfo info)
		{
			var i = new Invocation(
				info.TargetMethod.Name,
				info.TargetMethod.ReturnType,
				info.Arguments);

			if (info.TargetMethod.ReturnType == typeof (void))
			{
				_cell.Send(i);
				return null;
			}

			//TODO: improve
			if (typeof (Task).IsAssignableFrom(info.TargetMethod.ReturnType))
			{
				var taskResultHandler = TaskResultHandler.For(info.TargetMethod.ReturnType.GetGenericArguments().Single());
				_cell.Send(i, taskResultHandler);
				return taskResultHandler.Task;
			}

			var resultHandler = new TaskResultHandlerOf<object>();
			_cell.Send(i, resultHandler);
			return resultHandler.TypedTask.Result;
		}
	}
}