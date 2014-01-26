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
			IAgent tumor = new Agent("Tumor");
			IAgent brain = new Agent("Brain");
			IAgent nurse = new Agent("Nurse");

			tumor.OnStream().Of<Pain>().ReactWith(brain);
			brain.OnStream().Of<Pain>()/*.Threshold()*/.ReactWith(_ =>
			{
				Console.WriteLine("Suffering.");
				nurse.Dispatch(new Scream());
			});
			nurse.OnStream().Of<Scream>().ReactWith(_ =>
			{
				Console.WriteLine("Administering morphine.");
				tumor.Dispatch(new Morphine());
			});
			tumor.OnStream().Of<Morphine>().ReactWith(_ =>
			{
				Console.WriteLine("Much better.");
			});

			var timer = new Timer(_ => tumor.Dispatch(new Pain()));
			timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));

			Console.ReadKey(false);

			timer.Dispose();
			nurse.Dispose();
			brain.Dispose();
			tumor.Dispose();
		}
	}
}
