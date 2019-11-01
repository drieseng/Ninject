namespace Ninject.Tests.Unit.Selection
{
    using Ninject.Selection;
    using Ninject.Tests.Fakes;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    public class PropertyReflectionSelectorTests
    {
        [Fact]
        public void InjectNonPublicShouldReturnValueThatIsSet()
        {
            PropertyReflectionSelector selector;
            
            selector = new PropertyReflectionSelector(true);
            Assert.True(selector.InjectNonPublic);

            selector = new PropertyReflectionSelector(false);
            Assert.False(selector.InjectNonPublic);
        }

        [Fact]
        public void Select_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;
            var selector = new PropertyReflectionSelector(true);

            var actual = Assert.Throws<ArgumentNullException>(() => selector.Select(type));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(type), actual.ParamName);
        }

        [Fact]
        public void Select_InjectNonPublicIsTrue_ShouldNotSelectPropertiesThatHaveNoSetter()
        {
            #region Arrange

            var type = typeof(MyService);
            var enabledProperty = type.GetProperty(nameof(MyService.Enabled), BindingFlags.Public | BindingFlags.Instance);
            var selector = new PropertyReflectionSelector(true);

            #endregion Arrange

            var actual = new List<PropertyInfo>(selector.Select(type));

            Assert.DoesNotContain(enabledProperty, actual);
        }

        [Fact]
        public void Select_InjectNonPublicIsTrue_ShouldReturnPublicAndNonPublicInstanceProperties()
        {
            #region Arrange

            var type = typeof(MyService);
            var weaponProperty = type.GetProperty("Weapon", BindingFlags.NonPublic | BindingFlags.Instance);
            var idProperty = type.GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
            var nameProperty = type.GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance);
            var visibleProperty = type.GetProperty("Visible", BindingFlags.Public | BindingFlags.Instance);
            var stopProperty = type.GetProperty("Stop", BindingFlags.Public | BindingFlags.Instance);
            var selector = new PropertyReflectionSelector(true);

            #endregion Arrange

            var actual = new List<PropertyInfo>(selector.Select(type));

            Assert.Equal(5, actual.Count);
            Assert.Contains(weaponProperty, actual);
            Assert.Contains(idProperty, actual);
            Assert.Contains(nameProperty, actual);
            Assert.Contains(visibleProperty, actual);
            Assert.Contains(stopProperty, actual);
        }

        [Fact]
        public void Select_InjectNonPublicIsFalse_ShouldNotSelectPropertiesThatHaveNoSetter()
        {
            #region Arrange

            var type = typeof(MyService);
            var enabledProperty = type.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance);
            var selector = new PropertyReflectionSelector(false);

            #endregion Arrange

            var actual = new List<PropertyInfo>(selector.Select(type));

            Assert.DoesNotContain(enabledProperty, actual);
        }

        [Fact]
        public void Select_InjectNonPublicIsFalse_ShouldReturnPublicPropertiesDeclaredOnSpecifiedType()
        {
            #region Arrange

            var type = typeof(MyService);
            var visibleProperty = type.GetProperty("Visible", BindingFlags.Public | BindingFlags.Instance);
            var stopProperty = type.GetProperty("Stop", BindingFlags.Public | BindingFlags.Instance);
            var selector = new PropertyReflectionSelector(false);

            #endregion Arrange

            var actual = new List<PropertyInfo>(selector.Select(type));

            Assert.Equal(2, actual.Count);
            Assert.Contains(stopProperty, actual);
            Assert.Contains(visibleProperty, actual);
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

            internal string Name { get; private set; }

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
