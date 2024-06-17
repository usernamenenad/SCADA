using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator.src
{
    public class DigitalOutput : Output
    {
        [NotMapped]
        public double Value { get; set; }

        public DigitalOutput() 
        {
            Value = InitialValue;
        }
    }
}
