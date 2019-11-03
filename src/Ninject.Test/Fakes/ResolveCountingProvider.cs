namespace Ninject.Tests.Fakes
{
    using System;

    using Ninject.Activation;

    public class ResolveCountingProvider : IProvider
    {
        private readonly IProvider provider;

        public int Count { get; set; }

        public Type Type => provider.GetType();

        public bool ResolvesServices => false;

        public ResolveCountingProvider(IProvider provider)
        {
            this.provider = provider;
        }

        public object Create(IContext context, out bool isInitialized)
        {
            ++this.Count;
            return this.provider.Create(context, out isInitialized);
        }
    }
}