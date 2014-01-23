namespace Kostassoid.Nerve.Core
{
    using System;

    public class Expect : Hub
    {
        public static Expect When<T>(Action<T> handler)
        {
            var expect = new Expect();
            return expect.ElseWhen(handler);
        }

        protected Expect()
        {
        }

        public Expect ElseWhen<T>(Action<T> handler)
        {
            On<T>().Subscribe(ctx => handler(ctx.Message));
            return this;
        }
    }
}