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

using System.Linq;

namespace Kostassoid.Nerve.Core
{
	using System.Collections.Generic;
	using Pipeline;
	using Signal;
	using Tools;

	public class Cell : ICell
	{
		private object _sync = new object();

		public string Name { get; private set; }
		readonly ISet<Synapse> _links = new HashSet<Synapse>();

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
			Fire(new Signal<T>(body, new StackTrace(this)) as ISignal);
		}

		public void Fire(ISignal signal)
		{
			_links.ForEach(l => l.Process(signal));
		}

		public IPipelineStep OnStream()
		{
			return new Synapse(this).Pipeline;
		}

		internal void Attach(Synapse synapse)
		{
			_links.Add(synapse);
		}

		internal void Detach(Synapse synapse)
		{
			_links.Remove(synapse);
		}

		public IEmitterOf<T> GetEmitterOf<T>() where T : class
		{
			return new EmitterOf<T>(this);
		}

		public void Handle(ISignal signal)
		{
			signal.Trace(this);
			Fire(signal);
		}

		public override string ToString()
		{
			return string.Format("Cell [{0}]", Name ?? "unnamed");
		}
	}
}