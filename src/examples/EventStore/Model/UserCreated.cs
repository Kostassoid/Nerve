namespace EventStore.Model
{
	public class UserCreated : DomainEvent
	{
		public string Name { get; private set; }
		public int Age { get; private set; }

		protected UserCreated()
		{}

		public UserCreated(AggregateRoot root, string name, int age) : base(root)
		{
			Name = name;
			Age = age;
		}
	}

}