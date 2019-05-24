// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentCreateOptions.cs" company="Ninject Project Contributors">
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

namespace Ninject.Components
{
    using System;
    using System.Collections.Generic;

    using Ninject.Parameters;
    using Ninject.Planning;

    /// <summary>
    /// Provide information to create an instance of a given component.
    /// </summary>
    internal class ComponentCreateOptions
    {
        private static readonly IReadOnlyDictionary<string, IConstructorArgument> EmptyConstructorArguments = new Dictionary<string, IConstructorArgument>();
        private static readonly IReadOnlyDictionary<string, IPropertyValue> EmptyPropertyValues = new Dictionary<string, IPropertyValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCreateOptions"/> class.
        /// </summary>
        /// <param name="type">The type of the component.</param>
        /// <param name="plan">The activation plan for <paramref name="type"/>.</param>
        public ComponentCreateOptions(Type type, IPlan plan)
            : this(type, plan, EmptyConstructorArguments, EmptyPropertyValues)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCreateOptions"/> class.
        /// </summary>
        /// <param name="type">The type of the component.</param>
        /// <param name="plan">The activation plan for <paramref name="type"/>.</param>
        /// <param name="constructorParameters">The constructor parameters to use when instantiating the component.</param>
        /// <param name="propertyValues">The property values to use when instantiating the component.</param>
        public ComponentCreateOptions(Type type, IPlan plan, IReadOnlyDictionary<string, IConstructorArgument> constructorParameters, IReadOnlyDictionary<string, IPropertyValue> propertyValues)
        {
            this.Type = type;
            this.Plan = plan;
            this.ConstructorParameters = constructorParameters;
            this.PropertyValues = propertyValues;
        }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        public Type Type { get; }

        /// <summary>
        /// Gets the activation plan for <see cref="Type"/>.
        /// </summary>
        /// <value>
        /// The activation plan for <see cref="Type"/>.
        /// </value>
        public IPlan Plan { get; }

        /// <summary>
        /// Gets the constructor parameters to use when instantiating the component.
        /// </summary>
        /// <value>
        /// The constructor parameters to use when instantiating the component.
        /// </value>
        public IReadOnlyDictionary<string, IConstructorArgument> ConstructorParameters { get; }

        /// <summary>
        /// Gets the property values to use when instantiating the component.
        /// </summary>
        /// <value>
        /// The property values to use when instantiating the component.
        /// </value>
        public IReadOnlyDictionary<string, IPropertyValue> PropertyValues { get; }
    }
}