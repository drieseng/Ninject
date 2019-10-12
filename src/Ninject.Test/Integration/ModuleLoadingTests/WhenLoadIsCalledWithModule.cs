namespace Ninject.Tests.Integration.ModuleLoadingTests
{
    using System;
    using System.Text;
    using FluentAssertions;
    using Moq;
    using Ninject.Tests.Integration.ModuleLoadingTests.Fakes;
    using Xunit;

    public class WhenLoadIsCalledWithModule : ModuleLoadingContext
    {
        [Fact]
        public void IdenticalNamedModulesFromDifferenNamespacesCanBeLoadedTogether()
        {
            this.KernelBuilder.Modules(m => m.Load(new TestModule()));
            this.KernelBuilder.Modules(m => m.Load(new OtherFakes.TestModule()));
        }

        [Fact]
        public void MockModulePassedToLoadIsLoadedAndCallsOnLoad()
        {
            var moduleMock = this.CreateModuleMock("SomeName");
            var module = moduleMock.Object;

            this.KernelBuilder.Modules(m => m.Load(module));

            moduleMock.Verify(x => x.OnLoad(this.KernelBuilder), Times.Once);
            this.KernelBuilder.ModuleBuilder.Modules.Should().BeEquivalentTo(module);
        }

        [Fact]
        public void ModuleInstanceWithNullNameIsNotSupported()
        {
            var module = this.CreateModule(null);

            Action moduleLoadingAction = () => this.KernelBuilder.Modules(m => m.Load(module));

            moduleLoadingAction.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void TwoModulesWithSameNamesAreNotSupported()
        {
            const string ModuleName = "SomeModuleName";
            var module1 = this.CreateModule(ModuleName);
            var module2 = this.CreateModule(ModuleName);

            this.KernelBuilder.Modules(m => m.Load(module1));
            Action moduleLoadingAction = () => this.KernelBuilder.Modules(m => m.Load(module2));

            moduleLoadingAction.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ModulesAreVerifiedAfterAllModulesAreLoaded()
        {
            var moduleMock1 = this.CreateModuleMock("SomeName1");
            var moduleMock2 = this.CreateModuleMock("SomeName2");
            var orderStringBuilder = new StringBuilder();

            moduleMock1.Setup(m => m.OnLoad(this.KernelBuilder)).Callback(() => orderStringBuilder.Append("LoadModule1 "));
            moduleMock2.Setup(m => m.OnLoad(this.KernelBuilder)).Callback(() => orderStringBuilder.Append("LoadModule2 "));

            this.KernelBuilder.Modules(m => m.Load(moduleMock1.Object, moduleMock2.Object));

            moduleMock1.Setup(m => m.OnLoadCompleted(this.KernelBuilder)).Callback(() => orderStringBuilder.Append("VerifyModule1 "));
            moduleMock2.Setup(m => m.OnLoadCompleted(this.KernelBuilder)).Callback(() => orderStringBuilder.Append("VerifyModule2 "));

            this.KernelBuilder.Build();

            orderStringBuilder.ToString().Should().Be("LoadModule1 LoadModule2 VerifyModule VerifyModule ");
        }
    }
}