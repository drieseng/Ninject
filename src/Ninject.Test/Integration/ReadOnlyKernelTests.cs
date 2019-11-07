using FluentAssertions;
using Ninject.Builder;
using Ninject.Parameters;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;
using Xunit;

namespace Ninject.Tests.Integration
{
    public class ReadOnlyKernelTests
    {
        public class WhenTryGetIsCalledForUnboundServiceAndSelfBindingIsRegistered
        {
            [Fact]
            public void TryGetOfT_Parameters_ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
            {
                using (var kernel = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding())).Build())
                {
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
                using (var kernel = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding())).Build())
                {
                    var weapon = kernel.TryGet<IWeapon>();
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b => b.Bind<IWeapon>().To<Sword>().When(ctx => false));

                using (var kernel = kernelBuilder.Build())
                {
                    var weapon = kernel.TryGet<IWeapon>();
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b => b.Bind<IWarrior>().To<Samurai>());

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet<IWarrior>();
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWarrior>().To<Samurai>();
                                                               b.Bind<IWeapon>().To<Sword>();
                                                               b.Bind<IWeapon>().To<Shuriken>();
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet<IWarrior>();
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWarrior>().To<Samurai>();
                                                               b.Bind<IWeapon>().To<Sword>().When(ctx => false);
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet<IWarrior>();
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGetOfT_NameAndParameters_ReturnsNullWhenNoMatchingBindingExistsAndRegistersImplicitSelfBindingIfTypeIsSelfBindable()
            {
                using (var kernel = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding())).Build())
                {
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
                using (var kernel = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding())).Build())
                {
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
                using (var kernel = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding())).Build())
                {
                    var weapon = kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().BeEmpty();
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoMatchingBindingExistsAndTypeIsNotSelfBindable()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b => b.Bind<IWeapon>().To<Sword>().Named("b"));

                using (var kernel = kernelBuilder.Build())
                {
                    var weapon = kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b => b.Bind<IWeapon>().To<Sword>().When(ctx => false).Named("a"));

                using (var kernel = kernelBuilder.Build())
                {
                    var weapon = kernel.TryGet(typeof(IWeapon), "a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b => b.Bind<IWarrior>().To<Samurai>().Named("a"));

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet(typeof(IWarrior), "a", Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWarrior>().To<Samurai>().Named("a");
                                                               b.Bind<IWeapon>().To<Sword>();
                                                               b.Bind<IWeapon>().To<Shuriken>();
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet(typeof(IWarrior), "a", Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndNameAndParameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWarrior>().To<Samurai>();
                                                               b.Bind<IWeapon>().To<Sword>().When(ctx => false);
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ImplicitSelfBindingIsRegisteredAndActivatedIfTypeIsSelfBindable()
            {
                using (var kernel = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding())).Build())
                {
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
                using (var kernel = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding())).Build())
                {
                    var weapon = kernel.TryGet(typeof(IWeapon), (metadata) => true, Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(0);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfTypeHasOnlyUnmetConditionalBindings()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b => b.Bind<IWeapon>().To<Sword>().When(ctx => false));

                using (var kernel = kernelBuilder.Build())
                {
                    var weapon = kernel.TryGet(typeof(IWeapon), (metadata) => true, Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfNoBindingForADependencyExists()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b => b.Bind<IWarrior>().To<Samurai>());

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfMultipleBindingsExistForADependency()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWarrior>().To<Samurai>();
                                                               b.Bind<IWeapon>().To<Sword>();
                                                               b.Bind<IWeapon>().To<Shuriken>();

                                                           });

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }

            [Fact]
            public void TryGet_ServiceAndConstraintAndParameters_ReturnsNullIfOnlyUnmetConditionalBindingsExistForADependency()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWarrior>().To<Samurai>();
                                                               b.Bind<IWeapon>().To<Sword>().When(ctx => false);

                                                           });

                using (var kernel = kernelBuilder.Build())
                {
                    var warrior = kernel.TryGet(typeof(IWarrior), (metadata) => true, Array.Empty<IParameter>());
                    warrior.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWarrior));
                    bindings.Should().HaveCount(1);
                }
            }
        }

        public class WhenTryGetIsCalledForServiceWithMultipleBindingsOfSameWeight
        {
            [Fact]
            public void TryGetOfT_Parameters()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWeapon>().To<Sword>();
                                                               b.Bind<IWeapon>().To<Shuriken>();
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
                    var weapon = kernel.TryGet<IWeapon>(Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGetOfT_NameAndParameters()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                       {
                                                           b.Bind<IWeapon>().To<Sword>().Named("a");
                                                           b.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                       });

                using (var kernel = kernelBuilder.Build())
                {
                    var weapon = kernel.TryGet<IWeapon>("a", Array.Empty<IParameter>());
                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(typeof(IWeapon));
                    bindings.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGetOfT_ConstraintAndParameters()
            {
                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWeapon>().To<Sword>().Named("a");
                                                               b.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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

                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWeapon>().To<Sword>();
                                                               b.Bind<IWeapon>().To<Shuriken>();
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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

                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<Sword>().To<Sword>().Named("a");
                                                               b.Bind<Sword>().To<ShortSword>().Named("a");
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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

                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWeapon>().To<Sword>().Named("a");
                                                               b.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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

                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<Sword>().To<ShortSword>().Named("b");
                                                               b.Bind<Sword>().To<Sword>().Named("a");
                                                               b.Bind<Sword>().To<ShortSword>().Named("a");
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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

                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWeapon>().To<Sword>().Named("a");
                                                               b.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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

                var kernelBuilder = new KernelBuilder().Features(f => f.Resolution(r => r.SelfBinding()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWeapon>().To<Sword>().Named("b");
                                                               b.Bind<IWeapon>().To<ShortSword>().Named("b");
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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
            [Fact]
            public void ActivationCacheIsDisabled_ActivationStrategiesAreExecutedForEachBindingWhenServicesAreBoundToSameScope()
            {
                var activations = new List<string>();

                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction()))
                                                       .Bindings(b =>
                                                            {
                                                                b.Bind<IWeapon>()
                                                                 .To<Dagger>();
                                                                b.Bind<StartableWihDependencies>()
                                                                 .ToSelf()
                                                                 .InSingletonScope()
                                                                 .OnActivation((s) => activations.Add("ToSelf"));
                                                                b.Bind<IStartableWihDependencies>()
                                                                 .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                                                 .InSingletonScope()
                                                                 .OnActivation((s) => activations.Add("ToMethod"));
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction()))
                                                       .Bindings(b =>
                                                           {
                                                               b.Bind<IWeapon>()
                                                                .To<Dagger>();
                                                               b.Bind<StartableWihDependencies>()
                                                                .ToSelf()
                                                                .InSingletonScope()
                                                                .OnActivation((s) => activations.Add("ToSelf"));
                                                               b.Bind<IStartableWihDependencies>()
                                                                .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>());
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction()))
                                                       .Bindings(bindings =>
                                                           {
                                                               bindings.Bind<IWeapon>()
                                                                       .To<Dagger>();
                                                               bindings.Bind<StartableWihDependencies>()
                                                                       .ToSelf()
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToSelf"));
                                                               bindings.Bind<IStartableWihDependencies>()
                                                                       .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>());
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction()))
                                                       .Bindings(bindings =>
                                                           {
                                                               bindings.Bind<IWeapon>()
                                                                       .To<Dagger>();
                                                               bindings.Bind<StartableWihDependencies>()
                                                                       .ToSelf().InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToSelf"));
                                                               bindings.Bind<IStartableWihDependencies>()
                                                                       .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToMethod"));
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction()))
                                                       .Bindings(bindings =>
                                                           {
                                                               bindings.Bind<IWeapon>()
                                                                       .To<Dagger>();
                                                               bindings.Bind<StartableWihDependencies>()
                                                                       .ToSelf()
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToSelf"));
                                                               bindings.Bind<IStartableWihDependencies>()
                                                                       .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToMethod"));
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction().Startable()))
                                                       .Bindings(bindings =>
                                                           {
                                                               bindings.Bind<IWeapon>()
                                                                       .To<Dagger>();
                                                               bindings.Bind<StartableWihDependencies>()
                                                                       .ToSelf()
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToSelf"));
                                                               bindings.Bind<IStartableWihDependencies>()
                                                                       .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToMethod"));
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction().Startable()))
                                                       .Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>()
                                                                        .To<Dagger>();
                                                                bindings.Bind<StartableWihDependencies>()
                                                                        .ToSelf()
                                                                        .InSingletonScope()
                                                                        .OnActivation((s) => activations.Add("ToSelf"));
                                                                bindings.Bind<IStartableWihDependencies>()
                                                                        .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>());
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction().Startable()))
                                                       .Bindings(bindings =>
                                                           {
                                                               bindings.Bind<IWeapon>()
                                                                       .To<Dagger>();
                                                               bindings.Bind<StartableWihDependencies>()
                                                                       .ToSelf()
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToSelf"));
                                                               bindings.Bind<IStartableWihDependencies>()
                                                                       .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>());
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction().Startable()))
                                                       .Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>()
                                                                        .To<Dagger>();
                                                                bindings.Bind<StartableWihDependencies>()
                                                                        .ToSelf()
                                                                        .InSingletonScope()
                                                                        .OnActivation((s) => activations.Add("ToSelf"));
                                                                bindings.Bind<IStartableWihDependencies>()
                                                                        .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                                                        .InSingletonScope()
                                                                        .OnActivation((s) => activations.Add("ToMethod"));
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var activations = new List<string>();
                var kernelBuilder = new KernelBuilder().Features(f => f.Activation(a => a.BindingAction().Startable()))
                                                       .Bindings(bindings =>
                                                           {
                                                               bindings.Bind<IWeapon>()
                                                                       .To<Dagger>();
                                                               bindings.Bind<StartableWihDependencies>()
                                                                       .ToSelf()
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToSelf"));
                                                               bindings.Bind<IStartableWihDependencies>()
                                                                       .ToMethod(ctx => ctx.Kernel.Get<StartableWihDependencies>())
                                                                       .InSingletonScope()
                                                                       .OnActivation((s) => activations.Add("ToMethod"));
                                                           });

                using (var kernel = kernelBuilder.Build())
                {
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

                [Inject]
                IWeapon Weapon { get; set; }
            }
        }

        public class WhenTryGetIsCalledForBoundListOfServices
        {
            [Fact]
            public void TryGetOfT_Parameters()
            {
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>().To<Sword>();
                                                                bindings.Bind<IWeapon>().To<Shuriken>();
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
                    var weapons = kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().HaveCount(2);
                }
            }

            [Fact]
            public void TryGetOfT_Parameters_ShouldPreferBindingForList()
            {
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>().To<Sword>();
                                                                bindings.Bind<IWeapon>().To<Shuriken>();
                                                                bindings.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() });
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
                    var weapons = kernel.TryGet<List<IWeapon>>(Array.Empty<IParameter>());
                    weapons.Should().NotBeNull();
                    weapons.Should().HaveCount(1);
                    weapons.Should().AllBeOfType<Dagger>();
                }
            }

            [Fact]
            public void TryGetOfT_NameAndParameters()
            {
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>().To<Dagger>().Named("b");
                                                                bindings.Bind<IWeapon>().To<Sword>().Named("a");
                                                                bindings.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>().To<Sword>().Named("a");
                                                                bindings.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                                bindings.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                                                                bindings.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>().To<Dagger>().Named("b");
                                                                bindings.Bind<IWeapon>().To<Sword>().Named("a");
                                                                bindings.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>().To<Sword>().Named("a");
                                                                bindings.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                                bindings.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Sword() }).Named("b");
                                                                bindings.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() }).Named("a");
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                                    {
                                                                        bindings.Bind<IWeapon>().To<Sword>();
                                                                        bindings.Bind<IWeapon>().To<Shuriken>();
                                                                    });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>().To<Sword>();
                                                                bindings.Bind<IWeapon>().To<Shuriken>();
                                                                bindings.Bind<List<IWeapon>>().ToMethod(c => new List<IWeapon> { new Dagger() });
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                                    {
                                                                        bindings.Bind<IWeapon>().To<Dagger>().Named("b");
                                                                        bindings.Bind<IWeapon>().To<Sword>().Named("a");
                                                                        bindings.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                                    });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                            {
                                                                bindings.Bind<IWeapon>()
                                                                        .To<Sword>()
                                                                        .Named("a");
                                                                bindings.Bind<IWeapon>()
                                                                        .To<Shuriken>()
                                                                        .Named("a");
                                                                bindings.Bind<List<IWeapon>>()
                                                                        .ToMethod(c => new List<IWeapon> { new Sword() })
                                                                        .Named("b");
                                                                bindings.Bind<List<IWeapon>>()
                                                                        .ToMethod(c => new List<IWeapon> { new Dagger() })
                                                                        .Named("a");
                                                            });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                                    {
                                                                        bindings.Bind<IWeapon>().To<Dagger>().Named("b");
                                                                        bindings.Bind<IWeapon>().To<Sword>().Named("a");
                                                                        bindings.Bind<IWeapon>().To<Shuriken>().Named("a");
                                                                    });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                                    {
                                                                        bindings.Bind<IWeapon>()
                                                                                .To<Sword>()
                                                                                .Named("a");
                                                                        bindings.Bind<IWeapon>()
                                                                                .To<Shuriken>()
                                                                                .Named("a");
                                                                        bindings.Bind<List<IWeapon>>()
                                                                                .ToMethod(c => new List<IWeapon> { new Sword() })
                                                                                .Named("b");
                                                                        bindings.Bind<List<IWeapon>>()
                                                                                .ToMethod(c => new List<IWeapon> { new Dagger() })
                                                                                .Named("a");
                                                                    });

                using (var kernel = kernelBuilder.Build())
                {
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
                var kernelBuilder = new KernelBuilder().Bindings(bindings =>
                                                                    {
                                                                        bindings.Bind<List<int>>()
                                                                                .ToConstant(new List<int> { 1 })
                                                                                .Named("a");
                                                                    });

                using (var kernel = kernelBuilder.Build())
                {
                    var weapon = kernel.TryGet(service, (metadata) => metadata.Name == "a", Array.Empty<IParameter>());

                    weapon.Should().BeNull();

                    var bindings = kernel.GetBindings(service);
                    bindings.Should().BeEmpty();
                }
            }
        }

        public class WhenTryGetIsCalledForUnboundListOfServices
        {
            [Fact]
            public void TryGetOfT_Parameters()
            {
                using (var kernel = new KernelBuilder().Build())
                {
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
                using (var kernel = new KernelBuilder().Build())
                {
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
                using (var kernel = new KernelBuilder().Build())
                {
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
                using (var kernel = new KernelBuilder().Build())
                {
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
                using (var kernel = new KernelBuilder().Build())
                {
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
                using (var kernel = new KernelBuilder().Build())
                {
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

                using (var kernel = new KernelBuilder().Build())
                {
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
