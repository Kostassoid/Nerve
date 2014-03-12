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

	/// <summary>
	/// Base Class implementation.
	/// Relays all incoming signals through all links.
	/// </summary>
	public class Cell : ICell
	{
		public string Name { get; private set; }

		public event SignalExceptionHandler Failed = (cell, exception) => { };

        readonly ISet<ILink> _links = new HashSet<ILink>();

	    public Cell(string name = null)
		{
			Name = name;
		}

	    public Cell()
		{
		}

		public virtual void Dispose()
		{
			//var linksSnapshot = _links.ToArray();
			_links.Clear();
			//linksSnapshot.ForEach(l => l.Dispose());
		}

/*
		public IEmitterOf<T> GetEmitterOf<T>() where T : class
		{
			return new EmitterOf<T>(this);
		}
*/

        //TODO: possible race condition?
        internal IDisposable Attach(ILink link)
        {
            Requires.NotNull(link, "link");

            _links.Add(link);
            return new DisposableAction(() => Detach(link));
        }

        internal void Detach(ILink link)
        {
            Requires.NotNull(link, "link");

            _links.Remove(link);
        }

		protected void Relay(ISignal signal)
		{
			Requires.NotNull(signal, "signal");

			_links.ForEach(l => l.Process(signal));
		}

		public virtual bool OnFailure(SignalException exception)
		{
			Failed(this, exception);

			return true;
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}]", GetType().Name, Name ?? "unnamed");
		}

        public ILinkContinuation OnStream()
        {
            return new Link(this).Root;
        }

        public void Fire<T>(T body) where T : class
        {
            Requires.NotNull(body, "body");

            Handle(new Signal<T>(body, new StackTrace(this)));
        }

        public void Handle(ISignal signal)
        {
            Requires.NotNull(signal, "signal");

            signal.Trace(this);
            Relay(signal);
        }

        /*
                public void Fire(ISignal signal)
                {
                    Relay(signal);
                }
        */

	}
}