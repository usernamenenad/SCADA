using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.src
{
    public class AlarmHistorySample
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string AlarmId { get; set; }

        [Required]
        public string VarName { get; set; }

        public string Message { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }
    }
}
