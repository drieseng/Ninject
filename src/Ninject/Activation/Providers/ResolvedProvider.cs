using System;

namespace Ninject.Activation.Providers
{
    internal sealed class ResolveProviderWrapper : IProvider
    {
        private readonly Type providerType;

        public ResolveProviderWrapper(Type providerType, Type serviceType)
        {
            this.providerType = providerType;
            this.Type = serviceType;
        }

        public Type Type { get; }

        public object Create(IContext context)
        {
            var provider = (IProvider) context.Kernel.Get(this.providerType);
            return provider.Create(context);
        }
    }
}
