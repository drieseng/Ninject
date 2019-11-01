//-------------------------------------------------------------------------------
// <copyright file="MethodReflectionSelectorTests.cs" company="Ninject Project Contributors">
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

namespace Ninject.Tests.Unit.Selection
{
    using Moq;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;
    using Ninject.Tests.Fakes;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    public class MethodReflectionSelectorTests
    {
        private Mock<IMethodInjectionHeuristic> _injectionHeuristicMock1;
        private Mock<IMethodInjectionHeuristic> _injectionHeuristicMock2;

        public MethodReflectionSelectorTests()
        {
            _injectionHeuristicMock1 = new Mock<IMethodInjectionHeuristic>(MockBehavior.Strict);
            _injectionHeuristicMock2 = new Mock<IMethodInjectionHeuristic>(MockBehavior.Strict);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenInjectionHeuristicsIsNull()
        {
            const IEnumerable<IMethodInjectionHeuristic> injectionHeuristics = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new MethodReflectionSelector(injectionHeuristics, true));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(injectionHeuristics), actual.ParamName);
        }

        [Fact]
        public void InjectNonPublicShouldReturnValueThatIsSet()
        {
            MethodReflectionSelector selector;

            selector = new MethodReflectionSelector(Array.Empty<IMethodInjectionHeuristic>(), true);
            Assert.True(selector.InjectNonPublic);

            selector = new MethodReflectionSelector(Array.Empty<IMethodInjectionHeuristic>(), false);
            Assert.False(selector.InjectNonPublic);
        }

        [Fact]
        public void Select_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;
            var selector = new MethodReflectionSelector(Array.Empty<IMethodInjectionHeuristic>(), true);

            var actual = Assert.Throws<ArgumentNullException>(() => selector.Select(type));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(type), actual.ParamName);
        }

        [Fact]
        public void Select_InjectNonPublicIsTrue_ShouldReturnPublicAndNonPublicInstanceMethodsThatAreEligibleForInjection()
        {
            #region Arrange

            var type = typeof(MyService);
            var getWeaponMethod = type.GetMethod("get_Weapon", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setWeaponMethod = type.GetMethod("set_Weapon", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(IWeapon) }, Array.Empty<ParameterModifier>());
            var setIdMethod = type.GetMethod("set_Id", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(int) }, Array.Empty<ParameterModifier>());
            var getNameMethod = type.GetMethod("get_Name", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var getEnabledMethod = type.GetMethod("get_Enabled", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setStopMethod = type.GetMethod("set_Stop", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, Array.Empty<ParameterModifier>());
            var getVisibleMethod = type.GetMethod("get_Visible", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setVisibleMethod = type.GetMethod("set_Visible", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, Array.Empty<ParameterModifier>());
            var doMethodNoArgs = type.GetMethod("Do", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var doMethodStringArg = type.GetMethod("Do", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, Array.Empty<ParameterModifier>());
            var doMethodStringAndInt32Args = type.GetMethod("Do", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string), typeof(int) }, Array.Empty<ParameterModifier>());
            var executeMethodNoArgs = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var executeMethodStringArg = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, Array.Empty<ParameterModifier>());
            var executeMethodStringAndInt32Arg = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(int) }, Array.Empty<ParameterModifier>());
            var toStringMethod = type.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var equalsMethod = type.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(object) }, Array.Empty<ParameterModifier>());
            var getHashCodeMethod = type.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var getTypeMethod = type.GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var finalizeMethod = type.GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var memberwiseCloneMethod = type.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var selector = new MethodReflectionSelector(new IMethodInjectionHeuristic[] { _injectionHeuristicMock1.Object, _injectionHeuristicMock2.Object }, true);

            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getWeaponMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getWeaponMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setWeaponMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setWeaponMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setIdMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setIdMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getNameMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getNameMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getEnabledMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getEnabledMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setStopMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setStopMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getVisibleMethod)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setVisibleMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setVisibleMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(doMethodNoArgs)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(doMethodStringArg)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(doMethodStringArg)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(doMethodStringAndInt32Args)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(doMethodStringAndInt32Args)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodNoArgs)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(executeMethodNoArgs)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodStringArg)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodStringAndInt32Arg)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(executeMethodStringAndInt32Arg)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(toStringMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(toStringMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(equalsMethod)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getHashCodeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getHashCodeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getTypeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getTypeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(finalizeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(finalizeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(memberwiseCloneMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(memberwiseCloneMethod)).Returns(false);

            #endregion Arrange

            var actual = new List<MethodInfo>(selector.Select(type));

            Assert.Equal(6, actual.Count);
            Assert.Contains(getVisibleMethod, actual);
            Assert.Contains(doMethodNoArgs, actual);
            Assert.Contains(doMethodStringArg, actual);
            Assert.Contains(executeMethodStringArg, actual);
            Assert.Contains(executeMethodStringAndInt32Arg, actual);
            Assert.Contains(equalsMethod, actual);
        }

        [Fact]
        public void Select_InjectNonPublicIsFalse_ShouldReturnPublicAndNonPublicInstanceMethodsThatAreEligibleForInjection()
        {
            #region Arrange

            var type = typeof(MyService);
            var getEnabledMethod = type.GetMethod("get_Enabled", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setStopMethod = type.GetMethod("set_Stop", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, Array.Empty<ParameterModifier>());
            var getVisibleMethod = type.GetMethod("get_Visible", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var setVisibleMethod = type.GetMethod("set_Visible", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, Array.Empty<ParameterModifier>());
            var executeMethodNoArgs = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var executeMethodStringArg = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, Array.Empty<ParameterModifier>());
            var executeMethodStringAndInt32Arg = type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(int) }, Array.Empty<ParameterModifier>());
            var toStringMethod = type.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var equalsMethod = type.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(object) }, Array.Empty<ParameterModifier>());
            var getHashCodeMethod = type.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var getTypeMethod = type.GetMethod("GetType", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var memberwiseCloneMethod = type.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>());
            var selector = new MethodReflectionSelector(new IMethodInjectionHeuristic[] { _injectionHeuristicMock1.Object, _injectionHeuristicMock2.Object }, false);

            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getEnabledMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getEnabledMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setStopMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setStopMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getVisibleMethod)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(setVisibleMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(setVisibleMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodNoArgs)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodStringArg)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(executeMethodStringArg)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(executeMethodStringAndInt32Arg)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(executeMethodStringAndInt32Arg)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(toStringMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(toStringMethod)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(equalsMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(equalsMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getHashCodeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getHashCodeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(getTypeMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(getTypeMethod)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(memberwiseCloneMethod)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(memberwiseCloneMethod)).Returns(false);

            #endregion Arrange

            var actual = selector.Select(type);

            Assert.Equal(new[]
                            {
                                getVisibleMethod,
                                executeMethodNoArgs,
                                executeMethodStringArg,
                                toStringMethod
                            },
                        actual);
        }

        public class MyService
        {
            static MyService()
            {
            }

            private MyService(string name)
            {
            }

            protected MyService(int iterations)
            {
            }

            protected internal MyService(int iterations, string name)
            {
            }

            internal MyService(string name, int iterations)
            {
            }

            public MyService(Type type)
            {
            }

            public MyService(Type type, string name)
            {
            }

            private IWeapon Weapon { get; set; }

            protected int Id
            {
                set { }
            }

            internal string Name { get; }

            public bool Enabled { get; }

            public bool Stop
            {
                set { }
            }

            public bool Visible { get; set; }

            public static string ExecuteCount { get; set; }

            private void Do()
            {
            }

            protected void Do(string action)
            {
            }

            internal void Do(string action, int iterations)
            {
            }

            public void Execute()
            {
            }

            public void Execute(string name)
            {
            }

            public void Execute(string name, int iterations)
            {
            }

            public static MyService Create()
            {
                return new MyService(5);
            }

            private static MyService Create(int iterations)
            {
                return new MyService(iterations);
            }
        }
    }
}
