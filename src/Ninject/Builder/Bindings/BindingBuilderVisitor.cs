﻿// -------------------------------------------------------------------------------------------------
// <copyright file="BindingBuilderVisitor.cs" company="Ninject Project Contributors">
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
    using System;
    using System.Collections.Generic;

    using Ninject.Infrastructure;
    using Ninject.Planning.Bindings;

    internal class BindingBuilderVisitor : IVisitor<IBinding>
    {
        private readonly Dictionary<Type, ICollection<IBinding>> bindingsByType;

        public BindingBuilderVisitor()
        {
            this.bindingsByType = new Dictionary<Type, ICollection<IBinding>>(new ReferenceEqualityTypeComparer());
        }

        public BindingBuilderVisitor(Dictionary<Type, ICollection<IBinding>> bindings)
        {
            this.bindingsByType = bindings;
        }

        /// <summary>
        /// Gets the gathered bindings.
        /// </summary>
        /// <value>
        /// The gathered bindings.
        /// </value>
        public Dictionary<Type, ICollection<IBinding>> Bindings
        {
            get { return this.bindingsByType; }
        }

        public void Visit(IBinding binding)
        {
            if (!this.bindingsByType.TryGetValue(binding.Service, out var bindingsForType))
            {
                bindingsForType = new List<IBinding>();
                this.bindingsByType.Add(binding.Service, bindingsForType);
            }

            bindingsForType.Add(binding);
        }
    }
}