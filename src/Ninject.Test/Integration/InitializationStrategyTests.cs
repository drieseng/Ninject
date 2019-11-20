namespace Ninject.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Ninject.Activation;
    using Ninject.Activation.Strategies;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class InitializationStrategyTests : IDisposable
    {
        private readonly StandardKernel kernel;

        public InitializationStrategyTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void InitializationActions_WithoutContext_ExecutedWhenInstanceIsCreated()
        {
            int barracksInitializationCount = 0;

            this.kernel.Bind<Barracks>().ToSelf().OnInitialization(
                instance =>
                {
                    instance.Warrior = new FootSoldier();
                    instance.Weapon = new Shuriken();
                    barracksInitializationCount++;
                });

            var barracks = this.kernel.Get<Barracks>();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().BeOfType<Shuriken>();
            barracksInitializationCount.Should().Be(1);

            this.kernel.Get<Barracks>();
            barracksInitializationCount.Should().Be(2);
        }

        [Fact]
        public void InitializationActions_WithContext_ExecutedWhenInstanceIsCreated()
        {
            int warriorInitializationCount = 0;
            IContext activationContext = null;

            this.kernel.Bind<IWarrior>().To<Ninja>().OnInitialization(
                (ctx, instance) =>
                {
                    instance.Weapon = new Sword();
                    warriorInitializationCount++;
                    activationContext = ctx;
                });
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var warrior = this.kernel.Get<IWarrior>();
            warrior.Should().NotBeNull();
            warrior.Weapon.Should().BeOfType<Sword>();
            warriorInitializationCount.Should().Be(1);
            activationContext.Should().NotBeNull();
            activationContext.Request.Service.Should().Be<IWarrior>();

            this.kernel.Get<IWarrior>();
            warriorInitializationCount.Should().Be(2);
        }

        [Fact]
        public void InitializeIsNotInvokedForServiceBoundWithToMethod()
        {
            this.kernel.Components.Add<IInitializationStrategy, TestInitializationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().ToMethod(ctx => new Sword());
            var testInitializationStrategy = this.kernel.Components.GetAll<IInitializationStrategy>().OfType<TestInitializationStrategy>().Single();

            var warrior = this.kernel.Get<IWarrior>();

            testInitializationStrategy.InitializationCount.Should().Be(1);
            testInitializationStrategy.InitializedInstances.Count.Should().Be(1);
            testInitializationStrategy.InitializedInstances.Should().ContainInOrder(warrior);
        }

        [Fact]
        public void InitializeIsInvokedWhenKernelIsUsedToObtainInstanceInToMethod()
        {
            this.kernel.Components.Add<IInitializationStrategy, TestInitializationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<Sword>().ToSelf();
            this.kernel.Bind<IWeapon>().ToMethod(ctx => ctx.Kernel.Get<Sword>());
            var testInitializationStrategy = this.kernel.Components.GetAll<IInitializationStrategy>().OfType<TestInitializationStrategy>().Single();

            var warrior = this.kernel.Get<IWarrior>();

            testInitializationStrategy.InitializationCount.Should().Be(2);
            testInitializationStrategy.InitializedInstances.Count.Should().Be(2);
            testInitializationStrategy.InitializedInstances.Should().ContainInOrder(warrior.Weapon, warrior);
        }

        [Fact]
        public void InitializeIsInvokedWhenProviderReturnsInstanceThatIsNotInitialized()
        {
            this.kernel.Components.Add<IInitializationStrategy, TestInitializationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<Sword>().ToSelf();
            this.kernel.Bind<IWeapon>().ToProvider(new NotInitializedTestProvider());
            var testInitializationStrategy = this.kernel.Components.GetAll<IInitializationStrategy>().OfType<TestInitializationStrategy>().Single();

            var warrior = this.kernel.Get<IWarrior>();

            testInitializationStrategy.InitializationCount.Should().Be(2);
            testInitializationStrategy.InitializedInstances.Count.Should().Be(2);
            testInitializationStrategy.InitializedInstances.Should().ContainInOrder(warrior.Weapon, warrior);
        }

        [Fact]
        public void InitializeIsNotInvokedWhenProviderReturnsInstanceThatIsInitialized()
        {
            this.kernel.Components.Add<IInitializationStrategy, TestInitializationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>();
            //this.kernel.Bind<Sword>().ToSelf();
            this.kernel.Bind<IWeapon>().ToProvider(new InitializedTestProvider());
            var testInitializationStrategy = this.kernel.Components.GetAll<IInitializationStrategy>().OfType<TestInitializationStrategy>().Single();

            var warrior = this.kernel.Get<IWarrior>();

            testInitializationStrategy.InitializationCount.Should().Be(1);
            testInitializationStrategy.InitializedInstances.Count.Should().Be(1);
            testInitializationStrategy.InitializedInstances.Should().ContainInOrder(warrior);
        }

        [Fact]
        public void NullIsNotInitialized()
        {
            this.kernel.Settings.AllowNullInjection = true;
            this.kernel.Components.Add<IInitializationStrategy, TestInitializationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>().InSingletonScope();
            this.kernel.Bind<IWeapon>().ToConstant((IWeapon)null);
            var testInitializationStrategy = this.kernel.Components.GetAll<IInitializationStrategy>().OfType<TestInitializationStrategy>().Single();

            var warrior = this.kernel.Get<IWarrior>();

            testInitializationStrategy.InitializationCount.Should().Be(1);
            testInitializationStrategy.InitializedInstances.Count.Should().Be(1);
            testInitializationStrategy.InitializedInstances.Should().ContainInOrder(warrior);
        }

        public class TestInitializationStrategy : IInitializationStrategy
        {
            private int initializationCount = 0;

            public TestInitializationStrategy()
            {
                this.InitializedInstances = new List<object>();
            }


            public List<object> InitializedInstances { get; }

            public int InitializationCount
            {
                get
                {
                    return this.initializationCount;
                }
            }

            public void Dispose()
            {
            }

            public object Initialize(IContext context, object instance)
            {
                this.initializationCount++;
                this.InitializedInstances.Add(instance);
                return instance;
            }
        }

        private class InitializedTestProvider : IProvider<Sword>
        {
            public Type Type => typeof(Sword);

            public bool ResolvesServices => false;

            public object Create(IContext context, out bool isInitialized)
            {
                isInitialized = true;
                return new Sword();
            }
        }

        private class NotInitializedTestProvider : IProvider<Sword>
        {
            public Type Type => typeof(Sword);

            public bool ResolvesServices => false;

            public object Create(IContext context, out bool isInitialized)
            {
                isInitialized = false;
                return new Sword();
            }
        }
    }
}