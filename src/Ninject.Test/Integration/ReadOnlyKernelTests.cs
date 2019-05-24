using FluentAssertions;
using Ninject.Parameters;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;
using Xunit;

namespace Ninject.Tests.Integration
{
    public class ReadOnlyKernelTests
    {
        public class WhenTryGetIsCalledForUnboundService
        {
            private static IKernelConfiguration CreateConfiguration()
            {
                var settings = new NinjectSettings
                    {
                        // Disable to reduce memory pressure
                        ActivationCacheDisabled = true,
                        LoadExtensions = false,
                    };
                return new KernelConfiguration(settings);
            }

            [Fact]
            public void TryGetOfT_Parameters_ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet<Sword>();
                    weapon.Should().NotBeNull();
                    weapon.Should().BeOfType<Sword>();

                    var bindings = kernel.GetBindings(typeof(Sword));
                    bindings.Should().HaveCount(1);
                    bindings.Should().ContainSingle(b => b.IsImplicit);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfTypeIsNotSelfBindable()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet<IWeapon>();
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet<IWeapon>();
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet<IWarrior>();
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>();
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet<IWarrior>();
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>();
                    configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet<IWarrior>();
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGetOfT_NameAndParameters_ReturnsNullWhenNoMatchingBindingExistsAndRegistersImplicitSelfBindingIfTypeIsSelfBindable()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet<Sword>("a", Array.Empty<IParameter>());

                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(Sword));
                    bindings.Should().HaveCount(1);
                    bindings.Should().OnlyContain(b => b.IsImplicit);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullWhenNoMatchingBindingExistsAndRegistersImplicitSelfBindingIfTypeIsSelfBindable()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(typeof(Sword), "a", Array.Empty<IParameter>());

                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(Sword));
                    bindings.Should().HaveCount(1);
                    bindings.Should().OnlyContain(b => b.IsImplicit);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoBindingExistsAndTypeIsNotSelfBindable()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoMatchingBindingExistsAndTypeIsNotSelfBindable()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("b");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().When(ctx => false).Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet(typeof(IWarrior), "a", Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>().Named("a");
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet(typeof(IWarrior), "a", Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>();
                    configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(typeof(Sword), (metadata) => true, Array.Empty<IParameter>());
                    weapon.Should().NotBeNull();
                    weapon.Should().BeOfType<Sword>();

                    var bindings = kernel.GetBindings(typeof(Sword));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfTypeIsNotSelfBindable()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(typeof(IWeapon), (metadata) => true, Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(0);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(typeof(IWeapon), (metadata) => true, Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>();
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWarrior>().To<Samurai>();
                    configuration.Bind<IWeapon>().To<Sword>().When(ctx => false);

                    var kernel = configuration.BuildReadOnlyKernel();

                    var warrior = kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }
        }

        public class WhenTryGetIsCalledForServiceWithMultipleBindingsOfSameWeight
        {
            private static IKernelConfiguration CreateConfiguration()
            {
                var settings = new NinjectSettings
                    {
                        // Disable to reduce memory pressure
                        ActivationCacheDisabled = true,
                        LoadExtensions = false,
                    };
                return new KernelConfiguration(settings);
            }

            [Fact]
            public void TryGetOfT_Parameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet<IWeapon>(Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGetOfT_NameAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet<IWeapon>("a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet<IWeapon>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGet_ServiceAndParameters()
            {
                var service = typeof(IWeapon);

                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(service, Array.Empty<IParameter>());

                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().HaveCount(2);
                    bindings.Should().OnlyContain(b => !b.IsImplicit);
                }
            }

            [Fact(Skip = "Unique?")]
            public void TryGet_ServiceAndNameAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsSelfBinding()
            {
                var service = typeof(Sword);

                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<Sword>().To<Sword>().Named("a");
                    configuration.Bind<Sword>().To<ShortSword>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(service, "a", Array.Empty<IParameter>());
                    weapon.Should().NotBeNull();
                    weapon.Should().BeOfType<Sword>();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().HaveCount(2);
                    bindings.Should().OnlyContain(b => !b.IsImplicit);
                }
            }

            [Fact(Skip = "Unique?")]
            public void TryGet_ServiceAndNameAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsNotSelfBinding()
            {
                var service = typeof(IWeapon);

                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(service, "a", Array.Empty<IParameter>());
                    weapon.Should().NotBeNull();
                    weapon.Should().BeOfType<Sword>();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().HaveCount(2);
                    bindings.Should().OnlyContain(b => !b.IsImplicit);
                }
            }

            [Fact(Skip = "Unique?")]
            public void TryGet_ServiceAndConstraintAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsSelfBinding()
            {
                var service = typeof(Sword);

                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<Sword>().To<ShortSword>().Named("b");
                    configuration.Bind<Sword>().To<Sword>().Named("a");
                    configuration.Bind<Sword>().To<ShortSword>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                    weapon.Should().NotBeNull();
                    weapon.Should().BeOfType<Sword>();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().HaveCount(3);
                    bindings.Should().OnlyContain(b => !b.IsImplicit);
                }
            }

            [Fact(Skip ="Unique?")]
            public void TryGet_ServiceAndConstraintAndParameters_ResolvesUsingFirstMatchingBindingWhenTypeIsNotSelfBindingAndNotGeneric()
            {
                var service = typeof(IWeapon);

                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                    weapon.Should().NotBeNull();
                    weapon.Should().BeOfType<Sword>();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().HaveCount(2);
                    bindings.Should().OnlyContain(b => !b.IsImplicit);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenBindingsDoNotMatchAndTypeIsNotSelfBindingAndNotOpenGeneric()
            {
                var service = typeof(IWeapon);

                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("b");
                    configuration.Bind<IWeapon>().To<ShortSword>().Named("b");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().HaveCount(2);
                    bindings.Should().OnlyContain(b => !b.IsImplicit);
                }
            }
        }

        public class WhenMoreThanOneBindingIsInvolvedToResolveService
        {
            private static IKernelConfiguration CreateConfiguration(bool activationCacheDisabled)
            {
                var settings = new NinjectSettings
                    {
                        ActivationCacheDisabled = activationCacheDisabled,
                        LoadExtensions = false,
                        PropertyInjection = true
                    };
                return new KernelConfiguration(settings);
            }

            [Fact]
            public void ActivationCacheIsDisabled_ActivationStrategiesAreExecutedForEachBindingWhenServicesAreBoundToSameScope()
            {
                using (var configuration = CreateConfiguration(true))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>()
                                 .To<Dagger>();
                    configuration.Bind<StartableWihDependencies>()
                                 .ToSelf()
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>()
                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var singleton = kernel.Get<StartableWihDependencies>();
                    var singletonWeapon = singleton.Weapon;

                    var instance1 = kernel.Get<IStartableWihDependencies>();
                    var instance1Weapon = instance1.Weapon;

                    Assert.Same(singleton, instance1);
                    Assert.NotSame(singletonWeapon, instance1.Weapon);

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance2);
                    Assert.Same(instance1Weapon, instance2.Weapon);
                    Assert.NotSame(singletonWeapon, instance2.Weapon);

                    Assert.Equal(2, singleton.StartCount);
                    Assert.Equal(new[] { "ToSelf", "ToMethod" }, activations);
                }
            }

            [Fact]
            public void ActivationCacheIsDisabled_ActivationStrategiesAreOnlyExecutedForContextInWhichInstanceIsFirstActivatedWhenServiceIsBoundToDifferentScopeAndInstanceAlreadyExists()
            {
                using (var configuration = CreateConfiguration(true))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>()
                                 .To<Dagger>();
                    configuration.Bind<StartableWihDependencies>()
                                 .ToSelf()
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>()
                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                 .OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var singleton = kernel.Get<StartableWihDependencies>();
                    var weapon = singleton.Weapon;

                    var instance1 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance1);
                    Assert.Same(weapon, instance1.Weapon);

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, singleton.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            [Fact]
            public void ActivationCacheIsDisabled_ActivationStrategiesAreOnlyExecutedForContextInWhichInstanceIsFirstActivatedWhenServiceIsBoundToDifferentScopeAndInstanceDoesNotExist()
            {
                using (var configuration = CreateConfiguration(true))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>()
                                 .To<Dagger>();
                    configuration.Bind<StartableWihDependencies>()
                                 .ToSelf()
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>()
                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                 .OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var instance1 = kernel.Get<IStartableWihDependencies>();
                    var weapon = instance1.Weapon;

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(instance1, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, instance1.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            [Fact]
            public void ActivationCacheIsDisabled_ActivationStrategiesAreOnlyExecutedForContextInWhichInstanceIsFirstActivatedWhenServiceIsBoundToSameScopeAndInstanceAlreadyExists()
            {
                using (var configuration = CreateConfiguration(true))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>()
                                 .To<Dagger>();
                    configuration.Bind<StartableWihDependencies>()
                                 .ToSelf().InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>()
                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var singleton = kernel.Get<StartableWihDependencies>();
                    var weapon = singleton.Weapon;

                    var instance1 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance1);
                    Assert.Same(weapon, instance1.Weapon);

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, singleton.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            [Fact]
            public void ActivationCacheIsDisabled_ActivationStrategiesAreOnlyExecutedForContextInWhichInstanceIsFirstActivatedWhenServiceIsBoundToSameScopeAndInstanceDoesNotExist()
            {
                using (var configuration = CreateConfiguration(true))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>().To<Dagger>();
                    configuration.Bind<StartableWihDependencies>().ToSelf().InSingletonScope().OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>().ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>()).InSingletonScope().OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var instance1 = kernel.Get<IStartableWihDependencies>();
                    var weapon = instance1.Weapon;

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(instance1, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, instance1.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            [Fact]
            public void ActivationCacheIsEnabled_ActivationStrategiesAreExecutedOnceWhenServiceIsBoundInSameScope()
            {
                using (var configuration = CreateConfiguration(false))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>()
                                 .To<Dagger>();
                    configuration.Bind<StartableWihDependencies>()
                                 .ToSelf()
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>()
                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var singleton = kernel.Get<StartableWihDependencies>();
                    var weapon = singleton.Weapon;

                    var instance1 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance1);
                    Assert.Same(weapon, instance1.Weapon);

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, singleton.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            [Fact]
            public void ActivationCacheIsEnabled_ActivationStrategiesAreOnlyExecutedForContextInWhichInstanceIsFirstActivatedWhenServiceIsBoundToDifferentScopeAndInstanceAlreadyExists()
            {
                using (var configuration = CreateConfiguration(false))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>()
                                 .To<Dagger>();
                    configuration.Bind<StartableWihDependencies>()
                                 .ToSelf()
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>()
                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                 .OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var singleton = kernel.Get<StartableWihDependencies>();
                    var weapon = singleton.Weapon;

                    var instance1 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance1);
                    Assert.Same(weapon, instance1.Weapon);

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, singleton.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            [Fact]
            public void ActivationCacheIsEnabled_ActivationStrategiesAreOnlyExecutedForContextInWhichInstanceIsFirstActivatedWhenServiceIsBoundToDifferentScopeAndInstanceDoesNotExist()
            {
                using (var configuration = CreateConfiguration(false))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>()
                                 .To<Dagger>();
                    configuration.Bind<StartableWihDependencies>()
                                 .ToSelf()
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>()
                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                 .OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var instance1 = kernel.Get<IStartableWihDependencies>();
                    var weapon = instance1.Weapon;

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(instance1, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, instance1.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            [Fact]
            public void ActivationCacheIsEnabled_ActivationStrategiesAreOnlyExecutedForContextInWhichInstanceIsFirstActivatedWhenServiceIsBoundToSameScopeAndInstanceAlreadyExists()
            {
                using (var configuration = CreateConfiguration(false))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>()
                                 .To<Dagger>();
                    configuration.Bind<StartableWihDependencies>()
                                 .ToSelf().InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>()
                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                 .InSingletonScope()
                                 .OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var singleton = kernel.Get<StartableWihDependencies>();
                    var weapon = singleton.Weapon;

                    var instance1 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance1);
                    Assert.Same(weapon, instance1.Weapon);

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(singleton, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, singleton.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            [Fact]
            public void ActivationCacheIsEnabled_ActivationStrategiesAreOnlyExecutedForContextInWhichInstanceIsFirstActivatedWhenServiceIsBoundToSameScopeAndInstanceDoesNotExist()
            {
                using (var configuration = CreateConfiguration(false))
                {
                    var activations = new List<string>();

                    configuration.Bind<IWeapon>().To<Dagger>();
                    configuration.Bind<StartableWihDependencies>().ToSelf().InSingletonScope().OnActivation((s) => activations.Add("ToSelf"));
                    configuration.Bind<IStartableWihDependencies>().ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>()).InSingletonScope().OnActivation((s) => activations.Add("ToMethod"));

                    var kernel = configuration.BuildReadOnlyKernel();

                    var instance1 = kernel.Get<IStartableWihDependencies>();
                    var weapon = instance1.Weapon;

                    var instance2 = kernel.Get<IStartableWihDependencies>();

                    Assert.Same(instance1, instance2);
                    Assert.Same(weapon, instance2.Weapon);

                    Assert.Equal(1, instance1.StartCount);
                    Assert.Single(activations);
                    Assert.Equal("ToSelf", activations[0]);
                }
            }

            public class StartableWihDependencies : Startable, IStartableWihDependencies
            {
                [Inject]
                public IWeapon Weapon { get; set; }
            }

            public interface IStartableWihDependencies : IStartable
            {
                int StartCount { get; }

                int StopCount { get; }

                [Inject]
                IWeapon Weapon { get; set; }
            }
        }

        public class WhenTryGetIsCalledForBoundListOfServices
        {
            private static IKernelConfiguration CreateConfiguration()
            {
                var settings = new NinjectSettings
                    {
                        // Disable to reduce memory pressure
                        ActivationCacheDisabled = true,
                        LoadExtensions = false,
                    };
                return new KernelConfiguration(settings);
            }

            [Fact]
            public void TryGetOfT_Parameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ShouldPreferBindingForList()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() });

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().HaveCount(1);
                    weapons.Should().AllBeOfType<Dagger>();
                }
            }

            [Fact]
            public void TryGetOfT_NameAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Dagger>().Named("b");
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>("a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().HaveCount(3);

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(0);
                }
            }

            [Fact]
            public void TryGetOfT_NameAndParameters_ShouldPreferBindingForList()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>("a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().HaveCount(1);
                    weapons.Should().AllBeOfType<Dagger>();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Dagger>().Named("b");
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().HaveCount(3);

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(0);
                }
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters_ShouldPreferBindingForList()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().HaveCount(1);
                    weapons.Should().AllBeOfType<Dagger>();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(2);
                }
            }


            [Fact]
            public void TryGet_ServiceAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().HaveCount(2);

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(0);
                }
            }

            [Fact]
            public void TryGet_ServiceAndParameters_ShouldPreferBindingForList()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>();
                    configuration.Bind<IWeapon>().To<Shuriken>();
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() });

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().HaveCount(1);
                    weaponsList.Should().AllBeOfType<Dagger>();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Dagger>().Named("b");
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());

                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().HaveCount(3);

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(0);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ShouldPreferBindingForList()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().HaveCount(1);
                    weaponsList.Should().AllBeOfType<Dagger>();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(2);
                }
            }


            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Dagger>().Named("b");
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().HaveCount(3);

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(0);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ShouldPreferBindingForList()
            {
                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<IWeapon>().To<Sword>().Named("a");
                    configuration.Bind<IWeapon>().To<Shuriken>().Named("a");
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                    configuration.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().HaveCount(1);
                    weaponsList.Should().AllBeOfType<Dagger>();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenTypeIsUnboundGenericTypeDefinition()
            {
                var service = typeof(List<>);

                using (var configuration = CreateConfiguration())
                {
                    configuration.Bind<List<int>>().ToConstant(new List<int> { 1 }).Named("a");

                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().BeEmpty();
                }
            }
        }

        public class WhenTryGetIsCalledForUnboundListOfServices
        {
            private static IKernelConfiguration CreateConfiguration()
            {
                var settings = new NinjectSettings
                    {
                        // Disable to reduce memory pressure
                        ActivationCacheDisabled = true,
                        LoadExtensions = false,
                    };
                return new KernelConfiguration(settings);
            }

            [Fact]
            public void TryGetOfT_Parameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());

                    weapons.Should().NotBeNull();
                    weapons.Should().BeEmpty();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGetOfT_NameAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>("b", Array.Empty<IParameter>());

                    weapons.Should().NotBeNull();
                    weapons.Should().BeEmpty();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet<List<IWeapon>>((metadata) => metadata.Name == "b", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeEmpty();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGet_ServiceAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().BeEmpty();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), "a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().BeEmpty();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters()
            {
                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapons = kernel.TryGet(typeof(List<IWeapon>), (metadata) => metadata.Name == "a", Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().BeOfType<List<IWeapon>>();

                    var weaponsList = (List<IWeapon>)weapons;
                    weaponsList.Should().BeEmpty();

                    var bindings = kernel.GetBindings(typeof(List<IWeapon>));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullWhenTypeIsUnboundGenericTypeDefinition()
            {
                var service = typeof(List<>);

                using (var configuration = CreateConfiguration())
                {
                    var kernel = configuration.BuildReadOnlyKernel();

                    var weapon = kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().BeEmpty();
                }
            }
        }

        public class GenericService<T>
        {
        }
    }
}
