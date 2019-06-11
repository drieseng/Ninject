// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorReflectionSelectorBuilder.cs" company="Ninject Project Contributors">
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
    using Ninject.Builder.Syntax;
    using Ninject.Selection;

    /// <summary>
    /// Builds a component that composes a list of constructors that serve as candidate for instantiating a given
    /// service.
    /// </summary>
    internal sealed class ConstructorReflectionSelectorBuilder : IConstructorReflectionSelectorBuilder
    {
        private bool injectNonPublic;

        /// <summary>
        /// Builds the component.
        /// </summary>
        public void Build(IComponentBindingRoot root)
        {
            root.Bind<IConstructorReflectionSelector>()
                .To<ConstructorReflectionSelector>()
                .InSingletonScope()
                .WithPropertyValue(nameof(ConstructorReflectionSelector.InjectNonPublic), this.injectNonPublic);
        }

        /// <summary>
        /// Configures whether non-public constructors should also be inclused in the list of constructors.
        /// </summary>
        /// <param name="value"><see langword="true"/> if non-public constructors should also be included; otherwise, <see langword="false"/>.</param>
        /// <remarks>
        /// By default, only public constructors are included in the list of constructors.
        /// </remarks>
        public void InjectNonPublic(bool value)
        {
            this.injectNonPublic = value;
        }
    }
}