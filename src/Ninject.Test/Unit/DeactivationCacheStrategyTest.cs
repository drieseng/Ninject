namespace Ninject.Tests.Unit
{
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using System;
    using Xunit;

    public class DeactivationCacheStrategyTest
    {
        private DeactivationCacheStrategy testee;
        private Mock<IActivationCache> activationCacheMock;

        public DeactivationCacheStrategyTest()
        {
            this.activationCacheMock = new Mock<IActivationCache>(MockBehavior.Strict);
            this.testee = new DeactivationCacheStrategy(this.activationCacheMock.Object);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenActivationCacheIsNull()
        {
            const IActivationCache activationCache = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => new DeactivationCacheStrategy(activationCache));
            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(activationCache), actualException.ParamName);
        }

        [Fact]
        public void InstanceDeactivationsAreCachedAtDeactivation()
        {
            var instance = new object();
            var contextMock = new Mock<IContext>();

            this.activationCacheMock.Setup(a => a.AddDeactivatedInstance(instance));

            this.testee.Deactivate(contextMock.Object, new InstanceReference { Instance = instance });

            this.activationCacheMock.Verify(a => a.AddActivatedInstance(instance), Times.Once);
        }
    }
}