namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;

	public class Invocation
	{
		public string Method { get; private set; }
		public object[] Params { get; private set; }
		public Type Expects { get; private set; }

		public Invocation(string method, Type expects, params object[] @params)
		{
			Method = method;
			Expects = expects;
			Params = @params;
		}
	}
}