using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.src
{
    public class Output : Tag
    {
        [Required]
        public double InitialValue { get; set; }
    }
}
