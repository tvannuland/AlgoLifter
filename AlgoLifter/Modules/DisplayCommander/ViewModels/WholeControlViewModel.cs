using Microsoft.Practices.ServiceLocation;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;

namespace AlgoLifter.Modules.DisplayCommander.ViewModels
{
    public class WholeControlViewModel : BindableBase
    {
        public ObservableCollection<string> availablePorts { get; set; }
        public ObservableCollection<Models.StepperStatus> Stepper_statuses { get; set; }
        public string selectedPort { get; private set; }
        public ICommand ConnectToPortCommand { get; private set; }
        public ICommand DisconnectPortCommand { get; private set; }

        private Infrastructure.IPortCommunicator comport;
        private Infrastructure.ICommandBuilder commandBuilder;

        public WholeControlViewModel()
        {
            comport = ServiceLocator.Current.GetInstance<Infrastructure.IPortCommunicator>();
            commandBuilder = ServiceLocator.Current.GetInstance<Infrastructure.ICommandBuilder>();
            ConnectToPortCommand = new DelegateCommand(ConnectToPort,() => !comport.isOpen());
            DisconnectPortCommand = new DelegateCommand(DisconnectPort, () => comport.isOpen());
            availablePorts = new ObservableCollection<string>(comport.getComPorts());
            selectedPort = availablePorts.FirstOrDefault();
        }

        private void DisconnectPort()
        {
            throw new NotImplementedException();
        }

        private void ConnectToPort()
        {
            if (selectedPort == null) return;
            comport.setComPort(selectedPort);


        }
    }
}
