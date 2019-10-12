using Moq;
using Ninject.Components;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using System;
using System.Threading;
using Xunit;

namespace Ninject.Tests.Unit.Modules
{
    public class NinjectModuleTests
    {
        protected Mock<Builder.IKernelConfiguration> KernelConfigurationMock { get; }
        protected Mock<INewBindingRoot> BindingRootMock { get; }
        protected Mock<INinjectSettings> SettingsMock { get; }
        protected Mock<IComponentContainer> ComponentsMock { get; }
        protected Mock<IBinding> BindingMock { get; }
        protected MyNinjectModule NinjectModule { get; }

        public NinjectModuleTests()
        {
            KernelConfigurationMock = new Mock<Builder.IKernelConfiguration>(MockBehavior.Strict);
            BindingRootMock = new Mock<INewBindingRoot>(MockBehavior.Strict);
            BindingMock = new Mock<IBinding>(MockBehavior.Strict);

            NinjectModule = new MyNinjectModule();
        }

        public class WhenBindOfTIsCalledBeforeOnLoad : NinjectModuleTests
        {
            [Fact]
            public void InvalidOperationExceptionShouldBeThrown()
            {
                Assert.Throws<InvalidOperationException>(() => NinjectModule.Bind<string>());
            }
        }

        public class WhenBindOfTIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenBindOfTIsCalledAfterOnLoad()
            {
                ((INinjectModule) NinjectModule).OnLoad(KernelConfigurationMock.Object);
            }

            [Fact]
            public void CallShouldBeDelegatedToKernelConfiguration()
            {
                KernelConfigurationMock.Setup(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()))
                                       .Callback((Action<INewBindingRoot> action) => action(BindingRootMock.Object));

                NinjectModule.Bind<string>();

                KernelConfigurationMock.Verify(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()), Times.Once());
                BindingRootMock.Verify(p => p.Bind<string>(), Times.Once);
            }
        }

        public class WhenOnLoadIsCalled : NinjectModuleTests
        {
            [Fact]
            public void ArgumentNullExceptionShouldBeThrownWhenKernelConfigurationIsNull()
            {
                const Builder.IKernelConfiguration kernelConfiguration = null;

                var actual = Assert.Throws<ArgumentNullException>(() => ((INinjectModule) NinjectModule).OnLoad(kernelConfiguration));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(kernelConfiguration), actual.ParamName);
            }

            [Fact]
            public void XernelConfigurationShouldBeAssigned()
            {
                ((INinjectModule) NinjectModule).OnLoad(KernelConfigurationMock.Object);

                Assert.NotNull(NinjectModule.KernelConfiguration);
                Assert.Same(KernelConfigurationMock.Object, NinjectModule.KernelConfiguration);
            }

            [Fact]
            public void LoadShouldBeCalled()
            {
                ((INinjectModule) NinjectModule).OnLoad(KernelConfigurationMock.Object);

                Assert.Equal(1, NinjectModule.LoadCount);
            }
        }

        public class WhenUnbindIsCalledBeforeOnLoad : NinjectModuleTests
        {
            [Fact]
            public void InvalidOperationExceptionShouldBeThrown()
            {
                Assert.Throws<InvalidOperationException>(() => NinjectModule.Unbind<string>());
            }
        }

        public class WhenUnbindIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenUnbindIsCalledAfterOnLoad()
            {
                ((INinjectModule) NinjectModule).OnLoad(KernelConfigurationMock.Object);
            }

            [Fact]
            public void CallShouldBeDelegatedToKernelConfiguration()
            {
                KernelConfigurationMock.Setup(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()))
                                       .Callback((Action<INewBindingRoot> action) => action(BindingRootMock.Object));
                BindingRootMock.Setup(p => p.Unbind<string>());

                NinjectModule.Unbind<string>();

                BindingRootMock.Verify(p => p.Unbind<string>(), Times.Once);
            }
        }

        public class MyNinjectModule : NinjectModule
        {
            private Builder.IKernelConfiguration _kernelConfiguration;
            private int _loadCount;

            public int LoadCount
            {
                get { return _loadCount; }
            }

            public int UnloadCount
            {
                get { return _loadCount; }
            }

            public Builder.IKernelConfiguration KernelConfiguration
            {
                get { return _kernelConfiguration; }
            }

            protected override void OnLoad(Builder.IKernelConfiguration kernelConfiguration)
            {
                _kernelConfiguration = kernelConfiguration;
                Interlocked.Increment(ref _loadCount);
            }

            public new INewBindingToSyntax<T> Bind<T>()
            {
                return base.Bind<T>();
            }
            public new void Unbind<T>()
            {
                base.Unbind<T>();
            }
        }
    }
}
