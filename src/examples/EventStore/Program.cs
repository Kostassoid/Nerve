namespace EventStore
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Command;

	using Kostassoid.Nerve.Core.Linking.Operators;

	using Model;

	using Storage;

	class Program
	{
		static void Main(string[] args)
		{
			var store = new EventStore(new JsonFileEventStorage("Store"));
			var commandHandler = new CommandHandler(store);

			EventBus.OnStream().Of<UncommitedEventStream>().ReactWith(store);
			EventBus.OnStream().Of<ICommand>().ReactWith(commandHandler);

			var finnId = Guid.NewGuid();
			var jakeId = Guid.NewGuid();

			commandHandler.Send(new CreateUser(finnId, "Finn", 13));
			commandHandler.Send(new CreateUser(jakeId, "Jake", 33));
			commandHandler.Send(new ChangeUserName(jakeId, "SuperJake"));
			commandHandler.Send(new ChangeUserName(finnId, "SuperFinn"));

			Console.WriteLine("Press any key to read from store...");
			Console.ReadKey(true);

			var jake = store.Load<User>(jakeId);
			Console.WriteLine("Jake name = {0}", jake.Name);

			var finn = store.Load<User>(finnId);
			Console.WriteLine("Finn name = {0}", finn.Name);

			Console.WriteLine("Press any key to build concurrency conflict...");
			Console.ReadKey(true);

			var tasks = Enumerable.Range(1, 10).Select(i => Task.Factory.StartNew(
				() =>
					{
						var j = store.Load<User>(jakeId);
						j.ChangeName("Jake " + i);
						j.Commit().ContinueWith(
							t =>
								{
									if (t.IsFaulted)
									{
										Console.WriteLine("Command {0} failed with: {1}.", i, t.Exception.InnerException.Message);
									}
									else
									{
										Console.WriteLine("Command {0} succeeded.", i);
									}
								}).Wait();
					})).ToArray();

			Task.WaitAll(tasks);


		}
	}
}
