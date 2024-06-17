using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace DataConcentrator.src
{
    public class DigitalInput : Input, INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public Thread Scanner;

        public void Scan(PLCManager Manager)
        {
            while (true)
            {
                if (OnOffScan)
                {
                    Value = Manager.SimulatorManager.GetDigitalValue(Address);
                }
                Thread.Sleep((int)(ScanTime * 1000));
            }
        }
    }
}
