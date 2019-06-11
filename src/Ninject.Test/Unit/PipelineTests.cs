namespace Ninject.Tests.Unit.PipelineTests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;    
    using Ninject.Activation.Strategies;
    using Ninject.Infrastructure.Language;
    using Xunit;

    public class PipelineContext
    {
        protected readonly List<Mock<IInitializationStrategy>> InitializationStrategyMocks;
        protected readonly List<Mock<IActivationStrategy>> ActivationStrategyMocks;
        protected readonly List<Mock<IDeactivationStrategy>> DeactivationStrategyMocks;
        protected readonly Mock<IActivationCache> ActivationCacheMock;
        protected readonly Pipeline Pipeline;

        public PipelineContext()
        {
            this.InitializationStrategyMocks = new List<Mock<IInitializationStrategy>>
                {
                    new Mock<IInitializationStrategy>(),
                    new Mock<IInitializationStrategy>(),
                    new Mock<IInitializationStrategy>()
                };
            this.ActivationStrategyMocks = new List<Mock<IActivationStrategy>>
                {
                    new Mock<IActivationStrategy>(),
                    new Mock<IActivationStrategy>(),
                    new Mock<IActivationStrategy>()
                };
            this.DeactivationStrategyMocks = new List<Mock<IDeactivationStrategy>>
                {
                    new Mock<IDeactivationStrategy>(),
                    new Mock<IDeactivationStrategy>(),
                    new Mock<IDeactivationStrategy>()
                };
            this.ActivationCacheMock = new Mock<IActivationCache>();
            this.Pipeline = new Pipeline(this.InitializationStrategyMocks.Select(mock => mock.Object),
                                         this.ActivationStrategyMocks.Select(mock => mock.Object),
                                         this.DeactivationStrategyMocks.Select(mock => mock.Object),
                                         this.ActivationCacheMock.Object);
        }
    }

    public class WhenPipelineIsCreated : PipelineContext
    {
        [Fact]
        public void HasListOfStrategies()
        {
            this.Pipeline.Strategies.Should().NotBeNull();

            for (int idx = 0; idx < this.ActivationStrategyMocks.Count; idx++)
            {
                this.Pipeline.Strategies[idx].Should().BeSameAs(this.ActivationStrategyMocks[idx].Object);
            }
        }
    }

    public class WhenActivateIsCalled : PipelineContext
    {
        [Fact]
        public void CallsActivateOnStrategies()
        {
            var contextMock = new Mock<IContext>();
            var reference = new InstanceReference();

            this.Pipeline.Activate(contextMock.Object, reference);

            this.ActivationStrategyMocks.Map(mock => mock.Verify(x => x.Activate(contextMock.Object, reference)));
        }

        [Fact]
        public void WhenAlreadyActivatedNothingHappens()
        {
            var contextMock = new Mock<IContext>();
            var reference = new InstanceReference();
            this.ActivationCacheMock.Setup(activationCache => activationCache.IsActivated(It.IsAny<object>())).Returns(true);

            this.Pipeline.Activate(contextMock.Object, reference);

            this.ActivationStrategyMocks.Map(mock => mock.Verify(x => x.Activate(contextMock.Object, reference), Times.Never));
        }
    }

    public class WhenDeactivateIsCalled : PipelineContext
    {
        [Fact]
        public void CallsDeactivateOnStrategies()
        {
            var contextMock = new Mock<IContext>();
            var reference = new InstanceReference();

            this.Pipeline.Deactivate(contextMock.Object, reference);

            this.DeactivationStrategyMocks.Map(mock => mock.Verify(x => x.Deactivate(contextMock.Object, reference)));
        }

        [Fact]
        public void WhenAlreadyDeactivatedNothingHappens()
        {
            var contextMock = new Mock<IContext>();
            var reference = new InstanceReference();
            this.ActivationCacheMock.Setup(activationCache => activationCache.IsDeactivated(It.IsAny<object>())).Returns(true);

            this.Pipeline.Deactivate(contextMock.Object, reference);

            this.DeactivationStrategyMocks.Map(mock => mock.Verify(x => x.Deactivate(contextMock.Object, reference), Times.Never));
        }
    }
}