using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.src
{
    public class AnalogInput : Input
    {
        [Required]
        public double Value { get; set; }

        [Required]
        public double LowLimit { get; set; }

        [Required]
        public double HighLimit { get; set; }

        [Required]
        public string Units { get; set; }

        [Required]
        public List<Alarm> Alarms { get; set; } = new List<Alarm>();
    }
}
