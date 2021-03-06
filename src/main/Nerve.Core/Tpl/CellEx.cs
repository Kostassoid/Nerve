﻿// Copyright 2014 https://github.com/Kostassoid/Nerve
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

namespace Kostassoid.Nerve.Core.Tpl
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Fasterflect;
	using Tools;

	/// <summary>
	/// Cell Tpl extension methods
	/// </summary>
	public static class CellEx
	{
		static readonly Func<Type, MethodInvoker> BuildSender = payloadType =>
			{
				var send = typeof (Cell)
					.GetMethods().Single(mi => mi.Name == "Send" && mi.GetParameters().Length == 2)
					.MakeGenericMethod(payloadType);

				return send.DelegateForCallMethod();
			};

		static readonly Func<Type, MethodInvoker> SenderResolver = BuildSender.AsMemoized();

		/// <summary>
		/// Sends signal and returns a typed task to expect the return.
		/// </summary>
		/// <typeparam name="T">Return value type.</typeparam>
		/// <param name="cell">Cell to send through.</param>
		/// <param name="payload">Signal payload to send.</param>
		/// <returns>Future result.</returns>
		public static Task<T> SendFor<T>(this ICell cell, object payload)
		{
			var handler = new TaskResultHandlerOf<T>();

			var sender = SenderResolver(payload.GetType());

			sender.Invoke(cell, new[] { payload, handler });

			return handler.TypedTask;
		}
	}
}