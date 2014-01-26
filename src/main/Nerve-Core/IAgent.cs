namespace Kostassoid.Nerve.Core
{
	using System;

	public interface IAgent : IProducer, IConsumer, IDisposable
	{
	}
}