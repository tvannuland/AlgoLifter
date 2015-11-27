using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Unity;
using Prism.Modularity;

namespace AlgoLifter
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override ILoggerFacade CreateLogger()
        {
            return base.CreateLogger();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            ModuleCatalog catalog = new ModuleCatalog();
            catalog.AddModule(new ModuleInfo()
            {
                ModuleName = typeof(Modules.RS485Port.RS485PortModule).Name,
                ModuleType = typeof(Modules.RS485Port.RS485PortModule).AssemblyQualifiedName
            });
            catalog.AddModule(new ModuleInfo()
            {
                ModuleName = typeof(Modules.DisplayCommander.DisplayCommanderModule).Name,
                ModuleType = typeof(Modules.DisplayCommander.DisplayCommanderModule).AssemblyQualifiedName
            });

            return catalog;
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
