using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.src
{
    public class AlarmHistorySample
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AlarmId")]
        public int AlarmId { get; set; }

        public string VarName { get; set; }

        public string Message { get; set; }

        public DateTime TimeStamp { get; set; }

        public AlarmHistorySample(int alarmId, string varName, string message, DateTime timeStamp) 
        {
            AlarmId = alarmId;
            VarName = varName;
            Message = message;
            TimeStamp = timeStamp;
        }
    }
}
