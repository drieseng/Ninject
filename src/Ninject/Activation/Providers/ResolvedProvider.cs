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

        /// <summary>
        /// Gets a value indicating whether the provider uses Ninject to resolve services when creating an instance.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the provider uses Ninject to resolve service when creating an instance; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool ResolvesServices => true;

        public Type Type { get; }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isInitialized"><see langword="true"/> if the created instance is fully initialized; otherwise, <see langword="false"/></param>
        /// <returns>
        /// The created instance.
        /// </returns>
        public object Create(IContext context, out bool isInitialized)
        {
            var instance = context.Kernel.Get(this.providerType);
            if (instance is IProvider provider)
            {
                return provider.Create(context, out isInitialized);
            }

            throw new ActivationException("TODO");
        }
    }
}
