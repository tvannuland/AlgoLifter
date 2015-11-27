using AlgoLifter.Modules.DisplayCommander.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLifter.Modules.DisplayCommander
{
    public class DisplayCommanderModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer container;

        public void Initialize()
        {
            container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

            container.RegisterType<WholeControlViewModel, WholeControlViewModel>(new ContainerControlledLifetimeManager());

            regionManager.RegisterViewWithRegion("MainWindow", typeof(Views.WholeControlView));
        }
    }
}
