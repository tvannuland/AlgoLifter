using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLifter.Modules.RS485Port
{
    public class RS485PortModule : IModule
    {
        IUnityContainer container;

        public RS485PortModule()
        {
            container = ServiceLocator.Current.GetInstance<IUnityContainer>();
        }

        public void Initialize()
        {
            var Port = new Services.RS485Communicator();
            container.RegisterType<Infrastructure.IPortCommunicator, Services.RS485Communicator>(new ContainerControlledLifetimeManager());
            //container.RegisterInstance<Infrastructure.IPortCommunicator>(Port, new ContainerControlledLifetimeManager());
        }
    }
}
