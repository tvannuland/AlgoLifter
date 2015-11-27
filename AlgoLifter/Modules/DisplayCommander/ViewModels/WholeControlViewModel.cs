using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLifter.Modules.DisplayCommander.ViewModels
{
    public class WholeControlViewModel
    {
        public List<string> availablePorts { get; set; }

        private Infrastructure.IPortCommunicator comport;

        public WholeControlViewModel()
        {
            comport = ServiceLocator.Current.GetInstance<Infrastructure.IPortCommunicator>();
            availablePorts = comport.getComPorts();
        }
    }
}
