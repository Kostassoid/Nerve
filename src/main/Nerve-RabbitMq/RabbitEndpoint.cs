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

using System;
using Kostassoid.Nerve.Core;
using Kostassoid.Nerve.RabbitMq.Configuration;

namespace Kostassoid.Nerve.RabbitMq
{
    public class RabbitEndpoint : IDisposable
    {
	    public string Name { get; private set; }

	    public RabbitEndpoint(string name)
	    {
		    Name = name;
	    }

	    public void Start(Action<IRabbitEndpointConfigurator> configurator)
	    {
		    
	    }

		public ICell BuildCell(Action<IRabbitCellConfigurator> action)
	    {
			throw new NotImplementedException();
	    }

	    public void Dispose()
	    {
		    
	    }
    }
}
