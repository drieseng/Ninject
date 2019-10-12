using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Xunit;

namespace Ninject.Tests.Unit.StoppableStrategyTests
{
    using FluentAssertions;

    public class StoppableStrategyContext
    {
        protected readonly StoppableStrategy strategy;
        protected readonly Mock<IContext> contextMock;

        public StoppableStrategyContext()
        {
            this.contextMock = new Mock<IContext>();
            this.strategy = new StoppableStrategy();
        }
    }

    public class WhenDeactivateIsCalled : StoppableStrategyContext
    {
        [Fact]
        public void StrategyStopsInstanceIfItIsStoppable()
        {
            var instance = new StoppableObject();
            var reference = new InstanceReference { Instance = instance };

            this.strategy.Deactivate(this.contextMock.Object, reference);

            instance.WasStopped.Should().BeTrue();
        }

        [Fact]
        public void StrategyDoesNotAttemptToStopInstanceIfItIsNotStoppable()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            this.strategy.Deactivate(this.contextMock.Object, reference);
        }
    }

    public class StoppableObject : IStoppable
    {
        public bool WasStopped { get; set; }

        public void Stop()
        {
            this.WasStopped = true;
        }
    }
}