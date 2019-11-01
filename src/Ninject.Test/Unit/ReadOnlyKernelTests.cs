using System;
using System.Collections.Generic;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Components;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;
using Ninject.Syntax;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Test.Unit
{
    public class ReadOnlyKernelTests
    {
        protected Mock<ICache> CacheMock { get; }
        protected Mock<IPlanner> PlannerMock { get; }
        protected Mock<IPipeline> PipelineMock { get; }
        protected Mock<IExceptionFormatter> ExceptionFormatterMock { get; }
        protected Mock<IContextFactory> ContextFactoryMock { get; }
        protected Mock<IBindingPrecedenceComparer> BindingPrecedenceComparerMock { get; }
        protected Mock<IBindingResolver> BindingResolverMock1 { get; }
        protected Mock<IBindingResolver> BindingResolverMock2 { get; }
        protected Mock<IMissingBindingResolver> MissingBindingResolverMock1 { get; }
        protected Mock<IMissingBindingResolver> MissingBindingResolverMock2 { get; }
        protected List<IBindingResolver> BindingResolvers { get; }
        protected List<IMissingBindingResolver> MissingBindingResolvers { get; }

        public ReadOnlyKernelTests()
        {
            CacheMock = new Mock<ICache>(MockBehavior.Strict);
            PlannerMock = new Mock<IPlanner>(MockBehavior.Strict);
            PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            ExceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            ContextFactoryMock = new Mock<IContextFactory>(MockBehavior.Strict);
            BindingPrecedenceComparerMock = new Mock<IBindingPrecedenceComparer>(MockBehavior.Strict);
            BindingResolverMock1 = new Mock<IBindingResolver>(MockBehavior.Strict);
            BindingResolverMock2 = new Mock<IBindingResolver>(MockBehavior.Strict);
            MissingBindingResolverMock1 = new Mock<IMissingBindingResolver>(MockBehavior.Strict);
            MissingBindingResolverMock2 = new Mock<IMissingBindingResolver>(MockBehavior.Strict);

            BindingResolvers = new List<IBindingResolver>
                {
                    BindingResolverMock1.Object,
                    BindingResolverMock2.Object
                };
            MissingBindingResolvers = new List<IMissingBindingResolver>
                {
                    MissingBindingResolverMock1.Object,
                    MissingBindingResolverMock2.Object
                };
        }

        public class WhenCanResolveIsCalled : ReadOnlyKernelTests
        {
            private ReadOnlyKernel _readOnlyKernel;

            public WhenCanResolveIsCalled()
            {
                var bindingsForSword = new List<IBinding>
                    {
                        CreateBinding(typeof(Sword), true)
                    };

                var bindings = new Dictionary<Type, ICollection<IBinding>>
                    {
                        { typeof(Sword), bindingsForSword }
                    };

                BindingResolverMock1.Setup(p => p.Resolve(bindings, typeof(Sword))).Returns(bindingsForSword);
                BindingResolverMock2.Setup(p => p.Resolve(bindings, typeof(Sword))).Returns(Array.Empty<IBinding>());

                _readOnlyKernel = CreateReadOnlyKernel(bindings);
            }

            [Fact]
            public void CanResolve_Request_ShouldThrowArgumentNullExceptionWhenRequestIsNull()
            {
                const IRequest request = null;

                var actualException = Assert.Throws<ArgumentNullException>(() => _readOnlyKernel.CanResolve(request));

                Assert.Null(actualException.InnerException);
                Assert.Equal(nameof(request), actualException.ParamName);
            }

            [Fact]
            public void CanResolve_Request_ShouldNotIgnoreImplicitBindings()
            {
                var request = _readOnlyKernel.CreateRequest(typeof(Sword), null, Array.Empty<IParameter>(), false, true);

                Assert.True(_readOnlyKernel.CanResolve(request));
            }

            [Fact]
            public void CanResolve_RequestAndIgnoreImplicitBindings_ShouldThrowArgumentNullExceptionWhenRequestIsNull()
            {
                const IRequest request = null;

                var actualException = Assert.Throws<ArgumentNullException>(() => _readOnlyKernel.CanResolve(request, false));

                Assert.Null(actualException.InnerException);
                Assert.Equal(nameof(request), actualException.ParamName);
            }

            [Fact]
            public void CanResolve_RequestAndIgnoreImplicitBindings_ShouldIgnoreImplicitBindingsWhenIgnoreImplicitBindingsIsTrue()
            {
                var request = _readOnlyKernel.CreateRequest(typeof(Sword), null, Array.Empty<IParameter>(), false, true);

                Assert.False(_readOnlyKernel.CanResolve(request, true));
            }

            [Fact]
            public void CanResolve_RequestAndIgnoreImplicitBindings_ShouldNotIgnoreImplicitBindingsWhenIgnoreImplicitBindingsIsFalse()
            {
                var request = _readOnlyKernel.CreateRequest(typeof(Sword), null, Array.Empty<IParameter>(), false, true);

                Assert.True(_readOnlyKernel.CanResolve(request, false));
            }

            private static IBinding CreateBinding(Type service, bool isImplicit)
            {
                return new Binding(service)
                {
                    IsImplicit = isImplicit
                };
            }
        }

        public class WhenInjectIsCalled : ReadOnlyKernelTests
        {
            [Fact]
            public void Inject_ShouldThrowArgumentNullExceptionWhenInstanceIsNull()
            {
                var bindings = new Dictionary<Type, ICollection<IBinding>>();
                var readOnlyKernel = CreateReadOnlyKernel(bindings);

                const object instance = null;
                var parameters = new IParameter[0];

                var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.Inject(instance, parameters));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(instance), actual.ParamName);
            }

            [Fact]
            public void Inject_ShouldThrowArgumentNullExceptionWhenParametersIsNull()
            {
                var bindings = new Dictionary<Type, ICollection<IBinding>>();
                var readOnlyKernel = CreateReadOnlyKernel(bindings);

                var instance = new object();
                const IParameter[] parameters = null;

                var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.Inject(instance, parameters));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(parameters), actual.ParamName);
            }
        }

        public class WhenReleaseIsCalled : ReadOnlyKernelTests
        {
            [Fact]
            public void Release_ShouldThrowArgumentNullExceptionWhenInstanceIsNull()
            {
                var bindings = new Dictionary<Type, ICollection<IBinding>>();
                var readOnlyKernel = CreateReadOnlyKernel(bindings);

                const object instance = null;

                var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.Release(instance));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(instance), actual.ParamName);
            }
        }

        public class WhenResolveIsCalled : ReadOnlyKernelTests
        {
            [Fact]
            public void Resolve_ShouldThrowArgumentNullExceptionWhenRequestIsNull()
            {
                var bindings = new Dictionary<Type, ICollection<IBinding>>();
                var readOnlyKernel = CreateReadOnlyKernel(bindings);

                const IRequest request = null;

                var actual = Assert.Throws<ArgumentNullException>(() => readOnlyKernel.Resolve(request));

                Assert.Null(actual.InnerException);
                Assert.Equal(nameof(request), actual.ParamName);
            }
        }

        private ReadOnlyKernel CreateReadOnlyKernel(Dictionary<Type, ICollection<IBinding>> bindings)
        {
            var sequence = new MockSequence();

            BindingResolverMock1.InSequence(sequence)
                                 .Setup(p => p.Resolve(It.IsAny<Dictionary<Type, ICollection<IBinding>>>(), typeof(IReadOnlyKernel)))
                                 .Returns(Array.Empty<IBinding>);
            BindingResolverMock2.InSequence(sequence)
                                 .Setup(p => p.Resolve(It.IsAny<Dictionary<Type, ICollection<IBinding>>>(), typeof(IReadOnlyKernel)))
                                 .Returns(Array.Empty<IBinding>);
            BindingResolverMock1.InSequence(sequence)
                                 .Setup(p => p.Resolve(It.IsAny<Dictionary<Type, ICollection<IBinding>>>(), typeof(IResolutionRoot)))
                                 .Returns(Array.Empty<IBinding>);
            BindingResolverMock2.InSequence(sequence)
                                 .Setup(p => p.Resolve(It.IsAny<Dictionary<Type, ICollection<IBinding>>>(), typeof(IResolutionRoot)))
                                 .Returns(Array.Empty<IBinding>);

            return new ReadOnlyKernel(bindings,
                                      CacheMock.Object,
                                      PlannerMock.Object,
                                      PipelineMock.Object,
                                      ExceptionFormatterMock.Object,
                                      ContextFactoryMock.Object,
                                      BindingPrecedenceComparerMock.Object,
                                      BindingResolvers,
                                      MissingBindingResolvers);
        }
    }
}
