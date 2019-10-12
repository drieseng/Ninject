// -------------------------------------------------------------------------------------------------
// <copyright file="NinjectModule.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
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

namespace Ninject.Modules
{
    using System;
    using Ninject.Builder;
    using Ninject.Syntax;

    /// <summary>
    /// A loadable unit that defines bindings for your application.
    /// </summary>
    public abstract class NinjectModule : INinjectModule
    {
        private INewBindingRoot _bindingRoot;

        /// <summary>
        /// Gets the module's name. Only a single module with a given name can be loaded at one time.
        /// </summary>
        public virtual string Name
        {
            get { return this.GetType().FullName; }
        }

        private INewBindingRoot BindingRoot
        {
            get
            {
                if (_bindingRoot == null)
                {
                    throw new InvalidOperationException("Bindings can only be configured after module has been loaded.");
                }

                return _bindingRoot;
            }
        }

        /// <summary>
        /// Called when the module is loaded into a kernel configuration.
        /// </summary>
        /// <param name="kernelConfiguration">The configuration of the kernel that is loading the module.</param>
        void INinjectModule.OnLoad(IKernelConfiguration kernelConfiguration)
        {
            kernelConfiguration.Bindings(a => _bindingRoot = a);
            OnLoad(kernelConfiguration);
        }

        /// <summary>
        /// Called after all modules are loaded.
        /// </summary>
        /// <param name="kernelConfiguration">The configuration of the kernel that has loaded the modules.</param>
        /// <remarks>
        /// A module can verify here if all other required bindings are available.
        /// </remarks>
        void INinjectModule.OnLoadCompleted(IKernelConfiguration kernelConfiguration)
        {
            OnLoadCompleted(kernelConfiguration);
        }

        /// <summary>
        /// Called when the module is loaded into a kernel configuration.
        /// </summary>
        /// <param name="kernelConfiguration">The configuration of the kernel that is loading the module.</param>
        protected abstract void OnLoad(IKernelConfiguration kernelConfiguration);

        /// <summary>
        /// Called after all modules are loaded into a kernel configuration.
        /// </summary>
        /// <param name="kernelConfiguration">The configuration of the kernel that has loaded the modules.</param>
        /// <remarks>
        /// A module can verify here if all other required bindings are available.
        /// </remarks>
        protected virtual void OnLoadCompleted(IKernelConfiguration kernelConfiguration)
        {
        }

        /// <summary>
        /// Returns a value indicating whether a binding exists for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to check.</typeparam>
        /// <returns>
        /// <see langword="true"/> if a binding exists for <typeparamref name="T"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        protected bool IsBound<T>()
        {
            return BindingRoot.IsBound<T>();
        }

        protected INewBindingToSyntax<T> Bind<T>()
        {
            return BindingRoot.Bind<T>();
        }

        protected INewBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            return BindingRoot.Bind<T1, T2>();
        }

        protected INewBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
        {
            return BindingRoot.Bind<T1, T2, T3>();
        }

        protected INewBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            return BindingRoot.Bind<T1, T2, T3, T4>();
        }

        protected INewBindingToSyntax<object> Bind(params Type[] services)
        {
            return BindingRoot.Bind(services);
        }

        protected void Unbind<T>()
        {
            BindingRoot.Unbind<T>();
        }

        protected void Unbind(Type service)
        {
            BindingRoot.Unbind(service);
        }

        protected INewBindingToSyntax<T1> Rebind<T1>()
        {
            return BindingRoot.Rebind<T1>();
        }

        protected INewBindingToSyntax<T1, T2> Rebind<T1, T2>()
        {
            return BindingRoot.Rebind<T1, T2>();
        }

        protected INewBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>()
        {
            return BindingRoot.Rebind<T1, T2, T3>();
        }

        protected INewBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>()
        {
            return BindingRoot.Rebind<T1, T2, T3, T4>();
        }

        protected INewBindingToSyntax<object> Rebind(params Type[] services)
        {
            return BindingRoot.Rebind(services);
        }
   }
}