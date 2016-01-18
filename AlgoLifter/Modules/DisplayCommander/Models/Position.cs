using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AlgoLifter.Annotations;
using Prism.Commands;

namespace AlgoLifter.Modules.DisplayCommander.Models
{
    public class Position : INotifyPropertyChanged
    {
        private int microsteps;
        private double distance;
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

        public Position()
        {
            RemoveFromCollectionCommand = new DelegateCommand(RemoveFromCollection);
            GoToPositionCommand = new DelegateCommand(GoToPosition);
        }

        private void GoToPosition()
        {
            throw new NotImplementedException();
        }

        private void RemoveFromCollection()
        {
            throw new NotImplementedException();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void On_property_changed([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}