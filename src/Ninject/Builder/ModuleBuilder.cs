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

using System.Collections.Generic;
using Ninject.Modules;

namespace Ninject.Builder
{
    internal class ModuleBuilder : IModuleBuilder
    {
        private List<INinjectBuilderModule> _modules;

        public ModuleBuilder()
        {
            _modules = new List<INinjectBuilderModule>();
        }

        public void Load(params INinjectBuilderModule[] modules)
        {
            for (var i = 0; i < modules.Length; i++)
            {
                _modules.Add(modules[i]);
            }
        }

        public void Load(IEnumerable<INinjectBuilderModule> modules)
        {
            foreach (var module in modules)
            {
                _modules.Add(module);
            }
        }

        public void Build(IKernelConfiguration kernelConfiguration)
        {
            var moduleCount = _modules.Count;
            for (var i = 0; i < moduleCount; i++)
            {
                _modules[i].Load(kernelConfiguration);
            }
        }
    }
}
