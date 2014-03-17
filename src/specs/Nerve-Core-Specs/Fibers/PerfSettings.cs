namespace Kostassoid.Nerve.Core.Specs.Fibers
{
	using System.Runtime.InteropServices;

	public struct TIMECAPS
    {
        public uint PeriodMin;
        public uint PeriodMax;
    }

    public static class PerfSettings
    {
        [DllImport("winmm.dll")]
        public static extern uint timeBeginPeriod(uint period);

        [DllImport("winmm.dll")]
        public static extern uint timeEndPeriod(uint period);

        [DllImport("winmm.dll")]
        public static extern int timeGetDevCaps(ref TIMECAPS lpTimeCaps, int uSize);
    }
}