namespace SleepingBarber
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Linking.Operators;
	using Kostassoid.Nerve.Core.Scheduling;

	class Program
	{
		const int SeatsCount = 3;
		const int ClientsCount = 20;
		public static Random Random = new Random();

		abstract class ClientEvent
		{
			public Client Client { get; set; }
		}

		class ClientEnteredTheShop : ClientEvent
		{}

		class ClientIsReadyForHaircut : ClientEvent
		{}

		class ClientGotHaircut : ClientEvent
		{}

		class Client : Cell
		{
			public bool GotHaircut { get; private set; }

			public Client(int id)
				: base(id.ToString(), PoolScheduler.Factory)
			{
				OnStream().Of<ClientGotHaircut>().ReactWith(s => { GotHaircut = true; });
			}

		}

		class Barber : Cell
		{
			public Barber() : base(ThreadScheduler.Factory)
			{
				OnStream().Of<ClientIsReadyForHaircut>()
					.ReactWith(s =>
							   {
								   Thread.Sleep(Random.Next(10, 200));
								   var gotHaircutEvent = new ClientGotHaircut { Client = s.Payload.Client };

								   //TODO: not sure if I like this
								   s.Payload.Client.Fire(gotHaircutEvent);
								   s.Return(gotHaircutEvent);
							   });
			}

		}

		class Shop : Cell
		{
			readonly int _maxQueueSize;
			readonly Queue<Client> _queue = new Queue<Client>();

			bool _seatIsTaken;

			public Shop(int maxQueueSize) : base(ThreadScheduler.Factory)
			{
				_maxQueueSize = maxQueueSize;

				Console.WriteLine("{0} is open.", this);

				OnStream().Of<ClientEnteredTheShop>()
					.ReactWith(s =>
					{
						Console.WriteLine("{0} entered the shop.", s.Payload.Client);

						if (!_seatIsTaken)
						{
							SendToBarber(s.Payload.Client);
						}
						else
						{
							SendToQueue(s.Payload.Client);
						}
					});
				OnStream().Of<ClientGotHaircut>()
					.ReactWith(s =>
					{
						Console.WriteLine("{0} got haircut.", s.Payload.Client);
						
						_seatIsTaken = false;
						if (_queue.Count == 0)
						{
							Console.WriteLine("[Barber] is sleeping...");
							return;
						}
						var client = _queue.Dequeue();
						
						SendToBarber(client);
					});
			}

			private void SendToBarber(Client client)
			{
				_seatIsTaken = true;
				Console.WriteLine("{0} is getting a haircut.", client);
				Fire(new ClientIsReadyForHaircut { Client = client });
			}

			private void SendToQueue(Client client)
			{
				if (_queue.Count < _maxQueueSize)
				{
					Console.WriteLine("{0} is waiting in queue.", client);
					_queue.Enqueue(client);
				}
				else
				{
					Console.WriteLine("{0} is rejected.", client);
				}
			}

			protected override void Dispose(bool isDisposing)
			{
				Console.WriteLine("{0} is closed.", this);
				base.Dispose(isDisposing);
			}
		}

		static void Main(string[] args)
		{
			var barber = new Barber();
			var shop = new Shop(SeatsCount);
			var clients = Enumerable.Range(1, ClientsCount).Select(i => new Client(i)).ToList();

			shop.Attach(barber);

			foreach (var client in clients)
			{
				Thread.Sleep(Random.Next(10, 200));
				shop.Fire(new ClientEnteredTheShop { Client = client });
			}

			Thread.Sleep(1000);

			var gotHaircut = clients.Count(c => c.GotHaircut);
			Console.WriteLine("{0} out of {1} clients have got a new haircut.", gotHaircut, ClientsCount);
		}
	}
}
