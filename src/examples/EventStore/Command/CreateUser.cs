namespace EventStore.Command
{
	using System;

	public class CreateUser : ICommand
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public int Age { get; private set; }

		public CreateUser(Guid id, string name, int age)
		{
			Id = id;
			Name = name;
			Age = age;
		}
	}
}