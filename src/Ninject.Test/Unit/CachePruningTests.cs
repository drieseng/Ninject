namespace Ninject.Tests.Unit.CacheTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class WhenPruneIsCalled
    {
        private Mock<ICachePruner> cachePrunerMock;
        private Mock<IBindingConfiguration> bindingConfigurationMock;
        private Cache cache;

        public WhenPruneIsCalled()
        {
            this.cachePrunerMock = new Mock<ICachePruner>();
            this.bindingConfigurationMock = new Mock<IBindingConfiguration>();
            this.cache = new Cache(new PipelineMock(), this.cachePrunerMock.Object);
        }

        [Fact]
        public void CollectedScopeInstancesAreRemoved()
        {
            // Use separate method to allow scope to be finalized
            var swordWeakReference = Remember();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            this.cache.Prune();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            bool swordCollected = !swordWeakReference.IsAlive;
            swordCollected.Should().BeTrue();
        }

        [Fact]
        public void UncollectedScopeInstancesAreNotRemoved()
        {
            // Use separate method to allow scope to be finalized
            var swordWeakReference = Remember();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            bool swordCollected = !swordWeakReference.IsAlive;
            swordCollected.Should().BeFalse();
        }

        private static IContext CreateContextMock(object scope, IBindingConfiguration bindingConfiguration, params Type[] genericArguments)
        {
            var bindingMock = new Mock<IBinding>(MockBehavior.Strict);
            bindingMock.Setup(b => b.BindingConfiguration).Returns(bindingConfiguration);
            return new ContextMock(scope, bindingMock.Object, genericArguments);
        }

        private WeakReference Remember()
        {
            var sword = new Sword();
            var swordWeakReference = new WeakReference(sword);
            var scope = new TestObject(42);
            var context = CreateContextMock(scope, this.bindingConfigurationMock.Object);
            this.Remember(sword, scope, context);
            return swordWeakReference;
        }

        private void Remember(Sword sword, object scope, IContext context)
        {
            this.cache.Remember(context, scope, new InstanceReference { Instance = sword });
        }
    }

    public class PipelineMock : IPipeline
    {
        public void Dispose()
        {
        }

        public INinjectSettings Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the instance using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// The initialized instance.
        /// </returns>
        public object Initialize(IContext context, object instance)
        {
            return instance;
        }

        public void Activate(IContext context, InstanceReference reference)
        {
        }

        public void Deactivate(IContext context, InstanceReference reference)
        {
        }

        public IReadOnlyList<IActivationStrategy> Strategies
        {
            get;
            set;
        }
    }

    public class ContextMock : IContext
    {
        private WeakReference scope;
        public ContextMock(object scope, IBinding binding, Type[] genericArguments)
        {
            this.scope = new WeakReference(scope);
            this.Binding = binding;
            this.GenericArguments = genericArguments;
        }

        public object GetScope()
        {
            return this.scope.Target;
        }

        public object Resolve()
        {
            throw new NotImplementedException();
        }

        public void BuildPlan(Type type)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyKernel Kernel { get; set; }

        public IRequest Request { get; set; }

        public IBinding Binding { get; private set; }

        public IPlan Plan { get; set; }

        public ICache Cache { get; private set; }

        public IReadOnlyList<IParameter> Parameters { get; set; }

        public IProvider Provider => throw new NotImplementedException();

        public Type[] GenericArguments
        {
            get;
            private set;
        }

        public bool HasInferredGenericArguments
        {
            get
            {
                return this.GenericArguments != null;
            }
        }
    }
}