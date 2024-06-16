using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace DataConcentrator.src
{
    public class AnalogInput : Input, INotifyPropertyChanged
    {
        private double _value;
        [NotMapped]
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        [Required]
        public double LowLimit { get; set; }

        [Required]
        public double HighLimit { get; set; }

        [Required]
        public string Units { get; set; }

        [Required]
        public List<Alarm> Alarms { get; set; } = new List<Alarm>();

        public Thread Scanner;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Scan(PLCManager Manager)
        {
            while(true)
            {
                if(OnOffScan)
                {
                    Value = Manager.SimulatorManager.GetAnalogValue(Address);
                }
                Thread.Sleep((int)(ScanTime * 1000));
            }
        }
    }
}
