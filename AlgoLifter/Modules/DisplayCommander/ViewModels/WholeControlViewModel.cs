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
        public int vmin { get; set; }
        public int vmax { get; set; }
        public int vselected { get; set; }
        public int amin { get; set; }
        public int amax { get; set; }
        public int aselected { get; set; }
        public double Position { get; set; }

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
                        stepper.Status = commandBuilder.GetReturnStatus(answer);
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
                        stepper.Status = commandBuilder.GetReturnStatus(answer);
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
            Positions = new ObservableCollection<Models.Position>();

            selectedPort = availablePorts.FirstOrDefault();
            eventaggregator.GetEvent<MoveToPositionEvent>().Subscribe(MoveToPosition, ThreadOption.UIThread);
            eventaggregator.GetEvent<RemovePositionFromListEvent>()
                .Subscribe(RemovePositionFromList, ThreadOption.UIThread);
            //eventaggregator.GetEvent<SerialDataReceivedEvent>().Subscribe(OnDataReceive);
        }

        private void onPulseDividerChange()
        {
            if (Stepper_statuses.Count > 0) {
                foreach (var stepper in Stepper_statuses) {
                    var command = commandBuilder.SetSpeedDivider(stepper.id, PulseDivider);
                    var answer = comport.sendData(command);
                    stepper.Status = commandBuilder.GetReturnStatus(answer);
                }
            }
        }

        private void onRampDividerChange()
        {
            if (Stepper_statuses.Count > 0) {
                foreach (var stepper in Stepper_statuses) {
                    var command = commandBuilder.SetRampDivider(stepper.id, RampDivider);
                    var answer = comport.sendData(command);
                    stepper.Status = commandBuilder.GetReturnStatus(answer);
                }
            }
        }

        private void onMicrostepSelectionChange()
        {
            eventaggregator.GetEvent<NewMicrostepResolutionEvent>().Publish(SelectedMicrosteps);
            //Position = (int) Math.Round((Math.Log10(SelectedMicrosteps)/Math.Log10(2)));
            //OnPropertyChanged("Position");
            if (Stepper_statuses.Count > 0)
            {
                int currentPosition = (int) Math.Round((Position/(Math.PI*1.5)*SelectedMicrosteps*200));
                foreach (var stepper in Stepper_statuses)
                {
                    var command = commandBuilder.SetMicrostepResolution(stepper.id, (int) Math.Round(
                        (Math.Log10(SelectedMicrosteps)/Math.Log10(2))));
                    var answer = comport.sendData(command);
                    stepper.Status = commandBuilder.GetReturnStatus(answer);
                    command = commandBuilder.SetActualPosition(stepper.id, currentPosition);
                    answer = comport.sendData(command);
                    stepper.Status = commandBuilder.GetReturnStatus(answer);
                }
            }
        }

        private void OnDataReceive(bool obj)
        {
            string status = commandBuilder.GetReturnStatus(comport.recievedData());
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

            /*if (Stepper_statuses.Count > 0)
            {
                foreach (var stepperStatus in Stepper_statuses)
                {
                    var command = commandBuilder.SetBaudRate(stepperStatus.id, 7);
                    var answer = comport.sendData(command);
                    stepperStatus.Status = commandBuilder.GetReturnStatus(answer);
                }
                comport.closeComPort();
                comport.setComPort(selectedPort, 115200);
                foreach (var stepper in Stepper_statuses)
                {
                    var command = commandBuilder.GetFirmwareID(stepper.id);
                    var answer = comport.sendData(command);
                    if (answer != null) {
                        stepper.Version = "+" + commandBuilder.ReadFirmwareID(answer);
                    } else return;
                }
            }*/

            if (Stepper_statuses.Count > 0)
            {
                var stepper = Stepper_statuses.FirstOrDefault();
                var command = commandBuilder.GetMicrostepResolution(stepper.id);
                var answer = comport.sendData(command);
                stepper.Status = commandBuilder.GetReturnStatus(answer);
                SelectedMicrosteps = (int) Math.Pow(2, commandBuilder.ReadValue(answer));
                OnPropertyChanged("SelectedMicrosteps");
                command = commandBuilder.GetRampDivider(stepper.id);
                answer = comport.sendData(command);
                stepper.Status = commandBuilder.GetReturnStatus(answer);
                RampDivider = commandBuilder.ReadValue(answer);
                OnPropertyChanged("RampDivider");
                command = commandBuilder.GetSpeedDivider(stepper.id);
                answer = comport.sendData(command);
                stepper.Status = commandBuilder.GetReturnStatus(answer);
                PulseDivider = commandBuilder.ReadValue(answer);
                OnPropertyChanged("PulseDivider");
                command = commandBuilder.GetSpeed(stepper.id);
                answer = comport.sendData(command);
                stepper.Status = commandBuilder.GetReturnStatus(answer);
                Speed = commandBuilder.ReadValue(answer);
                OnPropertyChanged("Speed");
                command = commandBuilder.GetAcceleration(stepper.id);
                answer = comport.sendData(command);
                stepper.Status = commandBuilder.GetReturnStatus(answer);
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
                stepper.Status = commandBuilder.GetReturnStatus(answer);
            }
        }

        private void GoDown()
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses)
            {
                var command = commandBuilder.RotateRight(stepper.id, Speed);
                var answer = comport.sendData(command);
                stepper.Status = commandBuilder.GetReturnStatus(answer);
            }
        }

        private void StopMotion()
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses)
            {
                var command = commandBuilder.StopMotion(stepper.id);
                var answer = comport.sendData(command);
                stepper.Status = commandBuilder.GetReturnStatus(answer);
            }
            var answered = comport.sendData(commandBuilder.GetActualPosition(Stepper_statuses.FirstOrDefault().id));
            Position = ((double)commandBuilder.ReadValue(answered)/(SelectedMicrosteps*200))*Math.PI*1.5;
            OnPropertyChanged("Position");
        }

        private void MoveToZero()
        {
            MoveToPosition(0);
        }

        private void MoveToPosition(int position)
        {
            if (Stepper_statuses.Count <= 0) return;
            foreach (var stepper in Stepper_statuses) {
                var command = commandBuilder.MoveToPosition(stepper.id, position);
                var answer = comport.sendData(command);
                stepper.Status = commandBuilder.GetReturnStatus(answer);
            }
            Position = ((double) position/(SelectedMicrosteps*200))*Math.PI*1.5;
            OnPropertyChanged("Position");
        }

        private void AddPosition()
        {
            if (Stepper_statuses.Count <= 0)
            {
                var position = new Position
                {
                    Microsteps = 0,
                    Revolutions = 0,
                    MicrostepResolution = SelectedMicrosteps
                };
                Positions.Add(position);
            } else
            {
                var command = commandBuilder.GetActualPosition(Stepper_statuses.FirstOrDefault().id);
                var answer = comport.sendData(command);
                Stepper_statuses.FirstOrDefault().Status = commandBuilder.GetReturnStatus(answer);
                var position = new Position
                {
                    Microsteps = commandBuilder.ReadValue(answer)
                };
                position.Revolutions = (double) position.Microsteps/(SelectedMicrosteps*200);
                position.MicrostepResolution = SelectedMicrosteps;
                Positions.Add(position);
            }
        }

        private void RemovePositionFromList(Position thePositionWereTalkingAbout)
        {
            if ((Positions.Count > 0) && (Positions.Contains(thePositionWereTalkingAbout)))
            {
                Positions.Remove(thePositionWereTalkingAbout);
            }
        }

        private int GetVmin()
        {
            return 1;
        }

        private int GetVmax()
        {
            return 2047;
        }
    }
}
