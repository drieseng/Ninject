// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentCreateOptionsFactory.cs" company="Ninject Project Contributors">
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
    /// Factory for creating <see cref="ComponentCreateOptions"/> instances.
    /// </summary>
    internal class ComponentCreateOptionsFactory
    {
        private readonly IPlanner planner;
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCreateOptionsFactory"/> class.
        /// </summary>
        /// <param name="planner">The <see cref="IPlanner"/> component.</param>
        /// <param name="exceptionFormatter">The exception formatter.</param>
        public ComponentCreateOptionsFactory(IPlanner planner, IExceptionFormatter exceptionFormatter)
        {
            this.planner = planner;
            this.exceptionFormatter = exceptionFormatter;
        }

        /// <summary>
        /// Creates a <see cref="ComponentCreateOptions"/> for the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to create a <see cref="ComponentCreateOptions"/> for.</param>
        /// <returns>
        /// A <see cref="ComponentCreateOptions"/> for <paramref name="type"/>.
        /// </returns>
        public ComponentCreateOptions Create(Type type)
        {
            return new ComponentCreateOptions(type, this.planner.GetPlan(type));
        }

        /// <summary>
        /// Creates a <see cref="ComponentCreateOptions"/> for the specified <see cref="Type"/> and parameters.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to create a <see cref="ComponentCreateOptions"/> for.</param>
        /// <param name="parameters">The parameters to apply when creating an instance of <paramref name="type"/>.</param>
        /// <returns>
        /// A <see cref="ComponentCreateOptions"/> for <paramref name="type"/> and <paramref name="parameters"/>.
        /// </returns>
        public ComponentCreateOptions Create(Type type, IReadOnlyList<IParameter> parameters)
        {
            var constructorParameters = new Dictionary<string, IConstructorArgument>();
            var propertyValues = new Dictionary<string, IPropertyValue>();

            foreach (var parameter in parameters)
            {
                if (parameter is IConstructorArgument constructorArgument)
                {
                    if (constructorParameters.ContainsKey(constructorArgument.Name))
                    {
                        throw new Exception("TODO");
                    }

                    constructorParameters.Add(constructorArgument.Name, constructorArgument);
                }

                if (parameter is IPropertyValue propertyValue)
                {
                    if (propertyValues.ContainsKey(propertyValue.Name))
                    {
                        throw new Exception("TODO");
                    }

                    propertyValues.Add(propertyValue.Name, propertyValue);
                }
            }

            return new ComponentCreateOptions(type, this.planner.GetPlan(type), constructorParameters, propertyValues);
        }
    }
}