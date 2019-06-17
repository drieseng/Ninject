// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyValue.cs" company="Ninject Project Contributors">
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
    using System.Reflection;
    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Overrides the injected value of a property.
    /// </summary>
    public class PropertyValue : IPropertyValue, IEquatable<IPropertyValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public PropertyValue(string name, object value)
            : this(name, value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public PropertyValue(string name, object value, bool shouldInherit)
            : this(name, (ctx, target) => value, shouldInherit)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueCallback"/> is <see langword="null"/>.</exception>
        public PropertyValue(string name, Func<IContext, object> valueCallback)
            : this(name, valueCallback, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="valueCallback">The callback that will be triggered to get the parameter's value.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueCallback"/> is <see langword="null"/>.</exception>
        public PropertyValue(string name, Func<IContext, object> valueCallback, bool shouldInherit)
        {
            Ensure.ArgumentNotNullOrEmpty(name, nameof(name));
            Ensure.ArgumentNotNull(valueCallback, nameof(valueCallback));

            this.Name = name;
            this.ValueCallback = (ctx, target) => valueCallback(ctx);
            this.ShouldInherit = shouldInherit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueCallback"/> is <see langword="null"/>.</exception>
        public PropertyValue(string name, Func<IContext, ITarget<PropertyInfo>, object> valueCallback)
            : this(name, valueCallback, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="valueCallback">The callback that will be triggered to get the parameter's value.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueCallback"/> is <see langword="null"/>.</exception>
        public PropertyValue(string name, Func<IContext, ITarget<PropertyInfo>, object> valueCallback, bool shouldInherit)
        {
            Ensure.ArgumentNotNullOrEmpty(name, nameof(name));
            Ensure.ArgumentNotNull(valueCallback, nameof(valueCallback));

            this.Name = name;
            this.ValueCallback = valueCallback;
            this.ShouldInherit = shouldInherit;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the parameter should be inherited into child requests.
        /// </summary>
        public bool ShouldInherit { get; }

        /// <summary>
        /// Gets the callback that will be triggered to get the parameter's value.
        /// </summary>
        public Func<IContext, ITarget<PropertyInfo>, object> ValueCallback { get; protected set; }

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
        public bool AppliesToTarget(IContext context, ITarget<PropertyInfo> target)
        {
            return string.Equals(this.Name, target.Name);
        }

        /// <summary>
        /// Gets the value for the parameter within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// The value for the parameter.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        public object GetValue(IContext context, ITarget<PropertyInfo> target)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            return this.ValueCallback(context, target);
        }

        /// <summary>
        /// Determines whether the object equals the specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true"/> if the objects are equal; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is PropertyValue propertyValue ? this.Equals(propertyValue) : base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the object.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        /// <summary>
        /// Determines whether the object equals the specified object.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true"/> if the objects are equal; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(IPropertyValue other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Name.Equals(this.Name);
        }
    }
}