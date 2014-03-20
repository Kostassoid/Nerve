namespace EventStore.Command
{
	using System;

	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Linking.Operators;
	using Kostassoid.Nerve.Core.Scheduling;

	using Model;

	public class CommandHandler : Cell
	{
		readonly EventStore _store;

		public CommandHandler(EventStore store) : base("CommandHandler", ThreadScheduler.Factory)
		{
			_store = store;

			OnStream().Of<CreateUser>().ReactWith(CreateUserHandler);
			OnStream().Of<ChangeUserName>().ReactWith(ChangeUserNameHandler);
		}

		void CreateUserHandler(ISignal<CreateUser> signal)
		{
			Console.WriteLine("Creating user {0}.", signal.Payload.Name);

			var user = User.Create(signal.Payload.Id, signal.Payload.Name, signal.Payload.Age);
			user.Commit().Wait();
		}

		void ChangeUserNameHandler(ISignal<ChangeUserName> signal)
		{
			Console.WriteLine("Changing user name to {0}.", signal.Payload.NewName);

			var user = _store.Load<User>(signal.Payload.Id);
			user.ChangeName(signal.Payload.NewName);
			user.Commit().Wait();
		}

		public override bool OnFailure(SignalException exception)
		{
			Console.WriteLine("Cought exception of type {0}", exception.InnerException.GetType());
			return true;
		}
	}
}