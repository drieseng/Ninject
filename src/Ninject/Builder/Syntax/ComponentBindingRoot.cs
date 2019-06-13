// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentRoot.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;

    using Ninject.Infrastructure;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    internal class ComponentBindingRoot : IComponentBindingRoot
    {
        private readonly List<ComponentBindingBuilder> bindingBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBindingRoot"/> class.
        /// </summary>
        public ComponentBindingRoot()
        {
            this.bindingBuilders = new List<ComponentBindingBuilder>();
        }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        public IDictionary<string, object> Properties { get; }


        /// <inheritdoc/>
        public IComponentBindingToSyntax<T> Bind<T>()
        {
            var bindingBuilder = new ComponentBindingBuilder<T>();
            this.bindingBuilders.Add(bindingBuilder);
            return bindingBuilder;
        }

        /// <inheritdoc/>
        public void Unbind<T>()
        {
            var serviceToUnbind = typeof(T);

            for (var i = this.bindingBuilders.Count - 1; i >= 0; i--)
            {
                if (this.bindingBuilders[i].Service == serviceToUnbind)
                {
                    this.bindingBuilders.RemoveAt(i);
                }
            }
        }

        /// <inheritdoc/>
        public bool IsBound<T>()
        {
            var service = typeof(T);

            foreach (var binding in this.bindingBuilders)
            {
                if (binding.Service == service)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Builds the bindings.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="bindingVisitor">Gathers built bindings.</param>
        public void Build(IResolutionRoot root, IVisitor<IBinding> bindingVisitor)
        {
            foreach (var bindingBuilder in this.bindingBuilders)
            {
                bindingBuilder.Build(root, bindingVisitor);
            }
        }
    }
}