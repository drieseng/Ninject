namespace Ninject.Tests.Integration.SpecialResolutionTests
{
    using FluentAssertions;
    using Ninject.Builder;
    using Ninject.Syntax;
    using Xunit;

    public class WhenServiceRequestsKernel
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            var kernelBuilder = new KernelBuilder().Features(f => f.ConstructorInjection())
                                                   .Bindings(bindings => bindings.Bind<RequestsKernel>().ToSelf());

            using (var kernel = kernelBuilder.Build())
            {
                var instance = kernel.Get<RequestsKernel>();

                instance.Should().NotBeNull();
                instance.Kernel.Should().NotBeNull();
                instance.Kernel.Should().BeSameAs(kernel);
            }
        }
    }

    public class WhenServiceRequestsResolutionRoot
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            var kernelBuilder = new KernelBuilder().Features(f => f.ConstructorInjection())
                                                   .Bindings(bindings => bindings.Bind<RequestsResolutionRoot>().ToSelf());

            using (var kernel = kernelBuilder.Build())
            {
                var instance = kernel.Get<RequestsResolutionRoot>();

                instance.Should().NotBeNull();
                instance.ResolutionRoot.Should().NotBeNull();
                instance.ResolutionRoot.Should().BeSameAs(kernel);
            }
        }
    }

    public class WhenServiceRequestsString
    {
        [Fact]
        public void InstanceOfStringIsInjected()
        {
            var kernelBuilder = new KernelBuilder().Features(f => f.ConstructorInjection())
                                                   .Bindings(bindings => bindings.Bind<RequestsString>().ToSelf());

            using (var kernel = kernelBuilder.Build())
            {
                Assert.Throws<ActivationException>(() => kernel.Get<RequestsString>());
            }
        }
    }

    public class RequestsKernel
    {
        public IReadOnlyKernel Kernel { get; set; }

        public RequestsKernel(IReadOnlyKernel kernel)
        {
            this.Kernel = kernel;
        }
    }

    public class RequestsResolutionRoot
    {
        public IResolutionRoot ResolutionRoot { get; set; }

        public RequestsResolutionRoot(IResolutionRoot resolutionRoot)
        {
            this.ResolutionRoot = resolutionRoot;
        }
    }

    public class RequestsString
    {
        public string StringValue { get; set; }

        public RequestsString(string stringValue)
        {
            this.StringValue = stringValue;
        }
    }
}