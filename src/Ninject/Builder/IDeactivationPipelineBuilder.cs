using System;
using System.ComponentModel;

namespace Ninject.Builder
{
    public interface IDeactivationPipelineBuilder
    {
        IDeactivationPipelineBuilder BindingAction();

        IDeactivationPipelineBuilder Disposable();

        IDeactivationPipelineBuilder Stoppable();
    }
}
