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

namespace Kostassoid.Nerve.Core
{
	using System.Collections.Generic;
	using Linking;
	using Signal;
	using Tools;
	using Tools.CodeContracts;

	public abstract class Cell : ICell
	{
		readonly ISet<ILink> _links = new HashSet<ILink>();

		public string Name { get; private set; }
		public NerveCenter Owner { get; private set; }

		protected Cell(string name = null, NerveCenter owner = null)
		{
			Name = name;
			Owner = owner;
		}

		protected Cell()
		{
		}

		public void Dispose()
		{
			//var linksSnapshot = _links.ToArray();
			_links.Clear();
			//linksSnapshot.ForEach(l => l.Dispose());
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

		public ILinkContinuation OnStream()
		{
			return new Link(this).Root;
		}

		//TODO: possible race condition?
		public IDisposable Attach(ILink link)
		{
			Requires.NotNull(link, "link");

			_links.Add(link);
			return new DisposableAction(() => Detach(link));
		}

		public void Detach(ILink link)
		{
			Requires.NotNull(link, "link");

			_links.Remove(link);
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

		public virtual bool OnFailure(SignalHandlingException exception)
		{
			if (Owner != null)
				Owner.OnFailure(this, exception);

			return true;
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}]", GetType().Name, Name ?? "unnamed");
		}
	}
}