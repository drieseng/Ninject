//-------------------------------------------------------------------------------
// <copyright file="PropertyInjectionStrategyTests.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2013 Ninject Project Contributors
//   Authors: Ivan Appert (iappert@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Tests.Unit.PropertyInjectionStrategyTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Moq;
    using Xunit;

    using Ninject.Activation;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Injection;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;
    using Ninject.Selection;

    public class PropertyInjectionDirectiveContext
    {
        protected Mock<IPropertyReflectionSelector> PropertyReflectionSelectorMock { get; }
        public Mock<IInjectorFactory> InjectorFactoryMock { get; }
        protected Mock<IExceptionFormatter> ExceptionFormatterMock { get; }
        protected Mock<IContext> ContextMock { get; }
        protected Mock<IPlan> PlanMock { get; }
        protected Random Random { get; }
        protected PropertyInjectionStrategy Strategy { get; }

        public PropertyInjectionDirectiveContext()
        {
            this.PropertyReflectionSelectorMock = new Mock<IPropertyReflectionSelector>(MockBehavior.Strict);
            this.InjectorFactoryMock = new Mock<IInjectorFactory>(MockBehavior.Strict);
            this.ExceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            this.ContextMock = new Mock<IContext>(MockBehavior.Strict);
            this.PlanMock = new Mock<IPlan>(MockBehavior.Strict);

            this.Random = new Random();

            this.Strategy = new PropertyInjectionStrategy(this.PropertyReflectionSelectorMock.Object,
                                                          this.InjectorFactoryMock.Object,
                                                          this.ExceptionFormatterMock.Object);
        }
    }

    public class WhenConstructorIsInvoked : PropertyInjectionDirectiveContext
    {
        [Fact]
        public void ArgumentNullExceptionIsThrownWhenPropertyReflectionSelectorIsNull()
        {
            const IPropertyReflectionSelector propertyReflectionSelector = null;

            var actualException = Assert.Throws<ArgumentNullException>(() =>
                    new PropertyInjectionStrategy(propertyReflectionSelector,
                                                  this.InjectorFactoryMock.Object,
                                                  this.ExceptionFormatterMock.Object));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(propertyReflectionSelector), actualException.ParamName);
        }

        [Fact]
        public void ArgumentNullExceptionIsThrownWhenInjectorFactoryIsNull()
        {
            const IInjectorFactory injectorFactory = null;

            var actualException = Assert.Throws<ArgumentNullException>(() =>
                    new PropertyInjectionStrategy(this.PropertyReflectionSelectorMock.Object,
                                                  injectorFactory,
                                                  this.ExceptionFormatterMock.Object));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(injectorFactory), actualException.ParamName);

        }

        [Fact]
        public void ArgumentNullExceptionIsThrownWhenExceptionFormatterIsNull()
        {
            const IExceptionFormatter exceptionFormatter = null;

            var actualException = Assert.Throws<ArgumentNullException>(() =>
                    new PropertyInjectionStrategy(this.PropertyReflectionSelectorMock.Object,
                                                  this.InjectorFactoryMock.Object,
                                                  exceptionFormatter));

            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(exceptionFormatter), actualException.ParamName);
        }
    }

    public class WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithoutPropertyValues : PropertyInjectionDirectiveContext
    {
        private Mock<PropertyInjector> fooInjectorMock;
        private Mock<PropertyInjector> barInjectorMock;
        private Mock<ITarget<PropertyInfo>> fooTargetMock;
        private Mock<ITarget<PropertyInfo>> barTargetMock;
        private Mock<IPropertyInjectionDirective> fooPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> barPropertyDirectiveMock;
        private Dummy instance = new Dummy();
        private int fooResolvedValue;
        private string barResolvedValue;
        private List<IPropertyInjectionDirective> directives;

        public WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithoutPropertyValues()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.barInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.barPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);

            this.directives = new List<IPropertyInjectionDirective>
                {
                    fooPropertyDirectiveMock.Object,
                    barPropertyDirectiveMock.Object
                };

            this.fooResolvedValue = this.Random.Next();
            this.barResolvedValue = this.Random.Next().ToString();

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(Array.Empty<IParameter>());
            this.fooPropertyDirectiveMock.Setup(p => p.Target).Returns(fooTargetMock.Object);
            this.fooTargetMock.Setup(p => p.ResolveWithin(this.ContextMock.Object)).Returns(fooResolvedValue);
            this.fooPropertyDirectiveMock.Setup(p => p.Injector).Returns(fooInjectorMock.Object);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooResolvedValue));
            this.barPropertyDirectiveMock.Setup(p => p.Target).Returns(barTargetMock.Object);
            this.barTargetMock.Setup(p => p.ResolveWithin(this.ContextMock.Object)).Returns(barResolvedValue);
            this.barPropertyDirectiveMock.Setup(p => p.Injector).Returns(barInjectorMock.Object);
            this.barInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.barResolvedValue));
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.PlanMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ReturnsInstance()
        {
            var initialized = this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            Assert.Same(this.instance, initialized);
        }

        [Fact]
        public void ResolvesValuesForEachTargetOfEachDirective()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.fooPropertyDirectiveMock.Verify(p => p.Target, Times.Once);
            this.fooTargetMock.Verify(p => p.ResolveWithin(this.ContextMock.Object), Times.Once);

            this.barPropertyDirectiveMock.Verify(p => p.Target, Times.Once);
            this.barTargetMock.Verify(p => p.ResolveWithin(this.ContextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooResolvedValue), Times.Once);
            this.barInjectorMock.Verify(x => x(this.instance, this.barResolvedValue), Times.Once);
        }
    }

    public class WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_FullMatch : PropertyInjectionDirectiveContext
    {
        private Mock<PropertyInjector> fooInjectorMock;
        private Mock<PropertyInjector> barInjectorMock;
        private Mock<PropertyInjector> gooInjectorMock;
        private Mock<ITarget<PropertyInfo>> fooTargetMock;
        private Mock<ITarget<PropertyInfo>> barTargetMock;
        private Mock<ITarget<PropertyInfo>> gooTargetMock;
        private Mock<IPropertyValue> fooPropertyValueMock;
        private Mock<IPropertyValue> barPropertyValueMock;
        private Mock<IPropertyValue> gooPropertyValueMock;
        private Mock<IPropertyInjectionDirective> fooPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> barPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> gooPropertyDirectiveMock;
        private Dummy instance = new Dummy();
        private int fooPropertyValue;
        private string barPropertyValue;
        private string gooPropertyValue;
        private List<IPropertyInjectionDirective> directives;
        private IReadOnlyList<IParameter> parameters;

        public WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_FullMatch()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.barInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.gooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.gooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.fooPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);
            this.barPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);
            this.gooPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);
            this.fooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.barPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.gooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);

            this.fooPropertyValue = this.Random.Next();
            this.barPropertyValue = this.Random.Next().ToString();
            this.gooPropertyValue = this.Random.Next().ToString();

            this.directives = new List<IPropertyInjectionDirective>
                {
                    fooPropertyDirectiveMock.Object,
                    barPropertyDirectiveMock.Object,
                    gooPropertyDirectiveMock.Object
                };
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    this.barPropertyValueMock.Object,
                    this.fooPropertyValueMock.Object,
                    this.gooPropertyValueMock.Object,
                };

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.ContextMock.Object, this.fooTargetMock.Object))
                                     .Returns(this.fooPropertyValue);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.fooInjectorMock.Object);
            this.fooInjectorMock.InSequence(mockSequence)
                                         .Setup(p => p(this.instance, this.fooPropertyValue));

            #endregion Process Foo property injection directive

            #region Process Bar property injection directive

            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.barTargetMock.Object))
                                     .Returns(false);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.barTargetMock.Object))
                                     .Returns(true);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.ContextMock.Object, this.barTargetMock.Object))
                                     .Returns(this.barPropertyValue);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.barInjectorMock.Object);
            this.barInjectorMock.InSequence(mockSequence)
                                 .Setup(p => p(this.instance, this.barPropertyValue));

            #endregion Process Bar property injection directive

            #region Process Goo property injection directive

            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.barTargetMock.Object))
                                     .Returns(true);
            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.gooTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.ContextMock.Object, this.gooTargetMock.Object))
                                     .Returns(this.gooPropertyValue);
            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.gooInjectorMock.Object);
            this.gooInjectorMock.InSequence(mockSequence)
                                 .Setup(p => p(this.instance, this.gooPropertyValue));

            #endregion Process Goo property injection directive
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.PlanMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooPropertyValue), Times.Once);
            this.barInjectorMock.Verify(x => x(this.instance, this.barPropertyValue), Times.Once);
            this.gooInjectorMock.Verify(x => x(this.instance, this.gooPropertyValue), Times.Once);
        }
    }

    public class WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_MultiplePropertyValuesMatchGivenDirective : PropertyInjectionDirectiveContext
    {
        private Mock<ITarget<PropertyInfo>> fooTargetMock;
        private Mock<IPropertyInjectionDirective> fooPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> barPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> gooPropertyDirectiveMock;
        private Mock<IPropertyValue> fooPropertyValue1Mock;
        private Mock<IPropertyValue> fooPropertyValue2Mock;
        private Dummy instance = new Dummy();
        private List<IPropertyInjectionDirective> directives;
        private IReadOnlyList<IParameter> parameters;
        private string exceptionMessage;

        public WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_MultiplePropertyValuesMatchGivenDirective()
        {
            this.fooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.fooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.barPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.gooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.fooPropertyValue1Mock = new Mock<IPropertyValue>(MockBehavior.Strict);
            this.fooPropertyValue2Mock = new Mock<IPropertyValue>(MockBehavior.Strict);

            this.directives = new List<IPropertyInjectionDirective>
                {
                    fooPropertyDirectiveMock.Object,
                    gooPropertyDirectiveMock.Object,
                    barPropertyDirectiveMock.Object,
                };
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    this.fooPropertyValue1Mock.Object,
                    this.fooPropertyValue2Mock.Object
                };
            this.exceptionMessage = this.Random.Next().ToString();

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValue2Mock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValue1Mock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.ExceptionFormatterMock.InSequence(mockSequence)
                                       .Setup(p => p.MoreThanOnePropertyValueForTarget(this.ContextMock.Object, fooTargetMock.Object))
                                       .Returns(this.exceptionMessage);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.PlanMock.Verify(x => x.GetProperties());
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            Assert.Null(actualException.InnerException);
            Assert.Same(this.exceptionMessage, actualException.Message);
        }

        [Fact]
        public void VerifiesForEachPropertyValueWhetherItAppliesToDirective()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.fooPropertyValue1Mock.Verify(p => p.AppliesToTarget(this.ContextMock.Object, this.fooTargetMock.Object), Times.Once);
            this.fooPropertyValue2Mock.Verify(p => p.AppliesToTarget(this.ContextMock.Object, this.fooTargetMock.Object), Times.Once);
        }
    }

    public class WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_PropertyValuesWithNoCorrespondingDirective : PropertyInjectionDirectiveContext
    {
        private Mock<PropertyInjector> fooInjectorMock;
        private Mock<PropertyInjector> barInjectorMock;
        private Mock<ITarget<PropertyInfo>> fooTargetMock;
        private Mock<ITarget<PropertyInfo>> barTargetMock;
        private Mock<IPropertyValue> barPropertyValueMock;
        private Mock<IPropertyValue> gooPropertyValueMock;
        private Mock<IPropertyInjectionDirective> fooPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> barPropertyDirectiveMock;
        private Mock<IRequest> requestMock;
        private Dummy instance = new Dummy();
        private int fooPropertyValue;
        private string barPropertyValue;
        private string gooPropertyName;
        private List<IPropertyInjectionDirective> directives;
        private IReadOnlyList<IParameter> parameters;
        private string couldNotResolvePropertyExceptionMessage;

        public WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_PropertyValuesWithNoCorrespondingDirective()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.barInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.barPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);
            this.gooPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);
            this.fooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.barPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.requestMock = new Mock<IRequest>(MockBehavior.Strict);

            this.fooPropertyValue = this.Random.Next();
            this.barPropertyValue = this.Random.Next().ToString();

            this.gooPropertyName = this.Random.Next().ToString();
            this.couldNotResolvePropertyExceptionMessage = this.Random.Next().ToString();

            this.directives = new List<IPropertyInjectionDirective>
                {
                    fooPropertyDirectiveMock.Object,
                    barPropertyDirectiveMock.Object,
                };
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    this.barPropertyValueMock.Object,
                    this.gooPropertyValueMock.Object
                };

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.fooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.ContextMock.Object))
                              .Returns(this.fooPropertyValue);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.fooInjectorMock.Object);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooPropertyValue));

            #endregion Process Foo property injection directive

            #region Process Bar property injection directive

            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.barTargetMock.Object))
                                     .Returns(false);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, this.barTargetMock.Object))
                                     .Returns(true);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.ContextMock.Object, this.barTargetMock.Object))
                                     .Returns(this.barPropertyValue);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.barInjectorMock.Object);
            this.barInjectorMock.InSequence(mockSequence)
                                 .Setup(p => p(this.instance, this.barPropertyValue));

            #endregion Process Bar property injection directive

            this.ContextMock.InSequence(mockSequence)
                            .Setup(p => p.Request)
                            .Returns(this.requestMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.Name)
                                     .Returns(this.gooPropertyName);
            this.ExceptionFormatterMock.InSequence(mockSequence)
                                       .Setup(p => p.CouldNotResolvePropertyForValueInjection(this.requestMock.Object, this.gooPropertyName))
                                       .Returns(couldNotResolvePropertyExceptionMessage);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.PlanMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooPropertyValue), Times.Once);
            this.barInjectorMock.Verify(x => x(this.instance, this.barPropertyValue), Times.Once);
        }

        [Fact]
        public void ThrowsAnActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            Assert.Null(actualException.InnerException);
            Assert.Same(this.couldNotResolvePropertyExceptionMessage, actualException.Message);
        }
    }

    public class WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_ResolveWithinOfPropertyTargetThrowsActivationException : PropertyInjectionDirectiveContext
    {
        private Mock<PropertyInjector> fooInjectorMock;
        private Mock<ITarget<PropertyInfo>> fooTargetMock;
        private Mock<ITarget<PropertyInfo>> barTargetMock;
        private Mock<ITarget<PropertyInfo>> gooTargetMock;
        private Mock<IPropertyInjectionDirective> fooPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> barPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> gooPropertyDirectiveMock;
        private Mock<IPropertyValue> fooPropertyValueMock;
        private Mock<IPropertyValue> barPropertyValueMock;
        private Dummy instance = new Dummy();
        private int fooPropertyValue;
        private List<IPropertyInjectionDirective> directives;
        private IReadOnlyList<IParameter> parameters;
        private ActivationException activationException;

        public WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_ResolveWithinOfPropertyTargetThrowsActivationException()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.gooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.fooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.barPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.gooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.fooPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);
            this.barPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);

            this.fooPropertyValue = this.Random.Next();
            this.directives = new List<IPropertyInjectionDirective>
                {
                    fooPropertyDirectiveMock.Object,
                    gooPropertyDirectiveMock.Object,
                    barPropertyDirectiveMock.Object,
                };
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    this.fooPropertyValueMock.Object,
                    this.barPropertyValueMock.Object
                };
            this.activationException = new ActivationException();

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.ContextMock.Object, fooTargetMock.Object))
                                     .Returns(fooPropertyValue);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.fooInjectorMock.Object);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooPropertyValue));

            #endregion Process Foo property injection directive

            #region Process goo property injection directive

            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(gooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, gooTargetMock.Object))
                                     .Returns(false);
            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.gooTargetMock.Object);
            this.gooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.ContextMock.Object))
                              .Throws(this.activationException);

            #endregion Process goo property injection directive
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.PlanMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            Assert.Same(this.activationException, actualException);
        }

        [Fact]
        public void GetValueFromPropertyForDirectivesWithPropertyValueParameter()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.fooPropertyValueMock.Verify(p => p.GetValue(this.ContextMock.Object, this.fooTargetMock.Object), Times.Once);
        }

        [Fact]
        public void ResolvesValuesForDirectivesWithoutPropertyValueParameter()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.gooTargetMock.Verify(p => p.ResolveWithin(this.ContextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooPropertyValue), Times.Once);
        }
    }

    public class WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithoutPropertyValues_ResolveWithinOfPropertyTargetThrowsActivationException : PropertyInjectionDirectiveContext
    {
        private Mock<PropertyInjector> fooInjectorMock;
        private Mock<IPropertyInjectionDirective> fooPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> barPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> gooPropertyDirectiveMock;
        private Mock<ITarget<PropertyInfo>> fooTargetMock;
        private Mock<ITarget<PropertyInfo>> barTargetMock;
        private Mock<ITarget<PropertyInfo>> gooTargetMock;
        private Dummy instance = new Dummy();
        private int fooResolvedValue;
        private List<IPropertyInjectionDirective> directives;
        private IReadOnlyList<IParameter> parameters;
        private ActivationException activationException;

        public WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithoutPropertyValues_ResolveWithinOfPropertyTargetThrowsActivationException()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.barPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.gooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.gooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);

            this.fooResolvedValue = this.Random.Next();
            this.directives = new List<IPropertyInjectionDirective>
                {
                    fooPropertyDirectiveMock.Object,
                    gooPropertyDirectiveMock.Object,
                    barPropertyDirectiveMock.Object,
                };
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                };
            this.activationException = new ActivationException();

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.fooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.ContextMock.Object))
                              .Returns(fooResolvedValue);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.fooInjectorMock.Object);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooResolvedValue));

            #endregion Process Foo property injection directive

            #region Process goo property injection directive

            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.gooTargetMock.Object);
            this.gooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.ContextMock.Object))
                              .Throws(this.activationException);

            #endregion Process goo property injection directive
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.PlanMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            Assert.Same(this.activationException, actualException);
        }

        [Fact]
        public void ResolvesValuesForDirectivesWithoutPropertyValueParameter()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.fooTargetMock.Verify(p => p.ResolveWithin(this.ContextMock.Object), Times.Once);
            this.gooTargetMock.Verify(p => p.ResolveWithin(this.ContextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooResolvedValue), Times.Once);
        }
    }

    public class WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_PartialMatch : PropertyInjectionDirectiveContext
    {
        private Mock<PropertyInjector> fooInjectorMock;
        private Mock<PropertyInjector> barInjectorMock;
        private Mock<PropertyInjector> gooInjectorMock;
        private Mock<ITarget<PropertyInfo>> fooTargetMock;
        private Mock<ITarget<PropertyInfo>> barTargetMock;
        private Mock<ITarget<PropertyInfo>> gooTargetMock;
        private Mock<IPropertyInjectionDirective> fooPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> barPropertyDirectiveMock;
        private Mock<IPropertyInjectionDirective> gooPropertyDirectiveMock;
        private Mock<IPropertyValue> fooPropertyValueMock;
        private Mock<IPropertyValue> barPropertyValueMock;
        private Dummy instance = new Dummy();
        private int fooPropertyValue;
        private string gooResolvedValue;
        private string barPropertyValue;
        private List<IPropertyInjectionDirective> directives;
        private IReadOnlyList<IParameter> parameters;

        public WhenInitializeIsCalled_WithPropertyInjectionDirectivesAndWithPropertyValues_PartialMatch()
        {
            this.fooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.barInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.gooInjectorMock = new Mock<PropertyInjector>(MockBehavior.Strict);
            this.fooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.barTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.gooTargetMock = new Mock<ITarget<PropertyInfo>>(MockBehavior.Strict);
            this.fooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.barPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.gooPropertyDirectiveMock = new Mock<IPropertyInjectionDirective>(MockBehavior.Strict);
            this.fooPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);
            this.barPropertyValueMock = new Mock<IPropertyValue>(MockBehavior.Strict);

            this.fooPropertyValue = this.Random.Next();
            this.gooResolvedValue = this.Random.Next().ToString();
            this.barPropertyValue = this.Random.Next().ToString();
            this.directives = new List<IPropertyInjectionDirective>
                {
                    fooPropertyDirectiveMock.Object,
                    gooPropertyDirectiveMock.Object,
                    barPropertyDirectiveMock.Object,
                };
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    this.fooPropertyValueMock.Object,
                    this.barPropertyValueMock.Object
                };

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.ContextMock.Object, fooTargetMock.Object))
                                     .Returns(fooPropertyValue);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.fooInjectorMock.Object);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooPropertyValue));

            #endregion Process Foo property injection directive

            #region Process goo property injection directive

            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.gooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, gooTargetMock.Object))
                                     .Returns(false);
            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.gooTargetMock.Object);
            this.gooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.ContextMock.Object))
                              .Returns(this.gooResolvedValue);
            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.gooInjectorMock.Object);
            this.gooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.gooResolvedValue));

            #endregion Process goo property injection directive

            #region Process Bar property injection directive

            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.ContextMock.Object, barTargetMock.Object))
                                     .Returns(true);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.ContextMock.Object, barTargetMock.Object))
                                     .Returns(barPropertyValue);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.barInjectorMock.Object);
            this.barInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.barPropertyValue));

            #endregion Process goo property injection directive
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.PlanMock.Verify(x => x.GetProperties());
        }

        [Fact]
        public void ReturnsInstance()
        {
            var initialized = this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            Assert.Same(this.instance, initialized);
        }

        [Fact]
        public void GetValueFromPropertyForDirectivesWithPropertyValueParameter()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.fooPropertyValueMock.Verify(p => p.GetValue(this.ContextMock.Object, this.fooTargetMock.Object), Times.Once);
            this.barPropertyValueMock.Verify(p => p.GetValue(this.ContextMock.Object, this.barTargetMock.Object), Times.Once);
        }

        [Fact]
        public void ResolvesValuesForDirectivesWithoutPropertyValueParameter()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.gooTargetMock.Verify(p => p.ResolveWithin(this.ContextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooPropertyValue), Times.Once);
            this.gooInjectorMock.Verify(x => x(this.instance, this.gooResolvedValue), Times.Once);
            this.barInjectorMock.Verify(x => x(this.instance, this.barPropertyValue), Times.Once);
        }
    }

    public class WhenInitializeIsCalled_WithoutPropertyInjectionDirectivesAndWithoutPropertyValues : PropertyInjectionDirectiveContext
    {
        private Dummy instance;
        private InstanceReference reference;

        public WhenInitializeIsCalled_WithoutPropertyInjectionDirectivesAndWithoutPropertyValues()
        {
            IReadOnlyList<IParameter> parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B")
                };
            this.instance = new Dummy();

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(Array.Empty<IPropertyInjectionDirective>());
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(p => p.Parameters)
                            .Returns(parameters);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            this.PlanMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ReturnsInstance()
        {
            var initialized = this.Strategy.Initialize(this.ContextMock.Object, this.instance);

            Assert.Same(instance, initialized);
        }
    }

    public class WhenInitializeIsCalled_WithoutPropertyInjectionDirectivesAndWithPropertyValues : PropertyInjectionDirectiveContext
    {
        private Mock<IRequest> requestMock;
        private Dummy instance;
        private PropertyInfo fooProperty = typeof(Dummy).GetProperty("Foo");
        private PropertyInfo barProperty = typeof(Dummy).GetProperty("Bar");
        private int fooPropertyValue;
        private string barPropertyValue;
        private InstanceReference reference;
        private PropertyInjectionDirective[] directives;
        private IReadOnlyList<IParameter> parameters;
        private string exceptionMessage;

        public WhenInitializeIsCalled_WithoutPropertyInjectionDirectivesAndWithPropertyValues()
        {
            this.requestMock = new Mock<IRequest>(MockBehavior.Strict);

            this.fooPropertyValue = new Random().Next();
            this.barPropertyValue = new Random().Next().ToString();
            this.parameters = new List<IParameter>
                {
                    new ConstructorArgument("A", "B"),
                    new PropertyValue(this.fooProperty.Name, fooPropertyValue),
                    new PropertyValue(this.barProperty.Name, barPropertyValue)
                };
            this.instance = new Dummy();
            this.exceptionMessage = this.Random.Next().ToString();

            var mockSequence = new MockSequence();

            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.PlanMock.Object);
            this.PlanMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(Array.Empty<IPropertyInjectionDirective>());
            this.ContextMock.InSequence(mockSequence)
                            .SetupGet(p => p.Parameters)
                            .Returns(parameters);
            this.ContextMock.InSequence(mockSequence)
                            .Setup(p => p.Request)
                            .Returns(this.requestMock.Object);
            this.ExceptionFormatterMock.InSequence(mockSequence)
                                       .Setup(p => p.CouldNotResolvePropertyForValueInjection(this.requestMock.Object, this.fooProperty.Name))
                                       .Returns(this.exceptionMessage);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            this.PlanMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.Strategy.Initialize(this.ContextMock.Object, this.instance));

            Assert.Null(actualException.InnerException);
            Assert.Same(this.exceptionMessage, actualException.Message);
        }
    }

    public class Dummy
    {
        public int Foo { get; set; }
        public string Bar { get; set; }
        public string Goo { get; set; }
    }
}
