// -------------------------------------------------------------------------------------------------
// <copyright file="BindingsBuilder.cs" company="Ninject Project Contributors">
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

#if false

namespace Ninject.Builder
{
    using System;
    using System.Collections.Generic;
    using Ninject.Builder.Bindings;
    using Ninject.Infrastructure;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// Configures bindings for an <see cref="IKernelBuilder"/>.
    /// </summary>
    internal sealed class NewBindingsBuilder : NewBindingRoot
    {
        private readonly List<INewBindingBuilder> bindingBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewBindingsBuilder"/> class.
        /// </summary>
        public NewBindingsBuilder()
        {
            this.bindingBuilders = new List<INewBindingBuilder>();
        }

        public IReadOnlyList<INewBindingBuilder> Bindings
        {
            get { return this.bindingBuilders; }
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

        public override void Unbind(Type service)
        {
            for (var i = (this.bindingBuilders.Count - 1); i >= 0; i--)
            {
                var bindingBuilder = this.bindingBuilders[i];
                if (bindingBuilder.Service == service)
                {
                    this.bindingBuilders.RemoveAt(i);
                }
            }
        }

        public void Accept(IVisitor<INewBindingBuilder> visitor)
        {
            foreach (var bindingBuilder in this.bindingBuilders)
            {
                bindingBuilder.Accept(visitor);
            }
        }

        public override void AddBinding(INewBindingBuilder binding)
        {
            this.bindingBuilders.Add(binding);
        }

        protected internal override void RemoveBinding(INewBindingBuilder binding)
        {
            this.bindingBuilders.Remove(binding);
        }
    }
}

#endif