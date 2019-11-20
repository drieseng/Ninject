
// -------------------------------------------------------------------------------------------------
// <copyright file="BindingBuilder.cs" company="Ninject Project Contributors">
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

namespace Ninject.Builder
{
    using Ninject.Builder.Bindings;
    using Ninject.Infrastructure;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;
    using System;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="Binding"/>.
    /// </summary>
    internal abstract class NewBindingBuilder : INewBindingBuilder
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the service to bind.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> of the service to bind.
        /// </value>
        public abstract Type Service { get; }

        /// <summary>
        /// Gets the names of the services that this instance builds a binding for.
        /// </summary>
        /// <value>
        /// The names of the services that this instance builds a binding for.
        /// </value>
        public abstract string ServiceNames { get; }

        /// <summary>
        /// Gets the binding being built.
        /// </summary>
        public abstract BindingConfigurationBuilder BindingConfigurationBuilder { get; }

        INewBindingConfigurationBuilder INewBindingBuilder.BindingConfigurationBuilder
        {
            get
            {
                return BindingConfigurationBuilder;
            }
        }

        /// <summary>
        /// Builds the binding(s) of this instance.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="bindingVisitor">Gathers the bindings that are built from this <see cref="NewBindingBuilder"/>.</param>
        public abstract void Build(IResolutionRoot root, IVisitor<IBinding> bindingVisitor);

        public abstract void Accept(IVisitor<NewBindingBuilder> visitor);

        void INewBindingBuilder.Build(IResolutionRoot root, IVisitor<IBinding> bindingVisitor)
        {
            Build(root, bindingVisitor);
        }
    }
}