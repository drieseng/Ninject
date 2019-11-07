namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class ReadOnlyKernel
    {
        internal IComponentContainer AsComponentContainer()
        {
            return new ReadOnlyKernelComponentContainerAdapter(this);
        }

        private class ReadOnlyKernelComponentContainerAdapter : IComponentContainer
        {
            void IComponentContainer.Add<TComponent, TImplementation>()
            {
                this.exceptionFormatter;
            }

            void IComponentContainer.RemoveAll<T>()
            {
                throw new NotImplementedException();
            }

            void IComponentContainer.RemoveAll(Type component)
            {
                throw new NotImplementedException();
            }

            void IComponentContainer.Remove<T, TImplementation>()
            {
                throw new NotImplementedException();
            }

            T IComponentContainer.Get<T>()
            {
                throw new NotImplementedException();
            }

            IEnumerable<T> IComponentContainer.GetAll<T>()
            {
                throw new NotImplementedException();
            }

            object IComponentContainer.Get(Type component)
            {
                throw new NotImplementedException();
            }

            IEnumerable<object> IComponentContainer.GetAll(Type component)
            {
                throw new NotImplementedException();
            }

            void IComponentContainer.AddTransient<TComponent, TImplementation>()
            {
                throw new NotImplementedException();
            }
        }
    }
}
