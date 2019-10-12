// -------------------------------------------------------------------------------------------------
// <copyright file="ModuleBuilder.cs" company="Ninject Project Contributors">
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
    using System;
    using System.Collections.Generic;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Modules;

    internal class ModuleLoader : IModuleLoader
    {
        private readonly Dictionary<string, INinjectModule> _modulesByName;
        private readonly IKernelConfiguration _kernelConfiguration;
        private readonly IExceptionFormatter _exceptionFormatter;

        public ModuleLoader(IKernelConfiguration kernelConfiguration, IExceptionFormatter exceptionFormatter)
        {
            _modulesByName = new Dictionary<string, INinjectModule>();
            _kernelConfiguration = kernelConfiguration;
            _exceptionFormatter = exceptionFormatter;
        }

        /// <summary>
        /// Gets the loaded modules.
        /// </summary>
        /// <value>
        /// The loaded modules.
        /// </value>
        public IReadOnlyCollection<INinjectModule> Modules
        {
            get { return _modulesByName.Values; }
        }

        /// <summary>
        /// Determines whether a module with the specified name has been loaded in the kernel.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        /// <returns>
        /// <see langword="true"/> if the specified module has been loaded; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public bool HasModule(string name)
        {
            Ensure.ArgumentNotNull(name, nameof(name));

            return _modulesByName.ContainsKey(name);
        }

        /// <summary>
        /// Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="modules">The modules to load.</param>
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is <see langword="null"/>.</exception>
        public void Load(params INinjectModule[] modules)
        {
            Ensure.ArgumentNotNull(modules, nameof(modules));

            for (var i = 0; i < modules.Length; i++)
            {
                LoadModule(modules[i]);
            }
        }

        public void Load(IEnumerable<INinjectModule> modules)
        {
            Ensure.ArgumentNotNull(modules, nameof(modules));

            foreach (var module in modules)
            {
                LoadModule(module);
            }
        }

        public void Complete()
        {
            foreach (var entry in _modulesByName)
            {
                entry.Value.OnLoadCompleted(_kernelConfiguration);
            }
        }

        private void LoadModule(INinjectModule module)
        {
            if (module.Name == null)
            {
                throw new NotSupportedException(_exceptionFormatter.ModulesWithNullNameAreNotSupported());
            }

            if (_modulesByName.TryGetValue(module.Name, out INinjectModule existingModule))
            {
                throw new NotSupportedException(_exceptionFormatter.ModuleWithSameNameIsAlreadyLoaded(module, existingModule));
            }

            _modulesByName.Add(module.Name, module);
            module.OnLoad(_kernelConfiguration);
        }
    }
}
