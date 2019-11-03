namespace Ninject.Tests.Integration
{
    using System;
    using FluentAssertions;
    using Ninject.Activation;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ProviderTests : IDisposable
    {
        private readonly IKernel kernel;

        public ProviderTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void ThrowActivationExceptionWhenActivateIfMissingToMethod()
        {
            this.kernel.Bind<IConfig>();

            Assert.Throws<ActivationException>(() => this.kernel.Get<IConfig>());
        }

        [Fact]
        public void ThrowActivationExceptionWhenNoBindingExistsForProviderType_AllowNullInjectionIsTrue()
        {
            this.kernel.Bind<IConfig>().ToProvider(typeof(ConfigProvider));

            var instance = this.kernel.Get<IConfig>();

            instance.Should().NotBeNull();
        }

        [Fact]
        public void ThrowActivationExceptionWhenNoBindingExistsForProviderType_AllowNullInjectionIsFalse()
        {
            throw new Exception();
        }

        [Fact]
        public void ThrowActivationExceptionWhenServiceBoundToProviderTypeDoesNotImplementIProvider()
        {
            this.kernel.Bind<IConfig>().ToProvider(typeof(string));

            var instance = this.kernel.Get<IConfig>();

            instance.Should().NotBeNull();
        }

        [Fact]
        public void ThrowActivationExceptionWhenProviderDoesNotReturnInstanceThatIsAssignableToService()
        {
            this.kernel.Bind<IConfig>().ToProvider(typeof(StringProvider));

            var instance = this.kernel.Get<IConfig>();

            instance.Should().NotBeNull();
        }

        [Fact]
        public void ThrowActivationExceptionWhenProviderReturnsNull_AllowNullInjectionIsFalse()
        {
            this.kernel.Bind<IConfig>().ToProvider(typeof(StringProvider));

            var instance = this.kernel.Get<IConfig>();

            instance.Should().NotBeNull();
        }

        [Fact]
        public void ThrowActivationExceptionWhenProviderReturnsNullAndAllowNullInjectionIsFalse()
        {
            this.kernel.Bind<IConfig>().ToProvider(typeof(StringProvider));

            var instance = this.kernel.Get<IConfig>();

            instance.Should().NotBeNull();
        }

        [Fact]
        public void ShouldReturnNullWhenServiceIsReferenceTypeAndProviderReturnsNullAndAllowNullInjectionIsTrue()
        {
            this.kernel.Bind<NullProvider<Sword>>().ToSelf();
            this.kernel.Bind<Sword>().ToProvider(typeof(NullProvider<Sword>));

            var instance = this.kernel.Get<Sword>();

            instance.Should().BeNull();
        }

        [Fact]
        public void ShouldThrowNullReferenceExceptionWhenServiceIsValueTypeAndProviderReturnsNullAndAllowNullInjectionIsTrue()
        {
            this.kernel.Bind<NullProvider<int>>().ToSelf();
            this.kernel.Bind<int>().ToProvider(typeof(NullProvider<int>));

            Assert.Throws<NullReferenceException>(() => this.kernel.Get<int>());
        }

        [Fact]
        public void InstancesCanBeCreated()
        {
            this.kernel.Bind<IConfig>().ToProvider<ConfigProvider>();

            var instance = this.kernel.Get<IConfig>();

            instance.Should().NotBeNull();
        }

        private class ConfigProvider : IProvider
        {
            public Type Type
            {
                get
                {
                    return typeof(DynamicConfigReader);
                }
            }

            public bool ResolvesServices => false;

            public object Create(IContext context)
            {
                return new DynamicConfigReader("test");
            }

            public object Create(IContext context, out bool isInitialized)
            {
                isInitialized = true;
                return new DynamicConfigReader("test");
            }
        }

        private class StringProvider : IProvider
        {
            public Type Type
            {
                get
                {
                    return typeof(string);
                }
            }

            public bool ResolvesServices => false;


            public object Create(IContext context, out bool isInitialized)
            {
                isInitialized = true;
                return "test";
            }
        }

        public class NullProvider<T> : IProvider
        {
            public bool ResolvesServices => false;

            public Type Type => typeof(T);

            public object Create(IContext context, out bool isInitialized)
            {
                isInitialized = false;
                return null;
            }
        }


        public interface IConfig
        {
            string Get();
        }

        public class DynamicConfigReader : IConfig
        {
            private readonly string name;

            public DynamicConfigReader(string name)
            {
                this.name = name;
            }

            public string Get()
            {
                return this.name;
            }
        }
    }
}