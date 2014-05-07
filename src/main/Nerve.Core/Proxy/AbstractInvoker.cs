// Copyright 2014 https://github.com/Kostassoid/Nerve
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//  
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace Kostassoid.Nerve.Core.Proxy
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Core;
	using Tools;
	using Tpl;

	internal abstract class AbstractInvoker
	{
		//TODO: make thread-safe
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