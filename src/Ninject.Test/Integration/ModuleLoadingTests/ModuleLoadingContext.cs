namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;

    using Moq;
    using Ninject.Builder;
    using Ninject.Modules;

    public class ModuleLoadingContext : IDisposable
    {
        public ModuleLoadingContext()
        {
            this.KernelBuilder = new KernelBuilder();
        }

        public void Dispose()
        {
        }

        protected INinjectSettings NinjectSettings { get; private set; }
        protected KernelBuilder KernelBuilder { get; }

        protected string GetRegularMockModuleName()
        {
            return "TestModuleName";
        }

        protected Mock<INinjectModule> CreateModuleMock(string name)
        {
            var moduleMock = new Mock<INinjectModule>();
            moduleMock.SetupGet(x => x.Name).Returns(name);
            return moduleMock;
        }

        protected INinjectModule CreateModule(string name)
        {
            return this.CreateModuleMock(name).Object;
        }
    }
}