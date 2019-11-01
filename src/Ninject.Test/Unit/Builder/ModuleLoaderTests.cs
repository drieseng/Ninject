using Moq;
using Ninject.Builder;
using Ninject.Components;
using Ninject.Modules;
using System;
using Xunit;

namespace Ninject.Tests.Unit.Builder
{
    public class ModuleLoaderTests
    {
        private Mock<IKernelConfiguration> _kernelConfigurationMock;
        private Mock<IExceptionFormatter> _exceptionFormatterMock;
        private ModuleLoader _moduleLoader;

        public ModuleLoaderTests()
        {
            _kernelConfigurationMock = new Mock<IKernelConfiguration>(MockBehavior.Strict);
            _exceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            _moduleLoader = new ModuleLoader(_kernelConfigurationMock.Object, _exceptionFormatterMock.Object);
        }

        [Fact]
        public void HasModule_ShouldReturnFalseWhenNoModulesAreLoaded()
        {
            Assert.False(_moduleLoader.HasModule("InjectionModule"));
        }

        [Fact]
        public void HasModule_ShouldThrowArgumentNullExceptionWhenNameIsNull()
        {
            const string name = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _moduleLoader.HasModule(name));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(name), actualException.ParamName);
        }

        [Fact]
        public void HasModule_ShouldReturnFalseWhenNoModuleWithSpecifiedNameIsLoaded()
        {
            var moduleMock = CreateModuleMock("MyModule");

            moduleMock.Setup(p => p.OnLoad(_kernelConfigurationMock.Object));
            _moduleLoader.Load(moduleMock.Object);

            Assert.False(_moduleLoader.HasModule("InjectionModule"));
        }

        [Fact]
        public void HasModule_ShouldReturnTrueWhenModuleWithSpecifiedNameIsLoaded()
        {
            const string name = "MyModule";
            var moduleMock = CreateModuleMock(name);

            moduleMock.Setup(p => p.OnLoad(_kernelConfigurationMock.Object));

            _moduleLoader.Load(moduleMock.Object);

            Assert.True(_moduleLoader.HasModule(name));
        }

        [Fact]
        public void Load_ArrayOfINinjectModule_ShouldThrowNotSupportedExceptionWhenNameOfOneOfModulesIsNull()
        {
            const string name = null;
            const string exceptionMessage = "NULL";
            var moduleMock = CreateModuleMock(name);

            _exceptionFormatterMock.Setup(p => p.ModulesWithNullNameAreNotSupported()).Returns(exceptionMessage);

            var actualException = Assert.Throws<NotSupportedException>(() => _moduleLoader.Load(moduleMock.Object));

            Assert.Null(actualException.InnerException);
            Assert.Same(exceptionMessage, actualException.Message);
        }

        [Fact]
        public void Load_ArrayOfINinjectModule_ShouldThrowNotSupportedExceptionWhenModuleIsLoadedWithSameName()
        {
            const string exceptionMessage = "DÜPLICATE";
            var moduleMock1 = CreateModuleMock("A");
            var moduleMock2 = CreateModuleMock("A");

            moduleMock1.Setup(p => p.OnLoad(_kernelConfigurationMock.Object));
            _moduleLoader.Load(moduleMock1.Object);

            _exceptionFormatterMock.Setup(p => p.ModuleWithSameNameIsAlreadyLoaded(moduleMock2.Object, moduleMock1.Object))
                                   .Returns(exceptionMessage);

            var actualException = Assert.Throws<NotSupportedException>(() => _moduleLoader.Load(moduleMock2.Object));

            Assert.Null(actualException.InnerException);
            Assert.Same(exceptionMessage, actualException.Message);
        }

        [Fact]
        public void Load_ArrayOfINinjectModule_ShouldInvokeOnLoadOnEachLoadedModule()
        {
            var moduleMock1 = CreateModuleMock("A");
            var moduleMock2 = CreateModuleMock("B");

            moduleMock1.Setup(p => p.OnLoad(_kernelConfigurationMock.Object));
            moduleMock2.Setup(p => p.OnLoad(_kernelConfigurationMock.Object));

            _moduleLoader.Load(moduleMock1.Object, moduleMock2.Object);

            moduleMock1.Verify(p => p.OnLoad(_kernelConfigurationMock.Object), Times.Once);
            moduleMock2.Verify(p => p.OnLoad(_kernelConfigurationMock.Object), Times.Once);
        }

        private static Mock<INinjectModule> CreateModuleMock(string name)
        {
            var moduleMock = new Mock<INinjectModule>(MockBehavior.Strict);
            moduleMock.Setup(p => p.Name).Returns(name);
            return moduleMock;
        }
    }
}
