using System;
using System.ComponentModel;

namespace Ninject.Builder
{
    public interface IActivationPipelineBuilder
    {
        IActivationPipelineBuilder Startable();

        IActivationPipelineBuilder BindingAction();

        [EditorBrowsable(EditorBrowsableState.Never)]
        IActivationPipelineBuilder AddStage(Func<IComponentBuilder> componentDelegate);
    }
}
