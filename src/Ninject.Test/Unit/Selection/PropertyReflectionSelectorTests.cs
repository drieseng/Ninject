namespace Ninject.Tests.Unit.Selection
{
    using Moq;
    using Ninject.Selection;
    using Ninject.Tests.Fakes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class PropertyReflectionSelectorTests
    {
        private Mock<IPropertyInjectionHeuristic> _injectionHeuristicMock1;
        private Mock<IPropertyInjectionHeuristic> _injectionHeuristicMock2;
        private PropertyReflectionSelector _selector;

        public PropertyReflectionSelectorTests()
        {
            _injectionHeuristicMock1 = new Mock<IPropertyInjectionHeuristic>(MockBehavior.Strict);
            _injectionHeuristicMock2 = new Mock<IPropertyInjectionHeuristic>(MockBehavior.Strict);
            _selector = new PropertyReflectionSelector(new IPropertyInjectionHeuristic[] { _injectionHeuristicMock1.Object, _injectionHeuristicMock2.Object });
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullExceptionWhenInjectionHeuristicsIsNull()
        {
            const IEnumerable<IPropertyInjectionHeuristic> injectionHeuristics = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new PropertyReflectionSelector(injectionHeuristics));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(injectionHeuristics), actual.ParamName);
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

            var actual = Assert.Throws<ArgumentNullException>(() => _selector.Select(type));

            Assert.Null(actual.InnerException);
            Assert.Equal(nameof(type), actual.ParamName);
        }

        [Fact]
        public void Select_InjectNonPublicIsTrue_ShouldReturnPublicAndNonPublicInstanceMethodsThatAreEligibleForInjection()
        {
            #region Arrange

            var type = typeof(MyService);
            var weaponProperty = type.GetProperty("Weapon", BindingFlags.NonPublic | BindingFlags.Instance);
            var idProperty = type.GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
            var nameProperty = type.GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance);
            var activeProperty = type.GetProperty("Active", BindingFlags.Public | BindingFlags.Instance);
            var enabledProperty = type.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance);
            var visibleProperty = type.GetProperty("Visible", BindingFlags.Public | BindingFlags.Instance);
            var stopProperty = type.GetProperty("Stop", BindingFlags.Public | BindingFlags.Instance);

            _injectionHeuristicMock1.Setup(p => p.ShouldInject(weaponProperty))
                                    .Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(idProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(idProperty))
                                    .Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(nameProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(nameProperty))
                                    .Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(enabledProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(enabledProperty))
                                    .Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(visibleProperty))
                                    .Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(visibleProperty))
                                    .Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(stopProperty))
                                    .Returns(true);

            _selector.InjectNonPublic = true;

            #endregion Arrange

            var actual = _selector.Select(type);

            var actualList = actual.ToList();

            Assert.Equal(2, actualList.Count);
            Assert.Contains(weaponProperty, actualList);
            Assert.Contains(stopProperty, actualList);

            _injectionHeuristicMock1.Verify(p => p.ShouldInject(weaponProperty), Times.Once);
            _injectionHeuristicMock1.Verify(p => p.ShouldInject(idProperty), Times.Once);
            _injectionHeuristicMock2.Verify(p => p.ShouldInject(idProperty), Times.Once);
            _injectionHeuristicMock1.Verify(p => p.ShouldInject(nameProperty), Times.Once);
            _injectionHeuristicMock2.Verify(p => p.ShouldInject(nameProperty), Times.Once);
            _injectionHeuristicMock1.Verify(p => p.ShouldInject(enabledProperty), Times.Once);
            _injectionHeuristicMock2.Verify(p => p.ShouldInject(enabledProperty), Times.Once);
            _injectionHeuristicMock1.Verify(p => p.ShouldInject(visibleProperty), Times.Once);
            _injectionHeuristicMock2.Verify(p => p.ShouldInject(visibleProperty), Times.Once);
            _injectionHeuristicMock1.Verify(p => p.ShouldInject(stopProperty), Times.Once);
        }

        [Fact]
        public void Select_InjectNonPublicIsFalse_ShouldReturnPublicPropertiesDeclaradOnSpecifiedTypeThatAreEligibleForInjection()
        {
            #region Arrange

            var type = typeof(MyService);
            var enabledProperty = type.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance);
            var visibleProperty = type.GetProperty("Visible", BindingFlags.Public | BindingFlags.Instance);
            var stopProperty = type.GetProperty("Stop", BindingFlags.Public | BindingFlags.Instance);

            _injectionHeuristicMock1.Setup(p => p.ShouldInject(enabledProperty)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(enabledProperty)).Returns(false);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(stopProperty)).Returns(true);
            _injectionHeuristicMock1.Setup(p => p.ShouldInject(visibleProperty)).Returns(false);
            _injectionHeuristicMock2.Setup(p => p.ShouldInject(visibleProperty)).Returns(true);

            #endregion Arrange

            var actual = _selector.Select(type).ToList();

            Assert.Equal(2, actual.Count);
            Assert.Contains(stopProperty, actual);
            Assert.Contains(visibleProperty, actual);

            _injectionHeuristicMock1.Verify(p => p.ShouldInject(enabledProperty), Times.Once);
            _injectionHeuristicMock1.Verify(p => p.ShouldInject(stopProperty), Times.Once);
            _injectionHeuristicMock1.Verify(p => p.ShouldInject(visibleProperty), Times.Once);
            _injectionHeuristicMock2.Verify(p => p.ShouldInject(enabledProperty), Times.Once);
            _injectionHeuristicMock2.Verify(p => p.ShouldInject(visibleProperty), Times.Once);
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
