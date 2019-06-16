// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorArgument.cs" company="Ninject Project Contributors">
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
    using Ninject.Planning.Targets;

    /// <summary>
    /// Overrides the injected value of a constructor argument.
    /// </summary>
    public class ConstructorArgument : Parameter, IConstructorArgument, IEquatable<ConstructorArgument>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public ConstructorArgument(string name, object value)
            : base(name, value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        public ConstructorArgument(string name, Func<IContext, object> valueCallback)
            : base(name, valueCallback, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        public ConstructorArgument(string name, Func<IContext, ITarget<ParameterInfo>, object> valueCallback)
            : base(name, valueCallback, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public ConstructorArgument(string name, object value, bool shouldInherit)
            : base(name, value, shouldInherit)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        /// <param name="shouldInherit">if set to <see langword="true"/> [should inherit].</param>
        public ConstructorArgument(string name, Func<IContext, object> valueCallback, bool shouldInherit)
            : base(name, valueCallback, shouldInherit)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        /// <param name="shouldInherit">if set to <see langword="true"/> [should inherit].</param>
        public ConstructorArgument(string name, Func<IContext, ITarget<ParameterInfo>, object> valueCallback, bool shouldInherit)
            : base(name, valueCallback, shouldInherit)
        {
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
        public bool AppliesToTarget(IContext context, ITarget<ParameterInfo> target)
        {
            return string.Equals(this.Name, target.Name);
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
            return obj is ConstructorArgument argument ? this.Equals(argument) : base.Equals(obj);
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
        public bool Equals(ConstructorArgument other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Name.Equals(this.Name);
        }
    }
}