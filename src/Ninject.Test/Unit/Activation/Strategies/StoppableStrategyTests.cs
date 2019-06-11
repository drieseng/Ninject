using Moq;
using System;
using Xunit;

using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Tests.Fakes;
#if !NO_REMOTING
using Ninject.Tests.Infrastructure;
#endif // !NO_REMOTING

namespace Ninject.Tests.Unit.Activation.Strategies
{
    public class StoppableStrategyTests
    {
        private Mock<IContext> _contextMock;
        private StoppableStrategy _strategy;

        public StoppableStrategyTests()
        {
            _contextMock = new Mock<IContext>(MockBehavior.Strict);
            _strategy = new StoppableStrategy();
        }

        [Fact]
        public void Deactivate_NotTransparantProxy_Stoppable()
        {
            var stoppableMock = new Mock<IStoppable>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = stoppableMock.Object };

            stoppableMock.Setup(p => p.Stop());

            _strategy.Deactivate(_contextMock.Object, reference);

            Assert.Same(stoppableMock.Object, reference.Instance);
            stoppableMock.Verify(p => p.Stop(), Times.Once);
        }

        [Fact]
        public void Deactivate_NotTransparantProxy_NotStoppable()
        {
            var nonStoppableMock = new Mock<IWarrior>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = nonStoppableMock.Object };

            _strategy.Deactivate(_contextMock.Object, reference);

            Assert.Same(nonStoppableMock.Object, reference.Instance);
        }

        [Fact]
        public void Dactivate_InstanceIsNull()
        {
            var reference = new InstanceReference();

            _strategy.Deactivate(_contextMock.Object, reference);

            Assert.Null(reference.Instance);
        }

        [Fact]
        public void Deactivate_ReferenceIsNull()
        {
            const InstanceReference reference = null;

            Assert.Throws<NullReferenceException>(() => _strategy.Deactivate(_contextMock.Object, reference));
        }

#if !NO_REMOTING
        [Fact]
        public void Deactivate_TransparentProxy_Stoppable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Stoppable));

                var stoppable = client.GetService<Stoppable>();
                var reference = new InstanceReference { Instance = stoppable };

                _strategy.Deactivate(_contextMock.Object, reference);

                Assert.Equal(1, stoppable.StopCount);
                Assert.Same(stoppable, reference.Instance);
            }
        }

        [Fact]
        public void Deactivate_TransparentProxy_NotStoppable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Monk));

                var notStoppable = client.GetService<Monk>();
                var reference = new InstanceReference { Instance = notStoppable };

                _strategy.Deactivate(_contextMock.Object, reference);

                Assert.Same(notStoppable, reference.Instance);
            }
        }
#endif // !NO_REMOTING
    }
}