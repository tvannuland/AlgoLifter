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
