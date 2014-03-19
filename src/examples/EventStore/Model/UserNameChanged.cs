namespace EventStore.Model
{
	public class UserNameChanged : DomainEvent
	{
		public string NewName { get; private set; }

		protected UserNameChanged()
		{}

		public UserNameChanged(AggregateRoot root, string newName) : base(root)
		{
			NewName = newName;
		}
	}

}