// -------------------------------------------------------------------------------------------------
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
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Selects properties to inject services into.
    /// </summary>
    public sealed class PropertyReflectionSelector : IPropertyReflectionSelector
    {
        /// <summary>
        /// The default binding flags.
        /// </summary>
        private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        private readonly List<IInjectionHeuristic> injectionHeuristics;

        /// <summary>
        /// The binding flags.
        /// </summary>
        private BindingFlags bindingFlags;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyReflectionSelector"/> class.
        /// </summary>
        /// <param name="injectionHeuristics">The injection heuristics.</param>
        /// <exception cref="ArgumentNullException"><paramref name="injectionHeuristics"/> is <see langword="null"/>.</exception>
        public PropertyReflectionSelector(IEnumerable<IInjectionHeuristic> injectionHeuristics)
        {
            Ensure.ArgumentNotNull(injectionHeuristics, nameof(injectionHeuristics));

            this.injectionHeuristics = injectionHeuristics.ToList();
            this.InjectNonPublic = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include non-public constructors.
        /// </summary>
        /// <value>
        /// <see langword="true"/> to include include non-public constructors; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        public bool InjectNonPublic
        {
            get
            {
                return (this.bindingFlags & BindingFlags.NonPublic) != 0;
            }

            set
            {
                this.bindingFlags = value ? (DefaultBindingFlags | BindingFlags.NonPublic) : DefaultBindingFlags;
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

            return type.GetProperties(this.bindingFlags)
                       .Select(p => p.GetPropertyFromDeclaredType(p, this.bindingFlags))
                       .Where(p => p != null && p.GetSetMethod(this.InjectNonPublic) != null && ShouldInject(this.injectionHeuristics, p));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        private static bool ShouldInject(List<IInjectionHeuristic> injectionHeuristics, PropertyInfo property)
        {
            var shouldInject = false;

            foreach (var injectionHeuristic in injectionHeuristics)
            {
                if (injectionHeuristic.ShouldInject(property))
                {
                    shouldInject = true;
                    break;
                }
            }

            return shouldInject;
        }
    }
}