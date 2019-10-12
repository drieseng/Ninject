namespace Ninject.Tests.Unit
{
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Xunit;

    public class ActivationCacheStrategyTest
    {
        private ActivationCacheStrategy testee;

        private Mock<IActivationCache> activationCacheMock;

        public ActivationCacheStrategyTest()
        {
            this.activationCacheMock = new Mock<IActivationCache>();
            this.testee = new ActivationCacheStrategy(this.activationCacheMock.Object);
        }

        [Fact]
        public void InstanceActivationsAreCachedAtActivation()
        {
            var instance  = new object();
            var contextMock = new Mock<IContext>();
            
            this.testee.Activate(contextMock.Object, new InstanceReference { Instance = instance });

            this.activationCacheMock.Verify(activationCache => activationCache.AddActivatedInstance(instance));
        }
    }
}