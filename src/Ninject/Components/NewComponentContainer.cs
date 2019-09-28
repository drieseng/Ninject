using Ninject.Builder;
using System;
using System.Collections.Generic;

namespace Ninject.Components
{
    internal class NewComponentContainer : IComponentContainer
    {
        private readonly ComponentBindingRoot root;
        private IReadOnlyKernel componentKernel;

        public NewComponentContainer(ComponentBindingRoot root)
        {
            this.root = root;
        }

        private IReadOnlyKernel ComponentKernel
        {
            get
            {
                if (this.componentKernel == null)
                {
                    this.componentKernel = BuildComponentKernel();
                }

                return this.componentKernel;
            }
        }

        public void Add<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : INinjectComponent, TComponent
        {
            root.Bind<TComponent>().To<TImplementation>().InSingletonScope();
        }

        internal void Add<TComponent>(TComponent instance)
        {
            root.Bind<TComponent>().ToConstant(instance);
        }

        public void AddTransient<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : INinjectComponent, TComponent
        {
            root.Bind<TComponent>().To<TImplementation>().InTransientScope();
        }

        public void Dispose()
        {
        }

        public T Get<T>() where T : INinjectComponent
        {
            return ComponentKernel.Get<T>();
        }

        public object Get(Type component)
        {
            return ComponentKernel.Get(component);
        }

        public IEnumerable<T> GetAll<T>() where T : INinjectComponent
        {
            return ComponentKernel.GetAll<T>();
        }

        public IEnumerable<object> GetAll(Type component)
        {
            return ComponentKernel.GetAll(component);
        }

        public void Remove<T, TImplementation>()
            where T : INinjectComponent
            where TImplementation : T
        {
            throw new NotImplementedException();
        }

        public void RemoveAll<T>() where T : INinjectComponent
        {
            throw new NotImplementedException();
        }

        public void RemoveAll(Type component)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyKernel BuildComponentKernel()
        {
            var resolveComponentsKernel = new BuilderKernelFactory().CreateResolveComponentBindingsKernel();

            var componentBindingVisitor = new BindingBuilderVisitor();
            this.root.Build(resolveComponentsKernel, componentBindingVisitor);
            var componentBindings = componentBindingVisitor.Bindings;

            return new BuilderKernelFactory().CreateComponentsKernel(resolveComponentsKernel, componentBindings);
        }
    }
}
