// -------------------------------------------------------------------------------------------------
// <copyright file="KernelBase.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2019 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
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
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ninject.Activation;
    using Ninject.Activation.Blocks;
    using Ninject.Builder;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Infrastructure.Language;
    using Ninject.Modules;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// The base implementation of an <see cref="IKernelBuilder"/>.
    /// </summary>
    public abstract class KernelBase : DisposableObject, INewBindingRoot, IKernel
    {
        private readonly KernelBuilder kernelBuilder;
        private readonly object kernelLockObject = new object();

        private IReadOnlyKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBase"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is <see langword="null"/>.</exception>
        protected KernelBase(INinjectSettings settings, params INinjectModule[] modules)
        {
            Ensure.ArgumentNotNull(settings, nameof(settings));

            this.Settings = settings;

            this.kernelBuilder = new KernelBuilder();
            this.kernelBuilder.Modules(m => m.Load(modules));
        }

        /// <summary>
        /// Gets the kernel settings.
        /// </summary>
        public INinjectSettings Settings { get; }

        /// <summary>
        /// Gets the component container, which holds components that contribute to Ninject.
        /// </summary>
        public IComponentContainer Components { get; }

        private IReadOnlyKernel ReadOnlyKernel
        {
            get
            {
                if (this.kernel != null)
                {
                    return this.kernel;
                }

                lock (this.kernelLockObject)
                {
                    if (this.kernel == null)
                    {
                        this.kernel = BuildReadOnlyKernel();
                    }

                    return this.kernel;
                }
            }
        }

        private IReadOnlyKernel BuildReadOnlyKernel()
        {
            AddComponents(kernelBuilder);
            return kernelBuilder.Build();
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called manually, otherwise by GC.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                if (this.kernel != null)
                    this.kernel.Dispose();
            }

            base.Dispose(disposing);
        }

        /*
        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="IKernelBuilder"/> has already been built.</exception>
        public override void Unbind(Type service)
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            this.kernelBuilder.Bindings(b => b.Unbind(service));
        }
        */

        /// <summary>
        /// Gets an instance of the specified service.
        /// </summary>
        /// <param name="service">The service to resolve.</param>
        /// <returns>
        /// An instance of the service.
        /// </returns>
        public object Get(Type service)
        {
            return null;
        }

        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        protected abstract void AddComponents(IKernelBuilder kernelBuilder);

        /*
        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="IKernelBuilder"/> has already been built.</exception>
        public override void AddBinding(INewBindingBuilder binding)
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            this.kernelBuilder.Bindings(b => b.AddBinding(binding));
        }
        */

        /*
        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="IKernel"/> has already been built.</exception>
        void IKernel.AddBinding(INewBindingBuilder binding)
        {
            this.AddBinding(binding);
        }
        */

        /*

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        protected internal override void RemoveBinding(INewBindingBuilder binding)
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            this.kernelBuilder.BindingsBuilder.RemoveBinding(binding);
        }
        */

        /// <summary>
        /// Determines whether a module with the specified name has been loaded in the kernel.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        /// <returns>
        /// <see langword="true"/> if the specified module has been loaded; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public bool HasModule(string name)
        {
            return kernelBuilder.ModuleBuilder.HasModule(name);
        }

        /// <summary>
        /// Gets the modules that have been loaded into the kernel.
        /// </summary>
        /// <returns>A series of loaded modules.</returns>
        public IEnumerable<INinjectModule> GetModules()
        {
            return kernelBuilder.ModuleBuilder.Modules;
        }

        /// <summary>
        /// Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="modules">The modules to load.</param>
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="IKernelBuilder"/> has already been built.</exception>
        public void Load(IEnumerable<INinjectModule> modules)
        {
            Ensure.ArgumentNotNull(modules, nameof(modules));

            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            kernelBuilder.Modules(m => m.Load(modules));
        }

        /// <summary>
        /// Injects the specified existing instance, without managing its lifecycle.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public virtual void Inject(object instance, params IParameter[] parameters)
        {
            this.ReadOnlyKernel.Inject(instance, parameters);
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        /// <see langword="true"/> if the instance was found and released; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        public virtual bool Release(object instance)
        {
            return this.ReadOnlyKernel.Release(instance);
        }

        /// <summary>
        /// Immediately deactivates and removes all instances in the cache that are owned by
        /// the specified scope.
        /// </summary>
        /// <param name="scope">The scope whose instances should be deactivated.</param>
        public void Clear(object scope)
        {
            this.ReadOnlyKernel.Clear(scope);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// <see langword="true"/> if the request can be resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public virtual bool CanResolve(IRequest request)
        {
            return this.ReadOnlyKernel.CanResolve(request);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <see langword="true"/> implicit bindings are ignored.</param>
        /// <returns>
        /// <see langword="true"/> if the request can be resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public virtual bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            return this.ReadOnlyKernel.CanResolve(request, ignoreImplicitBindings);
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>
        /// An enumerator of instances that match the request.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ActivationException">More than one matching bindings is available for the request, and <see cref="IRequest.IsUnique"/> is <see langword="true"/>.</exception>
        public virtual IEnumerable<object> Resolve(IRequest request)
        {
            return this.ReadOnlyKernel.Resolve(request);
        }

        /// <summary>
        /// Creates a request for the specified service.
        /// </summary>
        /// <param name="service">The service that is being requested.</param>
        /// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        /// <param name="parameters">The parameters to pass to the resolution.</param>
        /// <param name="isOptional"><see langword="true"/> if the request is optional; otherwise, <see langword="false"/>.</param>
        /// <param name="isUnique"><see langword="true"/> if the request should return a unique result; otherwise, <see langword="false"/>.</param>
        /// <returns>The created request.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public virtual IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IReadOnlyList<IParameter> parameters, bool isOptional, bool isUnique)
        {
            return this.ReadOnlyKernel.CreateRequest(service, constraint, parameters, isOptional, isUnique);
        }

        /// <summary>
        /// Begins a new activation block, which can be used to deterministically dispose resolved instances.
        /// </summary>
        /// <returns>The new activation block.</returns>
        public virtual IActivationBlock BeginBlock()
        {
            return new ActivationBlock(this);
        }

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>A series of bindings that are registered for the service.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        public virtual IBinding[] GetBindings(Type service)
        {
            return this.ReadOnlyKernel.GetBindings(service);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is <see langword="null"/>.</exception>
        public object GetService(Type serviceType)
        {
            return this.ReadOnlyKernel.GetService(serviceType);
        }

        /// <summary>
        /// Resolves an instance for the specified request.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>
        /// An instance that matches the request.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ActivationException">More than one matching bindings is available for the request, and <see cref="IRequest.IsUnique"/> is <see langword="true"/>.</exception>
        public object ResolveSingle(IRequest request)
        {
            return this.ReadOnlyKernel.ResolveSingle(request);
        }

        private static InvalidOperationException CreateKernelHasBeenBuiltException()
        {
            return new InvalidOperationException("Cannot perform this operation after the kernel has been built.");
        }

        public bool IsBound<T>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            bool isBound = false;
            this.kernelBuilder.Bindings(m => isBound = m.IsBound<T>());
            return isBound;
        }

        public INewBindingToSyntax<T> Bind<T>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<T> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Bind<T>());
            return syntax;
        }

        public INewBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<T1, T2> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Bind<T1, T2>());
            return syntax;
        }

        public INewBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<T1, T2, T3> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Bind<T1, T2, T3>());
            return syntax;
        }

        public INewBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<T1, T2, T3, T4> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Bind<T1, T2, T3, T4>());
            return syntax;
        }

        public INewBindingToSyntax<object> Bind(params Type[] services)
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<object> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Bind(services));
            return syntax;
        }

        public void Unbind<T>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            this.kernelBuilder.Bindings(m => m.Unbind<T>());
        }

        public void Unbind(Type service)
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            this.kernelBuilder.Bindings(m => m.Unbind(service));
        }

        public INewBindingToSyntax<T1> Rebind<T1>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<T1> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Rebind<T1>());
            return syntax;
        }

        public INewBindingToSyntax<T1, T2> Rebind<T1, T2>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<T1, T2> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Rebind<T1, T2>());
            return syntax;
        }

        public INewBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<T1, T2, T3> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Rebind<T1, T2, T3>());
            return syntax;
        }

        public INewBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>()
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<T1, T2, T3, T4> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Rebind<T1, T2, T3, T4>());
            return syntax;
        }

        public INewBindingToSyntax<object> Rebind(params Type[] services)
        {
            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            INewBindingToSyntax<object> syntax = null;
            this.kernelBuilder.Bindings(m => syntax = m.Rebind(services));
            return syntax;
        }
    }
}
