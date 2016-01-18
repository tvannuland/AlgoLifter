using System;
using System.Collections.ObjectModel;
using System.Linq;
using AlgoLifter.Infrastructure;
using AlgoLifter.Modules.DisplayCommander.Models;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace AlgoLifter.Modules.DisplayCommander.ViewModels
{
    public class WholeControlViewModel : BindableBase
    {
        public ObservableCollection<string> availablePorts { get; set; }
        public ObservableCollection<StepperStatus> Stepper_statuses { get; set; }
        public ObservableCollection<Position> Positions { get; set; }
        public string selectedPort { get; set; }
        public DelegateCommand ConnectToPortCommand { get; }
        public DelegateCommand DisconnectPortCommand { get; }
        public DelegateCommand GoUpCommand { get; }
        public DelegateCommand GoDownCommand { get; }
        public DelegateCommand MotionStopCommand { get; }
        public DelegateCommand MoveToZeroCommand { get; }
        public DelegateCommand MicrostepSelectionChange { get; }
        public DelegateCommand RampDividerChange { get; }
        public DelegateCommand PulseDividerChange { get; }
        public DelegateCommand AddPositionCommand { get; }

        public int[] Divider { get; set; } = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        public int[] Microsteps { get; set; } = { 1, 2, 4, 8, 16, 32, 64, 128, 256 };

        public int PulseDivider { get; set; } = 0;
        public int RampDivider { get; set; } = 0;
        public int SelectedMicrosteps { get; set; } = 256;
        public int Position { get; set; }

        public int Acceleration
        {
            get { return acceleration; }
            set
            {
                if (value < 0) value = 0;
                if (value > 2047) value = 2047;
                if (Stepper_statuses.Count > 0) {
                    foreach (var stepper in Stepper_statuses) {
                        var command = commandBuilder.SetAcceleration(stepper.id, value);
                        var answer = comport.sendData(command);
                        stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                    }
                }
                SetProperty(ref acceleration, value);
            }
        }

        public int Speed
        {
            get { return speed; }
            set
            {
                if (value < 0) value = 0;
                if (value > 2047) value = 2047;
                if (Stepper_statuses.Count > 0) {
                    foreach (var stepper in Stepper_statuses) {
                        var command = commandBuilder.SetSpeed(stepper.id, value);
                        var answer = comport.sendData(command);
                        stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                    }
                }
                SetProperty(ref speed, value);
            }
        }

        private readonly IPortCommunicator comport;
        private readonly ICommandBuilder commandBuilder;
        private readonly IEventAggregator eventaggregator;
        private int acceleration = 0; 
        private int speed = 0;

        public WholeControlViewModel()
        {
            comport = ServiceLocator.Current.GetInstance<IPortCommunicator>();
            commandBuilder = ServiceLocator.Current.GetInstance<ICommandBuilder>();
            eventaggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            Stepper_statuses = new ObservableCollection<StepperStatus>();
            availablePorts = new ObservableCollection<string>(comport.getComPorts());

            ConnectToPortCommand = new DelegateCommand(ConnectToPort, () => !comport.isOpen());
            DisconnectPortCommand = new DelegateCommand(DisconnectPort, () => comport.isOpen());
            GoUpCommand = new DelegateCommand(GoUp, () => (Stepper_statuses.Count > 0));
            GoDownCommand = new DelegateCommand(GoDown, () => (Stepper_statuses.Count > 0));
            MotionStopCommand = new DelegateCommand(StopMotion, () => (Stepper_statuses.Count > 0));
            MoveToZeroCommand = new DelegateCommand(MoveToZero, () => (Stepper_statuses.Count > 0));
            MicrostepSelectionChange = new DelegateCommand(onMicrostepSelectionChange);
            RampDividerChange = new DelegateCommand(onRampDividerChange);
            PulseDividerChange = new DelegateCommand(onPulseDividerChange);
            AddPositionCommand = new DelegateCommand(AddPosition);
            Positions = new ObservableCollection<Position>();

            selectedPort = availablePorts.FirstOrDefault();
            //eventaggregator.GetEvent<SerialDataReceivedEvent>().Subscribe(OnDataReceive);
        }

        private void onPulseDividerChange()
        {
            if (Stepper_statuses.Count > 0) {
                foreach (var stepper in Stepper_statuses) {
                    var command = commandBuilder.SetSpeedDivider(stepper.id, PulseDivider);
                    var answer = comport.sendData(command);
                    stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                }
            }
        }

        private void onRampDividerChange()
        {
            if (Stepper_statuses.Count > 0) {
                foreach (var stepper in Stepper_statuses) {
                    var command = commandBuilder.SetRampDivider(stepper.id, RampDivider);
                    var answer = comport.sendData(command);
                    stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                }
            }
        }

        private void onMicrostepSelectionChange()
        {
            Position = (int) Math.Round((Math.Log10(SelectedMicrosteps)/Math.Log10(2)));
            OnPropertyChanged("Position");
            if (Stepper_statuses.Count > 0)
            {
                foreach (var stepper in Stepper_statuses)
                {
                    var command = commandBuilder.SetMicrostepResolution(stepper.id, (int) Math.Round(
                        (Math.Log10(SelectedMicrosteps)/Math.Log10(2))));
                    var answer = comport.sendData(command);
                    stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                }
            }
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
                    Stepper_statuses.Add(new StepperStatus
                    {
                        id = i,
                        Version = commandBuilder.ReadFirmwareID(returnmessage)
                    });
            }

            if (Stepper_statuses.Count > 0)
            {
                var stepper = Stepper_statuses.FirstOrDefault();
                var command = commandBuilder.GetMicrostepResolution(stepper.id);
                var answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                SelectedMicrosteps = (int) Math.Pow(2, commandBuilder.ReadValue(answer));
                OnPropertyChanged("SelectedMicrosteps");
                command = commandBuilder.GetRampDivider(stepper.id);
                answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                RampDivider = commandBuilder.ReadValue(answer);
                OnPropertyChanged("RampDivider");
                command = commandBuilder.GetSpeedDivider(stepper.id);
                answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                PulseDivider = commandBuilder.ReadValue(answer);
                OnPropertyChanged("PulseDivider");
                command = commandBuilder.GetSpeed(stepper.id);
                answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                Speed = commandBuilder.ReadValue(answer);
                OnPropertyChanged("Speed");
                command = commandBuilder.GetAcceleration(stepper.id);
                answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                Acceleration = commandBuilder.ReadValue(answer);
                OnPropertyChanged("Acceleration");

                GoUpCommand.RaiseCanExecuteChanged();
                GoDownCommand.RaiseCanExecuteChanged();
                MotionStopCommand.RaiseCanExecuteChanged();
                MoveToZeroCommand.RaiseCanExecuteChanged();
            }
        }

        private void GoUp()
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses)
            {
                var command = commandBuilder.RotateLeft(stepper.id, Speed);
                var answer = comport.sendData(command);
                stepper.Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
            }
        }

        private void GoDown()
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses)
            {
                var command = commandBuilder.RotateRight(stepper.id, Speed);
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

        private void AddPosition()
        {
            if (Stepper_statuses.Count <= 0)
            {
                var position = new Position
                {
                    Microsteps = 0,
                    Distance = 0
                };
                Positions.Add(position);
            } else
            {
                var command = commandBuilder.GetActualPosition(Stepper_statuses.FirstOrDefault().id);
                var answer = comport.sendData(command);
                Stepper_statuses.FirstOrDefault().Status = interpretAnswer(commandBuilder.GetReturnStatus(answer));
                var position = new Position
                {
                    Microsteps = commandBuilder.ReadValue(answer)
                };
                position.Distance = (double) position.Microsteps/(SelectedMicrosteps*200);
                Positions.Add(position);
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
