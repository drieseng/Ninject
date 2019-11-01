﻿namespace Ninject.Tests.Unit.CallbackProviderTests
{
    using FluentAssertions;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Tests.Fakes;
    using System;
    using Xunit;

    public class CallbackProviderContext
    {
        protected CallbackProvider<Sword> provider;
        protected Mock<IContext> contextMock;

        public CallbackProviderContext()
        {
            this.SetUp();
        }

        public void SetUp()
        {
            this.contextMock = new Mock<IContext>();
        }
    }

    public class WhenMethodIsNull
    {
        [Fact]
        public void ArgumentNullExceptionIsThrown()
        {
            const Func<IContext, string> method = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => new CallbackProvider<string>(method));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(method), actualException.ParamName);
        }
    }

    public class WhenCreateIsCalled : CallbackProviderContext
    {
        private Mock<Func<IContext, Sword>> _providerCallbackMock;

        public WhenCreateIsCalled()
        {
            _providerCallbackMock = new Mock<Func<IContext, Sword>>(MockBehavior.Strict);
        }

        [Fact]
        public void ProviderInvokesCallbackToRetrieveValue()
        {
            var providerInstance = new Sword();

            _providerCallbackMock.Setup(p => p(this.contextMock.Object))
                                 .Returns(providerInstance);

            this.provider = new CallbackProvider<Sword>(_providerCallbackMock.Object);

            var result = this.provider.Create(this.contextMock.Object);

            result.Should().BeSameAs(providerInstance);
            _providerCallbackMock.Verify(p => p(this.contextMock.Object), Times.Once);
        }
    }
}