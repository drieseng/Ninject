// -------------------------------------------------------------------------------------------------
// <copyright file="WeakPropertyValue.cs" company="Ninject Project Contributors">
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

namespace Ninject.Parameters
{
    using System;

    using Ninject.Activation;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Overrides the injected value of a property.
    /// Keeps a weak reference to the value.
    /// </summary>
    public class WeakPropertyValue : Parameter, IPropertyValue
    {
        private readonly WeakReference weakReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakPropertyValue"/> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public WeakPropertyValue(string name, object value)
            : base(name, (object)null, false)
        {
            this.weakReference = new WeakReference(value);
            this.ValueCallback = (ctx, target) => this.weakReference.Target;
        }

        /// <summary>
        /// Determines if the parameter applies to the given target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// <see langword="true"/> if the parameter applies in the specified context to the specified target;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Only one parameter may return <see langword="true"/>.
        /// </remarks>
        public bool AppliesToTarget(IContext context, ITarget target)
        {
            return string.Equals(this.Name, target.Name);
        }
    }
}