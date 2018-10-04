
namespace SIRO.Core.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using StructureMap;

    /// <summary>
    /// The structure map dependency resolver.
    /// </summary>
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public StructureMapDependencyResolver(IContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// The get service.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (serviceType.IsInterface || serviceType.IsAbstract)
            {
                return this.GetInterfaceService(serviceType);
            }
            return this.GetConcreteService(serviceType);
        }

        /// <summary>
        /// The get concrete service.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object GetConcreteService(Type serviceType)
        {
            try
            {
                // Can't use TryGetInstance here because it won’t create concrete types
                return this.container.GetInstance(serviceType);
            }
            catch (StructureMapException)
            {
                return null;
            }
        }

        /// <summary>
        /// The get interface service.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object GetInterfaceService(Type serviceType)
        {
            return this.container.TryGetInstance(serviceType);
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.container.GetAllInstances(serviceType).Cast<object>();
        }
    }
}
