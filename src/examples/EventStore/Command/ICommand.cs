namespace EventStore.Command
{
	using System;

	public interface ICommand
	{
		Guid Id { get; }
	}
}