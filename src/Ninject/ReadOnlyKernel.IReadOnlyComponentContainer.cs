using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject
{
    internal class ReadOnlyKernel5 : IComponentContainerNew
    {
        /// <summary>
        /// Gets an instance of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>
        /// An instance of the component.
        /// </returns>
        T IComponentContainerNew.Get<T>()
        {
            var request = this.CreateRequest(typeof(T), null, Array.Empty<IParameter>(), false, true);
            return (T)this.ResolveSingle(request);
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>
        /// A series of instances of the specified component.
        /// </returns>
        IEnumerable<T> IComponentContainerNew.GetAll<T>()
        {
            var request = this.CreateRequest(typeof(T), null, Array.Empty<IParameter>(), true, false);
            return this.ResolveAll(request, true).Cast<T>();
        }

        /// <summary>
        /// Gets an instance of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>
        /// The instance of the component.
        /// </returns>
        object IComponentContainerNew.Get(Type component)
        {
            var request = this.CreateRequest(component, null, Array.Empty<IParameter>(), false, true);
            return this.ResolveSingle(request);
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>
        /// A series of instances of the specified component.
        /// </returns>
        IEnumerable<object> IComponentContainerNew.GetAll(Type component)
        {
            var request = this.CreateRequest(component, null, Array.Empty<IParameter>(), true, false);
            return this.ResolveAll(request, true);
        }
    }
}
