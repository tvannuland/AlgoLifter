using Microsoft.Practices.ServiceLocation;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Events;
using AlgoLifter.Infrastructure;

namespace AlgoLifter.Modules.DisplayCommander.ViewModels
{
    public class WholeControlViewModel : BindableBase
    {
        public ObservableCollection<string> availablePorts { get; set; }
        public ObservableCollection<Models.StepperStatus> Stepper_statuses { get; set; }
        public string selectedPort { get; }
        public DelegateCommand ConnectToPortCommand { get; }
        public DelegateCommand DisconnectPortCommand { get; }

        private readonly IPortCommunicator comport;
        private readonly ICommandBuilder commandBuilder;
        private readonly IEventAggregator eventaggregator;

        public WholeControlViewModel()
        {
            comport = ServiceLocator.Current.GetInstance<IPortCommunicator>();
            commandBuilder = ServiceLocator.Current.GetInstance<ICommandBuilder>();
            eventaggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            ConnectToPortCommand = new DelegateCommand(ConnectToPort, () => !comport.isOpen());
            DisconnectPortCommand = new DelegateCommand(DisconnectPort, () => comport.isOpen());
            availablePorts = new ObservableCollection<string>(comport.getComPorts());
            selectedPort = availablePorts.FirstOrDefault();
            Stepper_statuses = new ObservableCollection<Models.StepperStatus>();
            //eventaggregator.GetEvent<SerialDataReceivedEvent>().Subscribe(OnDataReceive);
        }

        private void OnDataReceive(bool obj)
        {
            TMCLReturnStatus status = commandBuilder.GetReturnStatus(comport.recievedData());
        }

        private void DisconnectPort()
        {
            if (comport.isOpen())
                comport.closeComPort();
            DisconnectPortCommand.RaiseCanExecuteChanged();
            ConnectToPortCommand.RaiseCanExecuteChanged();
            Stepper_statuses.Clear();
        }

        private void ConnectToPort()
        {
            if (selectedPort == null) return;
            comport.setComPort(selectedPort);
            DisconnectPortCommand.RaiseCanExecuteChanged();
            ConnectToPortCommand.RaiseCanExecuteChanged();
            for (var i = 0; i < 10; i++)
            {
                var command = commandBuilder.GetFirmwareID(i);
                var returnmessage = comport.sendData(command);
                if (returnmessage != null)
                    Stepper_statuses.Add(new Models.StepperStatus()
                    {
                        id = i,
                        Version = commandBuilder.ReadFirmwareID(returnmessage),
                        Status = commandBuilder.ReadFirmwareID(returnmessage)
                    });
            }
        }
    }
}
