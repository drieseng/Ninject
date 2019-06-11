namespace Ninject.Tests.Fakes
{
    using System;

    using Ninject.Activation;

    public class ResolveCountingProvider : IProvider
    {
        private readonly IProvider provider;

        public int Count { get; set; }

        public Type Type => provider.GetType();

        public ResolveCountingProvider(IProvider provider)
        {
            this.provider = provider;
        }

        public object Create(IContext context)
        {
            ++this.Count;
            return this.provider.Create(context);
        }
    }
}