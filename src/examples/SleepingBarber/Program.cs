namespace SleepingBarber
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Processing.Operators;
	using Kostassoid.Nerve.Core.Scheduling;

	class Program
	{
		const int MaxQueueSize = 3;
		const int ClientsCount = 20;
		public static Random Random = new Random();

		class Client
		{
			readonly int _id;

			public bool GotHaircut { get; private set; }

			public Client(int id)
			{
				_id = id;
			}

			public override string ToString()
			{
				return string.Format("[Client {0}]", _id);
			}

			public void Cut()
			{
				if (GotHaircut)
				{
					throw new InvalidOperationException(ToString() + " already got a haircut!");
				}

				GotHaircut = true;
			}
		}

		class Barber : Cell
		{
			public Barber() : base(PoolScheduler.Factory)
			{
				OnStream().Of<Client>()
					.ReactWith(s =>
							   {
								   Thread.Sleep(Random.Next(10, 200));

								   s.Payload.Cut();

								   s.Return(s.Payload);
							   });
			}
		}

		class Shop : Cell
		{
			readonly Barber _barber;
			readonly int _maxQueueSize;
			readonly Queue<Client> _queue = new Queue<Client>();

			bool _seatIsTaken;

			public Shop(Barber barber, int maxQueueSize) : base(ThreadScheduler.Factory)
			{
				_barber = barber;
				_maxQueueSize = maxQueueSize;

				Console.WriteLine("{0} is open.", this);

				OnStream().Of<Client>()
					.Where(c => !c.GotHaircut)
					.ReactWith(s =>
					{
						Console.WriteLine("{0} entered the shop.", s.Payload);

						if (!_seatIsTaken)
						{
							SendToBarber(s.Payload);
						}
						else
						{
							SendToQueue(s.Payload);
						}
					});

				OnStream().Of<Client>()
					.Where(c => c.GotHaircut)
					.ReactWith(s =>
					{
						Console.WriteLine("{0} got haircut.", s.Payload);

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
				_barber.Send(client, this);
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

		// ReSharper disable UnusedParameter.Local
		static void Main(string[] args)
		// ReSharper restore UnusedParameter.Local
		{
			var barber = new Barber();
			var shop = new Shop(barber, MaxQueueSize);
			var clients = Enumerable.Range(1, ClientsCount).Select(i => new Client(i)).ToList();

			foreach (var client in clients)
			{
				Thread.Sleep(Random.Next(10, 200));
				shop.Send(client);
			}

			Thread.Sleep(1000);

			var gotHaircut = clients.Count(c => c.GotHaircut);
			Console.WriteLine("{0} out of {1} clients have got a new haircut.", gotHaircut, ClientsCount);
		}
	}
}
