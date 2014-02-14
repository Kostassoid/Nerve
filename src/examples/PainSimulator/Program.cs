namespace PainSimulator
{
	using System;
	using System.Threading;
	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Linking;

	class Program
	{
		class Pain { }
		class Scream { }
		class Morphine { }

		static void Main(string[] args)
		{
			ICell tumor = new Cell("Tumor");
			ICell patient = new Cell("Brain");
			ICell nurse = new Cell("Nurse");

			var tumorTimeout = 0;

			tumor.OnStream().Of<Pain>().ReactWith(patient);
			patient.OnStream().Of<Pain>().Gate(3, TimeSpan.FromSeconds(1)).ReactWith(_ =>
			{
				Console.WriteLine("\n[Patient]: Suffering.");
				nurse.Fire(new Scream());
			});
			nurse.OnStream().Of<Scream>().Gate(3, TimeSpan.FromSeconds(1)).ReactWith(_ =>
			{
				Console.WriteLine("[Nurse]: Administering morphine.");
				tumor.Fire(new Morphine());
			});
			tumor.OnStream().Of<Morphine>().ReactWith(_ =>
			{
				Console.WriteLine("[Tumor]: Not hurting anymore.");
				tumorTimeout += 5;
			});

			var timer = new Timer(_ =>
			{
				if (tumorTimeout > 0)
				{
					Console.Write(".");
					tumorTimeout--;
				}
				else
				{
					Console.Write("*");
					tumor.Fire(new Pain());
				}
			});
			timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(100));

			Console.ReadKey(false);

			timer.Dispose();
			nurse.Dispose();
			patient.Dispose();
			tumor.Dispose();
		}
	}
}
