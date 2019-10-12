namespace Ninject.Tests.Integration.ModuleLoadingTests.OtherFakes
{
    using Ninject.Modules;

    public class TestModule : NinjectModule
    {
        protected override void OnLoad(Builder.IKernelConfiguration kernelConfiguration)
        {
        }
    }
}