using System;
using System.Web.Http.Dependencies;
using System.Collections.Generic;

using SimpleInjector;

namespace N_Dexed.Deployment.Configuration
{
    public class DependencyController : IDependencyResolver
    {
        private readonly Container m_Container;

        public DependencyController()
        {
            m_Container = new Container();
        }

        #region IDependencyResolver Methods

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            IServiceProvider provider = m_Container;

            return provider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return m_Container.GetAllInstances(serviceType);
        }

        public void Dispose()
        {
            //no disposable assets exist.  Do nothing.
        }

        #endregion

        #region Public Methods

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            m_Container.Register<TService, TImplementation>();
        }

        public void Register<TConcrete>() where TConcrete : class
        {
            m_Container.Register<TConcrete>();
        }

        public void Register<TService>(Func<TService> instanceCreator) where TService : class
        {
            m_Container.Register(instanceCreator);
        }

        public void RegisterSingle<TService>(Func<TService> instanceCreator) where TService : class
        {
            m_Container.RegisterSingle(instanceCreator);
        }

        public void RegisterSingle<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            m_Container.RegisterSingle<TService, TImplementation>();
        }

        public void RegisterSingle<TService>(TService instance) where TService : class
        {
            m_Container.RegisterSingle(instance);
        }

        #endregion
    }
}
