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
    using Ninject.Activation.Providers;
    using Ninject.Components;
    using Ninject.Injection;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;

    public class PropertyInjectionDirectiveContext
    {
        protected Mock<IExceptionFormatter> exceptionFormatterMock { get; private set; }
        protected Mock<IContext> contextMock { get; private set; }
        protected Mock<IPlan> planMock { get; private set; }
        protected Random random { get; private set; }
        protected readonly PropertyInjectionStrategy strategy;

        public PropertyInjectionDirectiveContext()
        {
            this.exceptionFormatterMock = new Mock<IExceptionFormatter>(MockBehavior.Strict);
            this.contextMock = new Mock<IContext>(MockBehavior.Strict);
            this.planMock = new Mock<IPlan>(MockBehavior.Strict);

            this.random = new Random();

            this.strategy = new PropertyInjectionStrategy(this.exceptionFormatterMock.Object);
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

            this.directives = new List<IPropertyInjectionDirective>
                {
                    fooPropertyDirectiveMock.Object,
                    barPropertyDirectiveMock.Object
                };

            this.fooResolvedValue = this.random.Next();
            this.barResolvedValue = this.random.Next().ToString();

            var mockSequence = new MockSequence();

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(Array.Empty<IParameter>());
            this.fooPropertyDirectiveMock.Setup(p => p.Target).Returns(fooTargetMock.Object);
            this.fooTargetMock.Setup(p => p.ResolveWithin(this.contextMock.Object)).Returns(fooResolvedValue);
            this.fooPropertyDirectiveMock.Setup(p => p.Injector).Returns(fooInjectorMock.Object);
            this.fooInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.fooResolvedValue));
            this.barPropertyDirectiveMock.Setup(p => p.Target).Returns(barTargetMock.Object);
            this.barTargetMock.Setup(p => p.ResolveWithin(this.contextMock.Object)).Returns(barResolvedValue);
            this.barPropertyDirectiveMock.Setup(p => p.Injector).Returns(barInjectorMock.Object);
            this.barInjectorMock.InSequence(mockSequence)
                                .Setup(p => p(this.instance, this.barResolvedValue));
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.planMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ReturnsInstance()
        {
            var initialized = this.strategy.Initialize(this.contextMock.Object, this.instance);

            Assert.Same(this.instance, initialized);
        }

        [Fact]
        public void ResolvesValuesForEachTargetOfEachDirective()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.fooPropertyDirectiveMock.Verify(p => p.Target, Times.Once);
            this.fooTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);

            this.barPropertyDirectiveMock.Verify(p => p.Target, Times.Once);
            this.barTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

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

            this.fooPropertyValue = this.random.Next();
            this.barPropertyValue = this.random.Next().ToString();
            this.gooPropertyValue = this.random.Next().ToString();

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

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.contextMock.Object, this.fooTargetMock.Object))
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
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.barTargetMock.Object))
                                     .Returns(true);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.barTargetMock.Object))
                                     .Returns(false);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.contextMock.Object, this.barTargetMock.Object))
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
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.barTargetMock.Object))
                                     .Returns(true);
            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.gooTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.contextMock.Object, this.gooTargetMock.Object))
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
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.planMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

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
            this.exceptionMessage = this.random.Next().ToString();

            var mockSequence = new MockSequence();

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValue1Mock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValue2Mock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.exceptionFormatterMock.InSequence(mockSequence)
                                       .Setup(p => p.MoreThanOnePropertyValueForTarget(this.contextMock.Object, fooTargetMock.Object))
                                       .Returns(this.exceptionMessage);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            this.planMock.Verify(x => x.GetProperties());
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            Assert.Null(actualException.InnerException);
            Assert.Same(this.exceptionMessage, actualException.Message);
        }

        [Fact]
        public void VerifiesForEachPropertyValueWhetherItAppliesToDirective()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            this.fooPropertyValue1Mock.Verify(p => p.AppliesToTarget(this.contextMock.Object, this.fooTargetMock.Object), Times.Once);
            this.fooPropertyValue2Mock.Verify(p => p.AppliesToTarget(this.contextMock.Object, this.fooTargetMock.Object), Times.Once);
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
        private Dummy instance = new Dummy();
        private int fooPropertyValue;
        private string barPropertyValue;
        private string gooPropertyValue;
        private List<IPropertyInjectionDirective> directives;
        private IReadOnlyList<IParameter> parameters;

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
            this.fooPropertyValue = this.random.Next();
            this.barPropertyValue = this.random.Next().ToString();
            this.gooPropertyValue = this.random.Next().ToString();
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

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
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
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.barTargetMock.Object))
                                     .Returns(true);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.gooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, this.barTargetMock.Object))
                                     .Returns(false);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.contextMock.Object, this.barTargetMock.Object))
                                     .Returns(this.barPropertyValue);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Injector)
                                         .Returns(this.barInjectorMock.Object);
            this.barInjectorMock.InSequence(mockSequence)
                                 .Setup(p => p(this.instance, this.barPropertyValue));

            #endregion Process Bar property injection directive
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.planMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.fooInjectorMock.Verify(x => x(this.instance, this.fooPropertyValue), Times.Once);
            this.barInjectorMock.Verify(x => x(this.instance, this.barPropertyValue), Times.Once);
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

            this.fooPropertyValue = this.random.Next();
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

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.contextMock.Object, fooTargetMock.Object))
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
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, gooTargetMock.Object))
                                     .Returns(false);
            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.gooTargetMock.Object);
            this.gooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
                              .Throws(this.activationException);

            #endregion Process goo property injection directive
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            this.planMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            Assert.Same(this.activationException, actualException);
        }

        [Fact]
        public void GetValueFromPropertyForDirectivesWithPropertyValueParameter()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            this.fooPropertyValueMock.Verify(p => p.GetValue(this.contextMock.Object, this.fooTargetMock.Object), Times.Once);
        }

        [Fact]
        public void ResolvesValuesForDirectivesWithoutPropertyValueParameter()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            this.gooTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

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

            this.fooResolvedValue = this.random.Next();
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

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.fooTargetMock.Object);
            this.fooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
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
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
                              .Throws(this.activationException);

            #endregion Process goo property injection directive
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            this.planMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            Assert.Same(this.activationException, actualException);
        }

        [Fact]
        public void ResolvesValuesForDirectivesWithoutPropertyValueParameter()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            this.fooTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);
            this.gooTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

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

            this.fooPropertyValue = this.random.Next();
            this.gooResolvedValue = this.random.Next().ToString();
            this.barPropertyValue = this.random.Next().ToString();
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

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(this.directives);
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Parameters)
                            .Returns(this.parameters);

            #region Process Foo property injection directive

            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, fooTargetMock.Object))
                                     .Returns(true);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, fooTargetMock.Object))
                                     .Returns(false);
            this.fooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(fooTargetMock.Object);
            this.fooPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.contextMock.Object, fooTargetMock.Object))
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
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, gooTargetMock.Object))
                                     .Returns(false);
            this.gooPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(this.gooTargetMock.Object);
            this.gooTargetMock.InSequence(mockSequence)
                              .Setup(p => p.ResolveWithin(this.contextMock.Object))
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
                                     .Setup(p => p.AppliesToTarget(this.contextMock.Object, barTargetMock.Object))
                                     .Returns(true);
            this.barPropertyDirectiveMock.InSequence(mockSequence)
                                         .Setup(p => p.Target)
                                         .Returns(barTargetMock.Object);
            this.barPropertyValueMock.InSequence(mockSequence)
                                     .Setup(p => p.GetValue(this.contextMock.Object, barTargetMock.Object))
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
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.planMock.Verify(x => x.GetProperties());
        }

        [Fact]
        public void ReturnsInstance()
        {
            var initialized = this.strategy.Initialize(this.contextMock.Object, this.instance);

            Assert.Same(this.instance, initialized);
        }

        [Fact]
        public void GetValueFromPropertyForDirectivesWithPropertyValueParameter()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.fooPropertyValueMock.Verify(p => p.GetValue(this.contextMock.Object, this.fooTargetMock.Object), Times.Once);
            this.barPropertyValueMock.Verify(p => p.GetValue(this.contextMock.Object, this.barTargetMock.Object), Times.Once);
        }

        [Fact]
        public void ResolvesValuesForDirectivesWithoutPropertyValueParameter()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.gooTargetMock.Verify(p => p.ResolveWithin(this.contextMock.Object), Times.Once);
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

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

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(Array.Empty<IPropertyInjectionDirective>());
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(p => p.Parameters)
                            .Returns(parameters);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Initialize(this.contextMock.Object, this.instance);

            this.planMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ReturnsInstance()
        {
            var initialized = this.strategy.Initialize(this.contextMock.Object, this.instance);

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
            this.exceptionMessage = this.random.Next().ToString();

            var mockSequence = new MockSequence();

            this.contextMock.InSequence(mockSequence)
                            .SetupGet(x => x.Plan)
                            .Returns(this.planMock.Object);
            this.planMock.InSequence(mockSequence)
                         .Setup(x => x.GetProperties())
                         .Returns(Array.Empty<IPropertyInjectionDirective>());
            this.contextMock.InSequence(mockSequence)
                            .SetupGet(p => p.Parameters)
                            .Returns(parameters);
            this.contextMock.InSequence(mockSequence)
                            .Setup(p => p.Request)
                            .Returns(this.requestMock.Object);
            this.exceptionFormatterMock.InSequence(mockSequence)
                                       .Setup(p => p.CouldNotResolvePropertyForValueInjection(this.requestMock.Object, this.fooProperty.Name))
                                       .Returns(this.exceptionMessage);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

            this.planMock.Verify(x => x.GetProperties(), Times.Once);
        }

        [Fact]
        public void ThrowsActivationException()
        {
            var actualException = Assert.Throws<ActivationException>(() => this.strategy.Initialize(this.contextMock.Object, this.instance));

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
