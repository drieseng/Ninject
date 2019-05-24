// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyValueProvider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Providers
{
    using System;

    using Ninject.Planning.Directives;

    /// <summary>
    /// Provides values for injection into properties.
    /// </summary>
    public class PropertyValueProvider : IPropertyValueProvider
    {
        /// <summary>
        /// Gets a value for the property from the specified context.
        /// </summary>
        /// <param name="property">The property to provide a value for.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public object GetValue(PropertyInjectionDirective property, IContext context)
        {
            return property.Target.ResolveWithin(context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }
}