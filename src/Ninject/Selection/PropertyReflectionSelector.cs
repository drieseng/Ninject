﻿// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyReflectionSelector.cs" company="Ninject Project Contributors">
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

namespace Ninject.Selection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;

    /// <summary>
    /// Selects properties to either inject services into or assign values to.
    /// </summary>
    public sealed class PropertyReflectionSelector : IPropertyReflectionSelector
    {
        /// <summary>
        /// The default binding flags.
        /// </summary>
        private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// The binding flags.
        /// </summary>
        private readonly BindingFlags bindingFlags;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyReflectionSelector"/> class.
        /// </summary>
        /// <param name="injectNonPublic"><see langword="true"/> to include non-public properties; otherwise, <see langword="false"/>.</param>
        public PropertyReflectionSelector(bool injectNonPublic)
        {
            this.bindingFlags = injectNonPublic ? (DefaultBindingFlags | BindingFlags.NonPublic) : DefaultBindingFlags;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include non-public properties.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if non-public properties are included; otherwise, <see langword="false"/>.
        /// </value>
        public bool InjectNonPublic
        {
            get
            {
                return (this.bindingFlags & BindingFlags.NonPublic) != 0;
            }
        }

        /// <summary>
        /// Selects the properties that could be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A series of the selected properties.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        public IEnumerable<PropertyInfo> Select(Type type)
        {
            Ensure.ArgumentNotNull(type, nameof(type));

            // Cache locally to avoid evaluation for each property
            var injectNonPublic = InjectNonPublic;

            return type.GetProperties(this.bindingFlags)
                       .Select(p => p.GetPropertyFromDeclaredType(p, this.bindingFlags))
                       .Where(p => p != null && p.GetSetMethod(injectNonPublic) != null);
        }
    }
}