namespace Ninject.Tests.Integration.ModuleLoadingTests.Fakes
{
    using Ninject.Modules;

    public class TestModule : NinjectModule
    {
        protected override void OnLoad(Builder.IKernelConfiguration kernelConfiguration)
        {
        }
    }
}