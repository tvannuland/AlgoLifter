using Microsoft.Practices.ServiceLocation;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLifter.Modules.DisplayCommander.ViewModels
{
    public class WholeControlViewModel : BindableBase
    {
        public ObservableCollection<string> availablePorts { get; set; }
        public string selectedPort { get; private set; }

        private Infrastructure.IPortCommunicator comport;

        public WholeControlViewModel()
        {
            comport = ServiceLocator.Current.GetInstance<Infrastructure.IPortCommunicator>();
            availablePorts = new ObservableCollection<string>(comport.getComPorts());
            selectedPort = availablePorts.FirstOrDefault();
        }
    }
}
