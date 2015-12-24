using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AlgoLifter.Annotations;
using Prism.Mvvm;

namespace AlgoLifter.Modules.DisplayCommander.Models
{
    public class StepperStatus : INotifyPropertyChanged
    {
        private int id1;
        private string version;
        private string status;

        public int id
        {
            get { return id1; }
            set
            {
                if (value == id1) return;
                id1 = value;
                On_property_changed();
            }
        }

        public string Version
        {
            get { return version; }
            set
            {
                if (value == version) return;
                version = value;
                On_property_changed();
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                if (value == status) return;
                status = value;
                On_property_changed();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void On_property_changed([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
