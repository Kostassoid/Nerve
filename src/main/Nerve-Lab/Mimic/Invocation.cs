namespace Kostassoid.Nerve.Lab.Mimic
{
	using System.Collections.Generic;

	public class Invocation
	{
		public string MethodName { get; private set; }
		public IList<InvocationParam> Params { get; private set; }

		public Invocation(string methodName, IList<InvocationParam> @params)
		{
			MethodName = methodName;
			Params = @params;
		}
	}
}