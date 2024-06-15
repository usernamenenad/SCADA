using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.src
{
    public class Input : Tag
    {
        [Required]
        public double ScanTime { get; set; }

        [Required]
        public bool OnOffScan { get; set; }
    }
}
