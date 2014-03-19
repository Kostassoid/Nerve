namespace EventStore
{
	using System;
	using System.Threading;

	using Kostassoid.Nerve.Core.Linking.Operators;

	using Model;

	class Program
	{
		static void Main(string[] args)
		{
			var store = new FileBasedEventStore();
			//var queryService = new UserQueryService();

			EventBus.OnStream().Of<UncommitedEventStream>().ReactWith(store);

			var finn = User.Create("Finn", 13);
			var jake = User.Create("Jake", 33);

			jake.ChangeName("SuperJake");

			finn.Commit();
			jake.Commit();

			Console.WriteLine("Press any key to read from store...");
			Console.ReadKey(false);

			var user = store.Load<User>(jake.Id);
			Console.WriteLine("Found user {0} with id {1}", user.Id, user.Name);
			Console.ReadKey(false);
		}
	}
}
