namespace Kostassoid.Nerve.Lab.Mimic
{
	public class InvocationParam
	{
		public object Value { get; private set; }

		public InvocationParam(object value)
		{
			Value = value;
		}
	}
}