using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator.src
{
    public class AnalogOutput : Output
    {
        [Required]
        public double Value { get; set; }

        [Required]
        public double LowLimit { get; set; }

        [Required]
        public double HighLimit { get; set; }

        [Required]
        public string Units { get; set; }

        public AnalogOutput() 
        {
            Value = InitialValue;
        }
    }
}
