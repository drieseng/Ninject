// -------------------------------------------------------------------------------------------------
// <copyright file="ContextFactory.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation
{
    using Ninject.Activation.Caching;
    using Ninject.Components;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using System;

    internal class ContextFactory : IContextFactory
    {
        private readonly ICache cache;
        private readonly IPlanner planner;
        private readonly IPipeline pipeline;
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new <see cref="ContextFactory"/> instance.
        /// </summary>
        /// <param name="cache">The cache component.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="pipeline">The pipeline component.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        /// <param name="allowNullInjection"><see langword="true"/> if <see langword="null"/> is allowed as injected value; otherwise, <see langword="false"/>.</param>
        /// <param name="detectCyclicDependencies"><see langword="true"/> if cyclic dependencies should be detected; otherwise, <see langword="false"/>.</param>
        public ContextFactory(ICache cache, IPlanner planner, IPipeline pipeline, IExceptionFormatter exceptionFormatter, bool allowNullInjection, bool detectCyclicDependencies)
        {
            this.cache = cache;
            this.planner = planner;
            this.pipeline = pipeline;
            this.exceptionFormatter = exceptionFormatter;
            this.AllowNullInjection = allowNullInjection;
            this.DetectCyclicDependencies = detectCyclicDependencies;
        }

        /// <summary>
        /// Gets a value indicating whether <see langword="null"/> is a valid value for injection.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if <see langword="null"/> is allowed as injected value; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// When <see langword="false"/>, an <see cref="ActivationException"/> is thrown whenever a provider returns <see langword="null"/>.
        /// </remarks>
        public bool AllowNullInjection { get; }

        /// <summary>
        /// Gets a value whether cyclic dependencies should be detected.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if cyclic dependencies should be detected; otherwise, <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// When <see langword="true"/>, an <see cref="ActivationException"/> is thrown whenever a cyclic dependency
        /// is detected.
        /// </para>
        /// <para>
        /// When <see cref="DetectCyclicDependencies"/> is <see langword="false"/>, the CLR may throw a
        /// <see cref="StackOverflowException"/> and terminate the process in case of cyclic dependencies.
        /// </para>
        /// </remarks>
        public bool DetectCyclicDependencies { get; }

        public IContext Create(IReadOnlyKernel kernel, IRequest request, IBinding binding)
        {
            return new Context(kernel, 
                               request,
                               binding,
                               this.cache,
                               this.planner,
                               this.pipeline,
                               this.exceptionFormatter,
                               this.AllowNullInjection,
                               this.DetectCyclicDependencies);
        }
    }
}
