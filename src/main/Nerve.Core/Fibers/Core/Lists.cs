namespace Kostassoid.Nerve.Core.Fibers.Core
{
	using System;
	using System.Collections.Generic;

	internal static class Lists
    {
        public static void Swap(ref List<Action> a, ref List<Action> b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }
    }
}