using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.src
{
    public class AnalogInput : Input
    {
        public double Value { get; set; }

        public double LowLimit { get; set; }

        public double HighLimit { get; set; }

        public string Units { get; set; }

        public List<Alarm> Alarms { get; set; } = new List<Alarm>();
    }
}
