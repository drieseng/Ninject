//-------------------------------------------------------------------------------
// <copyright file="ConstructorReflectionSelectorTests.cs" company="Ninject Project Contributors">
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
    using Ninject.Selection;
    using System;
    using System.Reflection;
    using Xunit;

    public class ConstructorReflectionSelectorTests
    {
        private readonly ConstructorReflectionSelector _selector;

        public ConstructorReflectionSelectorTests()
        {
            _selector = new ConstructorReflectionSelector();
        }

        [Fact]
        public void InjectNonPublicShouldBeFalseByDefault()
        {
            Assert.False(_selector.InjectNonPublic);
        }

        [Fact]
        public void InjectNonPublicShouldReturnValueThatIsSet()
        {
            _selector.InjectNonPublic = true;
            Assert.True(_selector.InjectNonPublic);
            _selector.InjectNonPublic = false;
            Assert.False(_selector.InjectNonPublic);
        }

        [Fact]
        public void Select_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _selector.Select(type));
            Assert.Null(actualException.InnerException);
            Assert.Equal(nameof(type), actualException.ParamName);
        }

        [Fact]
        public void Select_InjectNonPublicIsTrue_ShouldReturnEmptyResultWhenTypeIsSubclassOfMulticastDelegate()
        {
            MyDelegate delegate1 = () => { };
            MyDelegate delegate2 = () => { };
            MyDelegate multiDelegate = delegate1 + delegate2;

            _selector.InjectNonPublic = true;

            Assert.Empty(_selector.Select(delegate1.GetType()));
            Assert.Empty(_selector.Select(multiDelegate.GetType()));
        }

        [Fact]
        public void Select_InjectNonPublicIsFalse_ShouldReturnEmptyResultWhenTypeIsSubclassOfMulticastDelegate()
        {
            MyDelegate delegate1 = () => { };
            MyDelegate delegate2 = () => { };
            MyDelegate multiDelegate = delegate1 + delegate2;

            Assert.Empty(_selector.Select(delegate1.GetType()));
            Assert.Empty(_selector.Select(multiDelegate.GetType()));
        }

        [Fact]
        public void Select_InjectNonPublicIsTrue_ShouldReturnPublicAndNonPublicInstanceConstructors()
        {
            var type = typeof(MyService);

            _selector.InjectNonPublic = true;

            var actual = _selector.Select(typeof(MyService));

            Assert.Equal(new[]
                            {
                                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(string) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(int) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(int), typeof(string) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(string), typeof(int) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new [] { typeof(Type) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new [] { typeof(Type), typeof(string) }, Array.Empty<ParameterModifier>())
                            },
                        actual);
        }

        [Fact]
        public void Select_InjectNonPublicIsFalse_ShouldOnlyReturnPublicConstructors()
        {
            var type = typeof(MyService);

            var actual = _selector.Select(type);

            Assert.Equal(new[]
                            {
                                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new [] { typeof(Type) }, Array.Empty<ParameterModifier>()),
                                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new [] { typeof(Type), typeof(string) }, Array.Empty<ParameterModifier>())
                            },
                        actual);
        }

        [Fact]
        public void Select_InjectNonPublicIsTrue_ShouldReturnEmptyResultWhenTypeIsStaticClass()
        {
            _selector.InjectNonPublic = true;

            var actual = _selector.Select(typeof(MyFactory));

            Assert.Empty(actual);
        }

        [Fact]
        public void Select_InjectNonPublicIsFalse_ShouldReturnEmptyResultWhenTypeIsStaticClass()
        {
            var actual = _selector.Select(typeof(MyFactory));

            Assert.Empty(actual);
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
        }

        public abstract class ServiceBase
        {
            public bool Active { get; set; }
        }


        public static class MyFactory
        {
            public static MyService Create()
            {
                return new MyService(typeof(string));
            }
        }

        private delegate void MyDelegate();
    }
}
