using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace AlgoLifter.Modules.TMCLCommands
{
    public class TMCLCommandModule : IModule
    {
        private IUnityContainer container;

        public void Initialize()
        {
            container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            container.RegisterType<Infrastructure.ICommandBuilder, Services.TMCLCommandBuilder>(
                new ContainerControlledLifetimeManager());
        }
    }
}
