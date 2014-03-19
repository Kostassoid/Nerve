namespace EventStore.Model
{
	using System;

	public class User : AggregateRoot
	{
		public string Name { get; private set; }
		public int Age { get; private set; }

		protected User()
		{ }

		protected User(Guid id)
			: base(id)
		{ }

		public static User Create(string name, int age)
		{
			var user = new User(Guid.NewGuid());
			user.Apply(new UserCreated(user, name, age));
			return user;
		}

		protected void OnUserCreated(UserCreated ev)
		{
			Id = ev.Id;
			Name = ev.Name;
			Age = ev.Age;
		}

		public void ChangeName(string newName)
		{
			if (string.IsNullOrWhiteSpace(newName))
			{
				throw new ArgumentException("Name shouldn't be empty.", "newName");
			}

			Apply(new UserNameChanged(this, newName));
		}

		protected void OnUserNameChanged(UserNameChanged ev)
		{
			Name = ev.NewName;
		}

	}
}