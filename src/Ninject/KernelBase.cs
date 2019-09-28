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
    using Ninject.Infrastructure.Language;
    using Ninject.Modules;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// The base implementation of an <see cref="IKernelBuilder"/>.
    /// </summary>
    public abstract class KernelBase : NewBindingRoot, IKernel
    {
        /// <summary>
        /// The ninject modules.
        /// </summary>
        private readonly Dictionary<string, INinjectModule> modules = new Dictionary<string, INinjectModule>();

        private KernelBuilder kernelBuilder;

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
            Ensure.ArgumentNotNull(modules, nameof(modules));

            this.Settings = settings;

            this.kernelBuilder = new KernelBuilder();
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
        public override void Dispose(bool disposing)
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
        /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or a zero-length <see cref="string"/>.</exception>
        public bool HasModule(string name)
        {
            Ensure.ArgumentNotNullOrEmpty(name, nameof(name));

            return this.modules.ContainsKey(name);
        }

        /// <summary>
        /// Gets the modules that have been loaded into the kernel.
        /// </summary>
        /// <returns>A series of loaded modules.</returns>
        public IEnumerable<INinjectModule> GetModules()
        {
            return this.modules.Values.ToArray();
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

            modules = modules.ToList();
            foreach (INinjectModule module in modules)
            {
                if (string.IsNullOrEmpty(module.Name))
                {
                    throw new NotSupportedException(ExceptionFormatter.ModulesWithNullOrEmptyNamesAreNotSupported());
                }

                if (this.modules.TryGetValue(module.Name, out INinjectModule existingModule))
                {
                    throw new NotSupportedException(ExceptionFormatter.ModuleWithSameNameIsAlreadyLoaded(module, existingModule));
                }

                module.OnLoad(kernelBuilder);

                this.modules.Add(module.Name, module);
            }

            foreach (INinjectModule module in modules)
            {
                module.LoadCompleted(kernelBuilder);
            }
        }

        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePatterns"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="IKernelBuilder"/> has already been built.</exception>
        public void Load(IEnumerable<string> filePatterns)
        {
            Ensure.ArgumentNotNull(filePatterns, nameof(filePatterns));

            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            var moduleLoader = this.Components.Get<IModuleLoader>();
            moduleLoader.LoadModules(filePatterns);
        }

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="IKernelBuilder"/> has already been built.</exception>
        public void Load(IEnumerable<Assembly> assemblies)
        {
            Ensure.ArgumentNotNull(assemblies, nameof(assemblies));

            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            this.Load(assemblies.SelectMany(asm => asm.GetNinjectModules()));
        }

        /// <summary>
        /// Unloads the plugin with the specified name.
        /// </summary>
        /// <param name="name">The plugin's name.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or a zero-length <see cref="string"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="IKernelBuilder"/> has already been built.</exception>
        public void Unload(string name)
        {
            Ensure.ArgumentNotNullOrEmpty(name, nameof(name));

            if (kernel != null)
            {
                throw CreateKernelHasBeenBuiltException();
            }

            if (!this.modules.TryGetValue(name, out INinjectModule module))
            {
                throw new NotSupportedException(ExceptionFormatter.NoModuleLoadedWithTheSpecifiedName(name));
            }

            /*
            module.OnUnload(this.kernelBuilder);
            */

            this.modules.Remove(name);
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
    }
}
