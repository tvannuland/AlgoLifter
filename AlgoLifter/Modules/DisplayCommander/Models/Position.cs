using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AlgoLifter.Annotations;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using AlgoLifter.Infrastructure;

namespace AlgoLifter.Modules.DisplayCommander.Models
{
    public class Position : INotifyPropertyChanged
    {
        private int microsteps;
        private double revolutions;
        private string distance;
        private int microstepResolution;

        public string Distance
        {
            get { return distance; }
            set
            {
                if (value.Equals(distance)) return;
                distance = value;
                On_property_changed();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double Revolutions
        {
            get { return revolutions; }
            set
            {
                if (value.Equals(revolutions)) return;
                revolutions = value;
                Microsteps = (int) (revolutions*microstepResolution*200);
                Distance = string.Format("{0:0.00} cm", 1.5*Math.PI*revolutions);
                On_property_changed();
            }
        }

        public DelegateCommand RemoveFromCollectionCommand { get; }
        public DelegateCommand GoToPositionCommand { get; }

        public int Microsteps
        {
            get { return microsteps; }
            set
            {
                if (value == microsteps) return;
                microsteps = value;
                On_property_changed();
            }
        }

        public int MicrostepResolution
        {
            get
            {
                return microstepResolution;
            }

            set
            {
                if (value == microstepResolution) return;
                microstepResolution = value;
                Microsteps = (int) (revolutions*microstepResolution*200);
                On_property_changed();
            }
        }

        private EventAggregator ea;

        public Position()
        {
            RemoveFromCollectionCommand = new DelegateCommand(RemoveFromCollection);
            GoToPositionCommand = new DelegateCommand(GoToPosition);
            ea = ServiceLocator.Current.GetInstance<EventAggregator>();
            ea.GetEvent<NewMicrostepResolutionEvent>()
                .Subscribe(onMicrostepResolutionChanged, ThreadOption.BackgroundThread);
        }

        private void GoToPosition()
        {
            ea.GetEvent<MoveToPositionEvent>().Publish(microsteps);
        }

        private void RemoveFromCollection()
        {
            ea.GetEvent<RemovePositionFromListEvent>().Publish(this);
        }

        private void onMicrostepResolutionChanged(int resolution)
        {
            MicrostepResolution = resolution;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void On_property_changed([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}