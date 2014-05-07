namespace Kostassoid.Nerve.Lab.Integration.Serialization
{
	public sealed class PayloadWrapperOf<T> : IPayloadWrapper
	{
		object IPayloadWrapper.Value { get { return Value; }}

		public T Value { get; private set; }

		public PayloadWrapperOf(T value)
		{
			Value = value;
		}
	}
}