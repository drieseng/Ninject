using System;
using System.ComponentModel;

namespace Ninject.Builder
{
    public interface IDeactivationPipelineBuilder
    {
        IDeactivationPipelineBuilder BindingAction();

        IDeactivationPipelineBuilder Disposable();

        [EditorBrowsable(EditorBrowsableState.Never)]
        IDeactivationPipelineBuilder AddStage(Func<IComponentBuilder> componentDelegate);
    }
}
