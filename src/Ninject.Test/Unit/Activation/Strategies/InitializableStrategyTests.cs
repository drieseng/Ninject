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
    public class InitializableStrategyTests
    {
        private Mock<IContext> _contextMock;
        private InitializableStrategy _strategy;

        public InitializableStrategyTests()
        {
            _contextMock = new Mock<IContext>(MockBehavior.Strict);

            _strategy = new InitializableStrategy();
        }

        [Fact]
        public void Initialize_NotTransparantProxy_Initializable()
        {
            var initializableMock = new Mock<IInitializable>(MockBehavior.Strict);
            var instance = initializableMock.Object;

            initializableMock.Setup(p => p.Initialize());

            _strategy.Initialize(_contextMock.Object, instance);

            Assert.Same(initializableMock.Object, instance);
            initializableMock.Verify(p => p.Initialize(), Times.Once);
        }

        [Fact]
        public void Initialize_NotTransparantProxy_NotInitializable()
        {
            var nonInitializableMock = new Mock<IWarrior>(MockBehavior.Strict);
            var reference = new InstanceReference { Instance = nonInitializableMock.Object };

            _strategy.Initialize(_contextMock.Object, reference);

            Assert.Same(nonInitializableMock.Object, reference.Instance);
        }

        [Fact]
        public void Initialize_InstanceIsNull()
        {
            var reference = new InstanceReference();

            _strategy.Initialize(_contextMock.Object, reference);

            Assert.Null(reference.Instance);
        }

#if !NO_REMOTING
        [Fact]
        public void Initialize_TransparentProxy_Initializable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Initializable));

                var initializable = client.GetService<Initializable>();
                var reference = new InstanceReference { Instance = initializable };

                _strategy.Initialize(_contextMock.Object, reference);

                Assert.Equal(1, initializable.InitializeCount);
                Assert.Same(initializable, reference.Instance);
            }
        }

        [Fact]
        public void Initialize_TransparentProxy_NotInitializable()
        {
            using (var server = new RemotingServer())
            using (var client = new RemotingClient())
            {
                server.RegisterActivatedService(typeof(Monk));

                var notInitializable = client.GetService<Monk>();
                var reference = new InstanceReference { Instance = notInitializable };

                _strategy.Initialize(_contextMock.Object, reference);

                Assert.Same(notInitializable, reference.Instance);
            }
        }
#endif // !NO_REMOTING
    }
}