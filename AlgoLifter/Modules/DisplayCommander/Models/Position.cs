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
        private double distance;
        private int microstepResolution;
        public event PropertyChangedEventHandler PropertyChanged;

        public double Distance
        {
            get { return distance; }
            set
            {
                if (value.Equals(distance)) return;
                distance = value;
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
                On_property_changed();
            }
        }

        private EventAggregator ea;

        public Position()
        {
            RemoveFromCollectionCommand = new DelegateCommand(RemoveFromCollection);
            GoToPositionCommand = new DelegateCommand(GoToPosition);
            ea = ServiceLocator.Current.GetInstance<EventAggregator>();
        }

        private void GoToPosition()
        {
            ea.GetEvent<MoveToPositionEvent>().Publish(microsteps);
        }

        private void RemoveFromCollection()
        {
            ea.GetEvent<RemovePositionFromListEvent>().Publish(this);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void On_property_changed([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}