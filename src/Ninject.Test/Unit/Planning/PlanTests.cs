using Ninject.Planning;
using Ninject.Planning.Directives;
using System;
using System.Linq;
using Xunit;

namespace Ninject.Test.Unit.Planning
{
    public class PlanTests
    {
        private Plan _planMulti;
        private ConstructorInjectionDirective _constructor1;
        private ConstructorInjectionDirective _constructor2;
        private PropertyInjectionDirective _property1;
        private PropertyInjectionDirective _property2;
        private MethodInjectionDirective _method1;
        private MethodInjectionDirective _method2;
        private Plan _emptyPlan;
        private Plan _planSingle;

        public PlanTests()
        {
            _constructor1 = CreateConstructorInjectionDirective();
            _constructor2 = CreateConstructorInjectionDirective();
            _property1 = CreatePropertyInjectionDirective();
            _property2 = CreatePropertyInjectionDirective();
            _method1 = CreateMethodInjectionDirective();
            _method2 = CreateMethodInjectionDirective();

            _planMulti = new Plan(this.GetType());
            _planMulti.Add(_constructor1);
            _planMulti.Add(_property1);
            _planMulti.Add(_method1);
            _planMulti.Add(_constructor2);
            _planMulti.Add(_property2);
            _planMulti.Add(_method2);

            _planSingle = new Plan(this.GetType());
            _planSingle.Add(_constructor1);
            _planSingle.Add(_property1);
            _planSingle.Add(_method1);

            _emptyPlan = new Plan(this.GetType());
        }

        [Fact]
        public void Ctor_Type_ShouldThrowArgumentNullExceptionWhenTypeIsNull()
        {
            const Type type = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => new Plan(type));
            Assert.Equal(nameof(type), actualException.ParamName);
        }

        [Fact]
        public void Ctor_Type()
        {
            var plan = new Plan(typeof(MyService));

            Assert.Same(typeof(MyService), plan.Type);
            Assert.Empty(plan.Directives);
        }

        [Fact]
        public void Add_IConstructorInjectionDirective_ShouldThrowArgumentNullExceptionWhenDirectiveIsNull()
        {
            const IConstructorInjectionDirective constructor = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _planMulti.Add(constructor));
            Assert.Equal(nameof(constructor), actualException.ParamName);
        }

        [Fact]
        public void Add_IMethodInjectionDirective_ShouldThrowArgumentNullExceptionWhenDirectiveIsNull()
        {
            const IMethodInjectionDirective method = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _planMulti.Add(method));
            Assert.Equal(nameof(method), actualException.ParamName);
        }

        [Fact]
        public void Add_IPropertyInjectionDirective_ShouldThrowArgumentNullExceptionWhenDirectiveIsNull()
        {
            const IPropertyInjectionDirective property = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _planMulti.Add(property));
            Assert.Equal(nameof(property), actualException.ParamName);
        }

        [Fact]
        public void Add_IDirective_ShouldThrowArgumentNullExceptionWhenDirectiveIsNull()
        {
            const IDirective directive = null;

            var actualException = Assert.Throws<ArgumentNullException>(() => _planMulti.Add(directive));
            Assert.Equal(nameof(directive), actualException.ParamName);
        }

        [Fact]
        public void Has_ShouldReturnFalseWhenNoDirectiveOfSpecifiedTypeExists()
        {
            Assert.False(_emptyPlan.Has<MethodInjectionDirective>());
        }

        [Fact]
        public void Has_ShouldReturnTrueWhenAtLeastOneDirectiveOfSpecifiedTypeExists()
        {
            Assert.True(_planMulti.Has<ConstructorInjectionDirective>());
            Assert.True(_planMulti.Has<PropertyInjectionDirective>());
        }

        [Fact]
        public void GetOne_ShouldReturnDirectiveOfSpecifiedTypeWhenOnlyOneDirectiveOfSpecifiedTypeExists()
        {
            var actual = _planSingle.GetOne<PropertyInjectionDirective>();

            Assert.NotNull(actual);
            Assert.Same(_property1, actual);
        }

        [Fact]
        public void GetOne_ShouldReturnNullWhenNoDirectiveOfSpecifiedTypeExists()
        {
            Assert.Null(_emptyPlan.GetOne<MethodInjectionDirective>());
        }

        [Fact]
        public void GetOne_ShoulThrowInvalidOperationExceptionWhenMoreThanOneDirectiveOfSpecifiedTypeExists()
        {
            Assert.Throws<InvalidOperationException>(() => _planMulti.GetOne<ConstructorInjectionDirective>());
        }

        [Fact]
        public void GetConstructors_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetConstructors();

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count);
            Assert.Same(_constructor1, actual[0]);
            Assert.Same(_constructor2, actual[1]);
        }

        [Fact]
        public void GetConstructors_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetConstructors();
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Count);
        }

        [Fact]
        public void GetAll_ConstructorInjectionDirective_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetAll<ConstructorInjectionDirective>();

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(_constructor1, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(_constructor2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_ConstructorInjectionDirective_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetAll<ConstructorInjectionDirective>();
            Assert.False(actual.Any());
        }

        [Fact]
        public void GetAll_IConstructorInjectionDirective_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetAll<IConstructorInjectionDirective>();

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(_constructor1, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(_constructor2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_IConstructorInjectionDirective_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetAll<IConstructorInjectionDirective>();
            Assert.False(actual.Any());
        }

        [Fact]
        public void GetMethods_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetMethods();

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count);
            Assert.Same(_method1, actual[0]);
            Assert.Same(_method2, actual[1]);
        }

        [Fact]
        public void GetMethods_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetMethods();
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Count);
        }

        [Fact]
        public void GetAll_MethodInjectionDirective_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetAll<MethodInjectionDirective>();

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(_method1, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(_method2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_MethodInjectionDirective_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetAll<MethodInjectionDirective>();
            Assert.False(actual.Any());
        }

        [Fact]
        public void GetAll_IMethodInjectionDirective_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetAll<IMethodInjectionDirective>();

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(_method1, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(_method2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_IMethodInjectionDirective_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetAll<IMethodInjectionDirective>();
            Assert.False(actual.Any());
        }

        [Fact]
        public void GetProperties_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetProperties();

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count);
            Assert.Same(_property1, actual[0]);
            Assert.Same(_property2, actual[1]);
        }

        [Fact]
        public void GetProperties_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetProperties();
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Count);
        }

        [Fact]
        public void GetAll_PropertyInjectionDirective_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetAll<PropertyInjectionDirective>();

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(_property1, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(_property2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_PropertyInjectionDirective_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetAll<PropertyInjectionDirective>();
            Assert.False(actual.Any());
        }

        [Fact]
        public void GetAll_IPropertyInjectionDirective_MatchingDirectivesAvailable()
        {
            var actual = _planMulti.GetAll<IPropertyInjectionDirective>();

            using (var enumerator = actual.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(_property1, enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(_property2, enumerator.Current);

                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void GetAll_IPropertyInjectionDirective_NoMatchingDirectivesAvailable()
        {
            var actual = _emptyPlan.GetAll<IPropertyInjectionDirective>();
            Assert.False(actual.Any());
        }

        private static ConstructorInjectionDirective CreateConstructorInjectionDirective()
        {
            return new ConstructorInjectionDirective(typeof(MyService).GetConstructor(new Type[0]), (_) => null);
        }

        private static PropertyInjectionDirective CreatePropertyInjectionDirective()
        {
            return new PropertyInjectionDirective(typeof(MyService).GetProperty("Name"), (target, value) => { });
        }

        private static MethodInjectionDirective CreateMethodInjectionDirective()
        {
            return new MethodInjectionDirective(typeof(MyService).GetMethod("Run"), (target, arguments) => { });
        }

        public class MyService
        {
            public MyService()
            {
            }

            public string Name { get; }

            public void Run()
            {
            }
        }
    }
}
