namespace Kostassoid.Nerve.Core
{
    using System;

    public interface IAgentFactory : IDisposable
    {
        IAgent Build();
    }

    public class InProcAgentFactory : IAgentFactory
    {
        public void Dispose()
        {
        }

        public IAgent Build()
        {
            return new InProcAgent();
        }
    }
}