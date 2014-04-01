namespace Kostassoid.Nerve.Lab.Mimic
{
	using System.Collections.Generic;
	using System.Linq;

	public class Invocation
	{
		public IList<InvocationParam> Params { get; private set; }

		public Invocation(IEnumerable<InvocationParam> @params)
		{
			Params = @params.ToList();
		}
	}
}