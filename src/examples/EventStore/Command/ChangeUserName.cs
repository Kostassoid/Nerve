namespace EventStore.Command
{
	using System;

	public class ChangeUserName : ICommand
	{
		public Guid Id { get; private set; }
		public string NewName { get; private set; }

		public ChangeUserName(Guid id, string newName)
		{
			Id = id;
			NewName = newName;
		}
	}
}