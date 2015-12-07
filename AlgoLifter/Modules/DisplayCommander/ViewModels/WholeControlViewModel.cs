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
using Prism.Events;
using AlgoLifter.Infrastructure;

namespace AlgoLifter.Modules.DisplayCommander.ViewModels
{
    public class WholeControlViewModel : BindableBase
    {
        public ObservableCollection<string> availablePorts { get; set; }
        public ObservableCollection<Models.StepperStatus> Stepper_statuses { get; set; }
        public string selectedPort { get; private set; }
        public ICommand ConnectToPortCommand { get; private set; }
        public ICommand DisconnectPortCommand { get; private set; }

        private readonly IPortCommunicator comport;
        private readonly ICommandBuilder commandBuilder;
        private IEventAggregator eventaggregator;

        public WholeControlViewModel()
        {
            comport = ServiceLocator.Current.GetInstance<IPortCommunicator>();
            commandBuilder = ServiceLocator.Current.GetInstance<ICommandBuilder>();
            eventaggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            this.ConnectToPortCommand = new DelegateCommand(ConnectToPort, () => !comport.isOpen());
            DisconnectPortCommand = new DelegateCommand(DisconnectPort, () => comport.isOpen());
            availablePorts = new ObservableCollection<string>(comport.getComPorts());
            selectedPort = availablePorts.FirstOrDefault();
            eventaggregator.GetEvent<SerialDataReceivedEvent>().Subscribe(OnDataReceive);
        }

        private void OnDataReceive(bool obj)
        {
            TMCLReturnStatus status = commandBuilder.GetReturnStatus(comport.recievedData());
        }

        private void DisconnectPort()
        {
            if (comport.isOpen())
                comport.closeComPort();
        }

        private void ConnectToPort()
        {
            if (selectedPort == null) return;
            comport.setComPort(selectedPort);


        }
    }
}
