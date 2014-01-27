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

using System;
using System.Linq;

namespace Kostassoid.Nerve.Core
{
	using System.Collections.Generic;
	using Pipeline;
	using Signal;
	using Tools;
	using Tools.CodeContracts;

	public class Cell : ICell
	{
		readonly ISet<Synapse> _links = new HashSet<Synapse>();

		public string Name { get; private set; }

		public event EventHandler<UnhandledExceptionEventArgs> UnhandledException = (sender, args) => {};

		public Cell(string name = null)
		{
			Name = name;
		}

		public void Dispose()
		{
			_links.ToArray().ForEach(l => l.Dispose());
			_links.Clear();
		}

		public void Fire<T>(T body) where T : class
		{
			Requires.NotNull(body, "body");

			Fire(new Signal<T>(body, new StackTrace(this)) as ISignal);
		}

		public void Fire(ISignal signal)
		{
			Requires.NotNull(signal, "signal");

			_links.ForEach(l => l.Process(signal));
		}

		public IPipelineStep OnStream()
		{
			return new Synapse(this).Pipeline;
		}

		internal void Attach(Synapse synapse)
		{
			Requires.NotNull(synapse, "synapse");

			_links.Add(synapse);
		}

		internal void Detach(Synapse synapse)
		{
			Requires.NotNull(synapse, "synapse");

			_links.Remove(synapse);
		}

		public IEmitterOf<T> GetEmitterOf<T>() where T : class
		{
			return new EmitterOf<T>(this);
		}

		public void Handle(ISignal signal)
		{
			Requires.NotNull(signal, "signal");

			signal.Trace(this);
			Fire(signal);
		}

		public bool OnFailure(SignalHandlingException exception)
		{
			UnhandledException(this, new UnhandledExceptionEventArgs(exception, false));
			return true;
		}

		public override string ToString()
		{
			return string.Format("Cell [{0}]", Name ?? "unnamed");
		}
	}
}