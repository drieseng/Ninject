﻿// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterTarget2.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Targets
{
    using System;
    using System.Reflection;

    public class ParameterTarget2 : Target<ParameterInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterTarget2"/> class.
        /// </summary>
        /// <param name="method">The method that defines the parameter.</param>
        /// <param name="site">The parameter that this target represents.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="site"/> is <see langword="null"/>.</exception>
        public ParameterTarget2(MethodBase method, ParameterInfo site)
            : base(method, site)
        {
        }

        public override Type Type => this.Site.ParameterType;

        public override string Name => this.Site.Name;

        /// <summary>
        /// Gets a value indicating whether the target has a default value.
        /// </summary>
        public override bool HasDefaultValue
        {
            get { return this.Site.HasDefaultValue; }
        }

        /// <summary>
        /// Gets the default value for the target.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the item does not have a default value.</exception>
        public override object DefaultValue
        {
            get { return this.HasDefaultValue ? this.Site.DefaultValue : base.DefaultValue; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return this.Site.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return this.Site.IsDefined(attributeType, inherit);
        }
    }
}