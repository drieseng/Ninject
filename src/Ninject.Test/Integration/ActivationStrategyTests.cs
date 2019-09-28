namespace Ninject.Tests.Integration
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Ninject.Activation;
    using Ninject.Activation.Strategies;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ActivationStrategyTests : IDisposable
    {
        private readonly StandardKernel kernel;

        public ActivationStrategyTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void ActivationActions_WithoutContext_NotExecutedWhenTransientInstanceIsInjectedIntoScopedInstance()
        {
            int daggerActivationCount = 0;

            this.kernel.Bind<IWarrior>().To<Ninja>().InThreadScope();
            this.kernel.Bind<IWeapon>().To<Dagger>().OnActivation(instance => daggerActivationCount++);

            var warrior = this.kernel.Get<IWarrior>();
            warrior.Should().NotBeNull();
            warrior.Should().BeOfType<Ninja>();
            warrior.Weapon.Should().BeOfType<Dagger>();
            daggerActivationCount.Should().Be(0);
        }

        [Fact]
        public void ActivationActions_WithoutContext_NotExecutedWhenTransientInstanceIsCreated()
        {
            int ninjaActivationCount = 0;

            this.kernel.Bind<IWarrior>().To<Ninja>().OnActivation(instance => ninjaActivationCount++);
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var warrior = this.kernel.Get<IWarrior>();
            warrior.Should().NotBeNull();
            warrior.Should().BeOfType<Ninja>();
            warrior.Weapon.Should().BeOfType<Dagger>();
            ninjaActivationCount.Should().Be(0);
        }

        [Fact]
        public void ActivationActions_WithoutContext_ExecutedWhenScopedInstanceIsCreated()
        {
            int barracksActivationCount = 0;

            this.kernel.Bind<Barracks>().ToSelf().InThreadScope().OnActivation(
                instance => 
                {
                    instance.Warrior = new FootSoldier();
                    instance.Weapon = new Shuriken();
                    barracksActivationCount++;
                });

            var barracks = this.kernel.Get<Barracks>();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().BeOfType<Shuriken>();
            barracksActivationCount.Should().Be(1);

            this.kernel.Get<Barracks>();
            barracksActivationCount.Should().Be(1);
        }

        [Fact]
        public void ActivationActions_WithContext_NotExecutedWhenTransientInstanceIsInjectedIntoScopedInstance()
        {
            int daggerActivationCount = 0;

            this.kernel.Bind<IWarrior>().To<Ninja>().InThreadScope();
            this.kernel.Bind<IWeapon>().To<Dagger>().OnActivation((ctx, instance) => daggerActivationCount++);

            var warrior = this.kernel.Get<IWarrior>();
            warrior.Should().NotBeNull();
            warrior.Should().BeOfType<Ninja>();
            warrior.Weapon.Should().BeOfType<Dagger>();
            daggerActivationCount.Should().Be(0);
        }

        [Fact]
        public void ActivationActions_WithContext_NotExecutedWhenTransientInstanceIsCreated()
        {
            int ninjaActivationCount = 0;

            this.kernel.Bind<IWarrior>().To<Ninja>().OnActivation((ctx, instance) => ninjaActivationCount++);
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var warrior = this.kernel.Get<IWarrior>();
            warrior.Should().NotBeNull();
            warrior.Should().BeOfType<Ninja>();
            warrior.Weapon.Should().BeOfType<Dagger>();
            ninjaActivationCount.Should().Be(0);
        }

        [Fact]
        public void ActivationActions_WithContext_ExecutedWhenScopedInstanceIsCreated()
        {
            int warriorActivationCount = 0;
            IContext activationContext = null;

            this.kernel.Bind<IWarrior>().To<Ninja>().InThreadScope().OnActivation(
                (ctx, instance) =>
                {
                    instance.Weapon = new Sword();
                    warriorActivationCount++;
                    activationContext = ctx;
                });
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var warrior = this.kernel.Get<IWarrior>();
            warrior.Should().NotBeNull();
            warrior.Weapon.Should().BeOfType<Sword>();
            warriorActivationCount.Should().Be(1);
            activationContext.Should().NotBeNull();
            activationContext.Request.Service.Should().Be<IWarrior>();

            this.kernel.Get<IWarrior>();
            warriorActivationCount.Should().Be(1);
        }

        [Fact]
        public void DeactivationActions_WithoutContext_ExecutedWhenScopedInstanceIsReleased()
        {
            int barracksDeactivationCount = 0;

            this.kernel.Bind<Barracks>().ToSelf().InSingletonScope()
                  .OnActivation(
                      instance =>
                      {
                          instance.Warrior = new FootSoldier();
                          instance.Weapon = new Shuriken();
                      })
                  .OnDeactivation(
                      instance =>
                      {
                          instance.Warrior = null;
                          instance.Weapon = null;
                          barracksDeactivationCount++;
                      });

            Barracks barracks = this.kernel.Get<Barracks>();
            barracks.Warrior.Should().BeOfType<FootSoldier>();
            barracks.Weapon.Should().BeOfType<Shuriken>();
            barracksDeactivationCount.Should().Be(0);

            this.kernel.Release(barracks);
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
            barracksDeactivationCount.Should().Be(1);
        }

        [Fact]
        public void DeactivationActions_WithContext_ExecutedWhenScopedInstanceIsReleased()
        {
            int warriorDeactivationCount = 0;
            IContext deactivationContext = null;

            this.kernel.Bind<IWarrior>().To<Ninja>().InThreadScope()
                .OnActivation((ctx, instance) =>
                    {
                        instance.Weapon = new Sword();
                    })
                .OnDeactivation((ctx, instance) =>
                    {
                        instance.Weapon = null;
                        warriorDeactivationCount++;
                        deactivationContext = ctx;
                    });
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var warrior = this.kernel.Get<IWarrior>();
            warrior.Should().BeOfType<Ninja>();
            warrior.Weapon.Should().BeOfType<Sword>();
            warriorDeactivationCount.Should().Be(0);

            this.kernel.Release(warrior);
            warrior.Weapon.Should().BeNull();
            warriorDeactivationCount.Should().Be(1);

            deactivationContext.Should().NotBeNull();
            deactivationContext.Request.Service.Should().Be<IWarrior>();
        }

        [Fact]
        public void DeactivationActions_WithContext_ExecutedWhenScopeOfInstanceIsCleared()
        {
            var scope = new object();
            int warriorDeactivationCount = 0;
            IContext deactivationContext = null;

            this.kernel.Bind<IWarrior>().To<Ninja>().InScope(ctx => scope)
                .OnActivation((ctx, instance) =>
                {
                    instance.Weapon = new Sword();
                })
                .OnDeactivation((ctx, instance) =>
                {
                    instance.Weapon = null;
                    warriorDeactivationCount++;
                    deactivationContext = ctx;
                });
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var warrior = this.kernel.Get<IWarrior>();
            warrior.Should().BeOfType<Ninja>();
            warrior.Weapon.Should().BeOfType<Sword>();
            warriorDeactivationCount.Should().Be(0);

            this.kernel.Clear(scope);
            warrior.Weapon.Should().BeNull();
            warriorDeactivationCount.Should().Be(1);

            deactivationContext.Should().NotBeNull();
            deactivationContext.Request.Service.Should().Be<IWarrior>();
        }

        [Fact]
        public void ObjectsActivatedOnlyOnce()
        {
            this.kernel.Components.Add<IActivationStrategy, TestActivationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<Sword>().ToSelf();
            this.kernel.Bind<IWeapon>().ToMethod(ctx => ctx.Kernel.Get<Sword>());
            var testActivationStrategy = this.kernel.Components.GetAll<IActivationStrategy>().OfType<TestActivationStrategy>().Single();

            this.kernel.Get<IWarrior>();

            testActivationStrategy.ActivationCount.Should().Be(2);
        }

        [Fact]
        public void NullIsNotActivated()
        {
            this.kernel.Settings.AllowNullInjection = true;
            this.kernel.Components.Add<IActivationStrategy, TestActivationStrategy>();
            this.kernel.Bind<IWarrior>().To<Samurai>().InSingletonScope();
            this.kernel.Bind<IWeapon>().ToConstant((IWeapon)null);
            var testActivationStrategy = this.kernel.Components.GetAll<IActivationStrategy>().OfType<TestActivationStrategy>().Single();

            this.kernel.Get<IWarrior>();

            testActivationStrategy.ActivationCount.Should().Be(1);
        }

        public class TestActivationStrategy : IActivationStrategy
        {
            private int activationCount = 0;

            public int ActivationCount
            {
                get
                {
                    return this.activationCount;
                }
            }

            public void Activate(Activation.IContext context, Activation.InstanceReference reference)
            {
                this.activationCount++;
            }

            public void Dispose()
            {
            }
        }
    }
}