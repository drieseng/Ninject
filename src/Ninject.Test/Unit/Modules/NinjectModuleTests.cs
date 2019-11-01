using Moq;
using Ninject.Builder;
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
        protected Mock<IKernelConfiguration> KernelConfigurationMock { get; }
        protected Mock<INewBindingRoot> BindingRootMock { get; }
        protected Mock<INinjectSettings> SettingsMock { get; }
        protected Mock<IComponentContainer> ComponentsMock { get; }
        protected Mock<IBinding> BindingMock { get; }
        protected MyNinjectModule NinjectModule { get; }

        public NinjectModuleTests()
        {
            KernelConfigurationMock = new Mock<IKernelConfiguration>(MockBehavior.Strict);
            BindingRootMock = new Mock<INewBindingRoot>(MockBehavior.Strict);
            BindingMock = new Mock<IBinding>(MockBehavior.Strict);

            NinjectModule = new MyNinjectModule();
        }

        public class WhenBindOfTIsCalledBeforeOnLoad : NinjectModuleTests
        {
            [Fact]
            public void InvalidOperationExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<InvalidOperationException>(() => NinjectModule.Bind<string>());
                Assert.Null(exception.InnerException);
                Assert.Equal("Bindings can only be configured after module has been loaded.", exception.Message);
            }
        }

        public class WhenBindOfTIsCalledAfterOnLoad : NinjectModuleTests
        {
            public WhenBindOfTIsCalledAfterOnLoad()
            {
                KernelConfigurationMock.Setup(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()))
                                       .Callback((Action<INewBindingRoot> action) => action(BindingRootMock.Object))
                                       .Returns(KernelConfigurationMock.Object);

                ((INinjectModule) NinjectModule).OnLoad(KernelConfigurationMock.Object);

                KernelConfigurationMock.Verify(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()), Times.Once);
            }

            [Fact]
            public void BindShouldBeDelegatedToBindingRootOfKernelConfiguration()
            {
                var bindingSyntaxMock = new Mock<INewBindingToSyntax<string>>(MockBehavior.Strict);

                BindingRootMock.Setup(p => p.Bind<string>()).Returns(bindingSyntaxMock.Object);

                var actual = NinjectModule.Bind<string>();

                Assert.Same(bindingSyntaxMock.Object, actual);

                KernelConfigurationMock.Verify(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()), Times.Once);
                BindingRootMock.Verify(p => p.Bind<string>(), Times.Once);
            }
        }

        public class WhenOnLoadIsCalled : NinjectModuleTests
        {
            [Fact]
            public void ArgumentNullExceptionShouldBeThrownWhenKernelConfigurationIsNull()
            {
                const IKernelConfiguration kernelConfiguration = null;

                var actual = Assert.Throws<ArgumentNullException>(() => ((INinjectModule) NinjectModule).OnLoad(kernelConfiguration));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(kernelConfiguration), actual.ParamName);
            }

            [Fact]
            public void XernelConfigurationShouldBeAssigned()
            {
                KernelConfigurationMock.Setup(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()))
                                       .Callback((Action<INewBindingRoot> action) => action(BindingRootMock.Object))
                                       .Returns(KernelConfigurationMock.Object);

                ((INinjectModule) NinjectModule).OnLoad(KernelConfigurationMock.Object);

                Assert.NotNull(NinjectModule.OnLoadKernelConfiguration);
                Assert.Same(KernelConfigurationMock.Object, NinjectModule.OnLoadKernelConfiguration);
            }

            [Fact]
            public void OnLoadShouldBeCalled()
            {
                KernelConfigurationMock.Setup(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()))
                                       .Callback((Action<INewBindingRoot> action) => action(BindingRootMock.Object))
                                       .Returns(KernelConfigurationMock.Object);

                ((INinjectModule) NinjectModule).OnLoad(KernelConfigurationMock.Object);

                Assert.Equal(1, NinjectModule.LoadCount);
            }
        }

        public class WhenOnLoadCompletedIsCalled : NinjectModuleTests
        {
            [Fact]
            public void ArgumentNullExceptionShouldBeThrownWhenKernelConfigurationIsNull()
            {
                const IKernelConfiguration kernelConfiguration = null;

                var actual = Assert.Throws<ArgumentNullException>(() => ((INinjectModule)NinjectModule).OnLoadCompleted(kernelConfiguration));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(kernelConfiguration), actual.ParamName);
            }

            [Fact]
            public void XernelConfigurationShouldBeAssigned()
            {
                ((INinjectModule)NinjectModule).OnLoadCompleted(KernelConfigurationMock.Object);

                Assert.NotNull(NinjectModule.OnLoadCompletedKernelConfiguration);
                Assert.Same(KernelConfigurationMock.Object, NinjectModule.OnLoadCompletedKernelConfiguration);
            }

            [Fact]
            public void OnLoadCompletedShouldBeCalled()
            {
                ((INinjectModule)NinjectModule).OnLoadCompleted(KernelConfigurationMock.Object);

                Assert.Equal(1, NinjectModule.LoadCompletedCount);
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
                KernelConfigurationMock.Setup(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()))
                                       .Callback((Action<INewBindingRoot> action) => action(BindingRootMock.Object))
                                       .Returns(KernelConfigurationMock.Object);

                ((INinjectModule) NinjectModule).OnLoad(KernelConfigurationMock.Object);

                KernelConfigurationMock.Verify(p => p.Bindings(It.IsAny<Action<INewBindingRoot>>()), Times.Once);
            }

            [Fact]
            public void CallShouldBeDelegatedToKernelConfiguration()
            {
                BindingRootMock.Setup(p => p.Unbind<string>());

                NinjectModule.Unbind<string>();

                BindingRootMock.Verify(p => p.Unbind<string>(), Times.Once);
            }
        }

        public class MyNinjectModule : NinjectModule
        {
            private int _loadCount;
            private int _loadCompletedCount;

            public int LoadCount
            {
                get { return _loadCount; }
            }

            public int LoadCompletedCount
            {
                get { return _loadCompletedCount; }
            }

            public IKernelConfiguration OnLoadKernelConfiguration { get; private set; }

            public IKernelConfiguration OnLoadCompletedKernelConfiguration { get; private set; }

            protected override void OnLoad(IKernelConfiguration kernelConfiguration)
            {
                OnLoadKernelConfiguration = kernelConfiguration;
                Interlocked.Increment(ref _loadCount);
            }

            protected override void OnLoadCompleted(IKernelConfiguration kernelConfiguration)
            {
                OnLoadCompletedKernelConfiguration = kernelConfiguration;
                Interlocked.Increment(ref _loadCompletedCount);
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
