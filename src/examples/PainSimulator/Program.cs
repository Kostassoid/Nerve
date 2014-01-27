namespace PainSimulator
{
	using System;
	using System.Threading;
	using Kostassoid.Nerve.Core;
	using Kostassoid.Nerve.Core.Pipeline;

	class Program
	{
		class Pain { }
		class Scream { }
		class Morphine { }

		static void Main(string[] args)
		{
			ICell tumor = new Cell("Tumor");
			ICell brain = new Cell("Brain");
			ICell nurse = new Cell("Nurse");

			Int32 tumorTimeout = 0;

			tumor.OnStream().Of<Pain>().ReactWith(brain);
			brain.OnStream().Of<Pain>().Gate(3, 5000).ReactWith(_ =>
			{
				Console.WriteLine("Suffering.");
				nurse.Fire(new Scream());
			});
			nurse.OnStream().Of<Scream>().Gate(3, 5000).ReactWith(_ =>
			{
				Console.WriteLine("Administering morphine.");
				tumor.Fire(new Morphine());
			});
			tumor.OnStream().Of<Morphine>().ReactWith(_ =>
			{
				Console.WriteLine("Much better.");
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
			brain.Dispose();
			tumor.Dispose();
		}
	}
}
