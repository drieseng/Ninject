namespace Ninject.Tests.Unit.Syntax
{
    using Moq;
    using Ninject.Components;
    using Ninject.Syntax;
    using System;
    using Xunit;

    public class BindingRootTests
    {
        private Mock<IExceptionFormatter> _exceptionFormatterMock;
        private NewBindingRoot _bindingRoot;


        public BindingRootTests()
        {
            _exceptionFormatterMock = new Mock<IExceptionFormatter>();
            _bindingRoot = new NewBindingRoot(_exceptionFormatterMock.Object);
        }

        [Fact]
        public void Bind_ShouldThrowArgumentNullExceptionWhenServicesIsNull()
        {
            const Type[] services = null;

            var actual = Assert.Throws<ArgumentNullException>(() => _bindingRoot.Bind(services));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(services), actual.ParamName);
        }

        [Fact]
        public void Bind_ShouldThrowArgumentExceptionWhenServicesIsEmpty()
        {
            var services = Array.Empty<Type>();

            var actual = Assert.Throws<ArgumentException>(() => _bindingRoot.Bind(services));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Specify at least one type to bind.{Environment.NewLine}Parameter name: {actual.ParamName}", actual.Message);
            Assert.Equal(nameof(services), actual.ParamName);
        }

        [Fact]
        public void Rebind_ShouldThrowArgumentNullExceptionWhenServicesIsNull()
        {
            const Type[] services = null;

            var actual = Assert.Throws<ArgumentNullException>(() => _bindingRoot.Rebind(services));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(services), actual.ParamName);
        }

        [Fact]
        public void Rebind_ShouldThrowArgumentExceptionWhenServicesIsEmpty()
        {
            var services = Array.Empty<Type>();

            var actual = Assert.Throws<ArgumentException>(() => _bindingRoot.Rebind(services));

            Assert.Null(actual.InnerException);
            Assert.Equal($"Specify at least one type to bind.{Environment.NewLine}Parameter name: {actual.ParamName}", actual.Message);
            Assert.Equal(nameof(services), actual.ParamName);
        }

        [Fact]
        public void Unbind_Service_ShouldThrowArgumentNullExceptionWhenServiceIsNull()
        {
            const Type service = null;

            var actual = Assert.Throws<ArgumentNullException>(() => _bindingRoot.Unbind(service));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(service), actual.ParamName);
        }
    }
}
