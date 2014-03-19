namespace EventStore.Model
{
	public class UserCreated : DomainEvent
	{
		public string Name { get; set; }
		public int Age { get; set; }

		public UserCreated()
		{}

		public UserCreated(AggregateRoot root, string name, int age) : base(root)
		{
			Name = name;
			Age = age;
		}
	}

}