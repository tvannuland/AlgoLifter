using Microsoft.Practices.ServiceLocation;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Events;
using AlgoLifter.Infrastructure;
using System;

namespace AlgoLifter.Modules.DisplayCommander.ViewModels
{
    public class WholeControlViewModel : BindableBase
    {
        public ObservableCollection<string> availablePorts { get; set; }
        public ObservableCollection<Models.StepperStatus> Stepper_statuses { get; set; }
        public string selectedPort { get; }
        public DelegateCommand ConnectToPortCommand { get; }
        public DelegateCommand DisconnectPortCommand { get; }
        public DelegateCommand GoUpCommand { get; }
        public DelegateCommand GoDownCommand { get; }
        public DelegateCommand MotionStopCommand { get; }
        public DelegateCommand MoveToZeroCommand { get; }

        private readonly IPortCommunicator comport;
        private readonly ICommandBuilder commandBuilder;
        private readonly IEventAggregator eventaggregator;

        public WholeControlViewModel()
        {
            comport = ServiceLocator.Current.GetInstance<IPortCommunicator>();
            commandBuilder = ServiceLocator.Current.GetInstance<ICommandBuilder>();
            eventaggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            Stepper_statuses = new ObservableCollection<Models.StepperStatus>();
            availablePorts = new ObservableCollection<string>(comport.getComPorts());

            ConnectToPortCommand = new DelegateCommand(ConnectToPort, () => !comport.isOpen());
            DisconnectPortCommand = new DelegateCommand(DisconnectPort, () => comport.isOpen());
            GoUpCommand = new DelegateCommand(GoUp, () => (Stepper_statuses.Count > 0));
            GoDownCommand = new DelegateCommand(GoDown, () => (Stepper_statuses.Count > 0));
            MotionStopCommand = new DelegateCommand(StopMotion, () => (Stepper_statuses.Count > 0));
            MoveToZeroCommand = new DelegateCommand(MoveToZero, () => (Stepper_statuses.Count > 0));

            selectedPort = availablePorts.FirstOrDefault();
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
            GoUpCommand.RaiseCanExecuteChanged();
            GoDownCommand.RaiseCanExecuteChanged();
            MotionStopCommand.RaiseCanExecuteChanged();
            MoveToZeroCommand.RaiseCanExecuteChanged();
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
                        Version = commandBuilder.ReadFirmwareID(returnmessage)
                    });
            }
            GoUpCommand.RaiseCanExecuteChanged();
            GoDownCommand.RaiseCanExecuteChanged();
            MotionStopCommand.RaiseCanExecuteChanged();
            MoveToZeroCommand.RaiseCanExecuteChanged();
        }

        private void GoUp()
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses)
            {
                var command = commandBuilder.RotateLeft(stepper.id, 1000);
                var answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
            }
        }

        private void GoDown()
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses)
            {
                var command = commandBuilder.RotateRight(stepper.id, 2000);
                var answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
            }
        }

        private void StopMotion()
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses)
            {
                var command = commandBuilder.StopMotion(stepper.id);
                var answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
            }
        }

        private void MoveToZero()
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses)
            {
                var command = commandBuilder.MoveToPosition(stepper.id, 0);
                var answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
            }
        }

        private string interpretAnswer(TMCLReturnStatus answer)
        {
            switch (answer)
            {
                case TMCLReturnStatus.WrongChecksum:
                    return "bad Checksum";
                case TMCLReturnStatus.InvalidCommand:
                    return "invalid Command";
                case TMCLReturnStatus.WrongType:
                    return "Type?";
                case TMCLReturnStatus.InvalidValue:
                    return "bad Value";
                case TMCLReturnStatus.EEPROMlocked:
                    return "locked";
                case TMCLReturnStatus.CommandNotAvailable:
                    return "Command n/a";
                case TMCLReturnStatus.Success:
                    return "OK";
                case TMCLReturnStatus.LoadedToEEPROM:
                    return "loaded";
                default:
                    throw new ArgumentOutOfRangeException(nameof(answer), answer, null);
            }
        }
    }
}
