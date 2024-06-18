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

        public event PropertyChangedEventHandler PropertyChanged;

        public Thread Scanner;

        public void Scan(PLCManager Manager)
        {
            double previousValue = Value;
            while(true)
            {
                previousValue = Value;
                if (OnOffScan)
                {
                    Value = Manager.SimulatorManager.GetAnalogValue(Address);
                }
                foreach (Alarm alarm in Alarms)
                {
                    if(alarm.IsActive)
                    {
                        continue;
                    }
                    if(alarm.ActivationEdge == AlarmActivationEdge.Rising)
                    {
                        if(Value > alarm.ActivationValue && Value > previousValue && !alarm.IsAcknowledged)
                        {
                            alarm.IsActive = true;
                        }
                        if(Value < alarm.ActivationValue)
                        {
                            alarm.IsAcknowledged = false;
                        }
                    }
                    else
                    {
                        if(Value < alarm.ActivationValue && Value < previousValue && !alarm.IsAcknowledged)
                        {
                            alarm.IsActive = true;
                        }
                        if (Value > alarm.ActivationValue)
                        {
                            alarm.IsAcknowledged = false;
                        }
                    }
                }
                Thread.Sleep((int)(ScanTime * 1000));
            }
        }
    }
}
