using Ninject.Planning.Bindings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.Activation
{
    public interface IContextFactory
    {
        IContext Create(IReadOnlyKernel kernel, IRequest request, IBinding binding);
    }
}
