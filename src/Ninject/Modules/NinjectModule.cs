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
    using Ninject.Builder;
    using Ninject.Syntax;

    /// <summary>
    /// A loadable unit that defines bindings for your application.
    /// </summary>
    public abstract class NinjectModule : NewBindingRoot, INinjectModule
    {
        /// <summary>
        /// Gets the module's name. Only a single module with a given name can be loaded at one time.
        /// </summary>
        public virtual string Name
        {
            get { return this.GetType().FullName; }
        }

        /// <summary>
        /// Called when the module is loaded into a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is loading the module.</param>
        public abstract void OnLoad(IKernelConfiguration kernel);

        /// <summary>
        /// Called after all modules are loaded. A module can verify here if all other required binding are available.
        /// </summary>
        public virtual void LoadCompleted(IKernelConfiguration kernel)
        {
        }

        /*
        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        public override void Unbind(Type service)
        {
            this.Kernel.Bindings(b => b.Unbind(service));
        }

        public void OnUnload(IKernelBuilder kernel)
        {
            this.Kernel.Bindings(builder => this.bindings.ForEach(b => builder.RemoveBinding(b)));
        }
        */

        /*
        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public override void AddBinding(INewBindingBuilder binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.Kernel.Bindings(b => b.AddBinding(binding));


            this.Bindings.Add(binding);
        }
        */

        /*
        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        public override void RemoveBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.Kernel.RemoveBinding(binding);
            this.Bindings.Remove(binding);
        }
        */
    }
}