using System.Linq;
using System.Reflection;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Infrastructure.Language;
using Ninject.Injection;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.MethodInjectionStrategyTests
{
    using FluentAssertions;
    using System.Collections.Generic;

    public class MethodInjectionStrategyContext
    {
        protected readonly MethodInjectionStrategy strategy;

        public MethodInjectionStrategyContext()
        {
            this.strategy = new MethodInjectionStrategy();
        }
    }

    public class WhenActivateIsCalled : MethodInjectionStrategyContext
    {
        private Dummy instance;

        private Mock<IMethodInjectionDirective> method1DirectiveMock;
        private Mock<ITarget<ParameterInfo>> method1Target1Mock;
        private Mock<ITarget<ParameterInfo>> method1Target2Mock;
        private object method1Target1ResolvedValue;
        private object method1Target2ResolvedValue;
        private Mock<MethodInjector> method1InjectorMock;

        private Mock<IMethodInjectionDirective> method2DirectiveMock;
        private Mock<ITarget<ParameterInfo>> method2Target1Mock;
        private Mock<MethodInjector> method2InjectorMock;
        private object method2Target1ResolvedValue;

        private Mock<IContext> contextMock;
        private Mock<IPlan> planMock;

        public WhenActivateIsCalled()
        {
            this.instance = new Dummy();

            this.contextMock = new Mock<IContext>();
            this.planMock = new Mock<IPlan>();
            this.method1Target1ResolvedValue = new object();
            this.method1Target2ResolvedValue = new object();
            this.method2Target1ResolvedValue = new object();

            method1DirectiveMock = new Mock<IMethodInjectionDirective>(MockBehavior.Strict);
            method1InjectorMock = new Mock<MethodInjector>(MockBehavior.Strict);
            method1Target1Mock = new Mock<ITarget<ParameterInfo>>(MockBehavior.Strict);
            method1Target2Mock = new Mock<ITarget<ParameterInfo>>(MockBehavior.Strict);

            method2DirectiveMock = new Mock<IMethodInjectionDirective>(MockBehavior.Strict);
            method2InjectorMock = new Mock<MethodInjector>(MockBehavior.Strict);
            method2Target1Mock = new Mock<ITarget<ParameterInfo>>(MockBehavior.Strict);

            var mockSequence = new MockSequence();

            contextMock.InSequence(mockSequence)
                       .SetupGet(x => x.Plan)
                       .Returns(this.planMock.Object);
            planMock.InSequence(mockSequence)
                    .Setup(x => x.GetMethods())
                    .Returns(new List<IMethodInjectionDirective> { method1DirectiveMock.Object, method2DirectiveMock.Object });

            method1DirectiveMock.InSequence(mockSequence)
                                .Setup(p => p.Injector)
                                .Returns(this.method1InjectorMock.Object);
            method1DirectiveMock.InSequence(mockSequence)
                                .Setup(p => p.Targets)
                                .Returns(new ITarget<ParameterInfo>[] { method1Target1Mock.Object, method1Target2Mock.Object });
            method1Target1Mock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
                              .Returns(this.method1Target1ResolvedValue);
            method1Target2Mock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
                              .Returns(this.method1Target2ResolvedValue);
            method1InjectorMock.InSequence(mockSequence)
                               .Setup(p => p(this.instance, this.method1Target1ResolvedValue, this.method1Target2ResolvedValue));

            method2DirectiveMock.InSequence(mockSequence)
                                 .Setup(p => p.Injector)
                                 .Returns(this.method2InjectorMock.Object);
            method2DirectiveMock.InSequence(mockSequence)
                                .Setup(p => p.Targets)
                                .Returns(new ITarget<ParameterInfo>[] { method2Target1Mock.Object });
            method2Target1Mock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
                              .Returns(this.method2Target1ResolvedValue);
            method2InjectorMock.InSequence(mockSequence)
                               .Setup(p => p(this.instance, this.method2Target1ResolvedValue));
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.planMock.Verify(x => x.GetMethods(), Times.Once);
        }

        [Fact]
        public void ResolvesValuesForEachTargetOfEachDirective()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.method1Target1Mock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);
            this.method1Target2Mock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);
            this.method2Target1Mock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            method1InjectorMock.Verify(p => p(this.instance, this.method1Target1ResolvedValue, this.method1Target2ResolvedValue), Times.Once);
            method2InjectorMock.Verify(p => p(this.instance, this.method2Target1ResolvedValue), Times.Once);
        }
    }

    public class Dummy
    {
        public void Foo(int a, string b) { }
        public void Bar(IWeapon weapon) { }
    }
}