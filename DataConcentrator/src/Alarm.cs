using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.src
{
    public enum AlarmPriority
    {
        Lowest,
        Low,
        Medium, 
        High,
        Highest
    }
    public class Alarm
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        [EnumDataType(typeof(AlarmPriority))]
        public AlarmPriority Priority { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }

        public string TagName { get; set; }

        public static event Action<Alarm> OnAlarmTriggered;

        public Alarm()
        {
            IsActive = false;
        }

        public void TriggerAlarm()
        {
            OnAlarmTriggered?.Invoke(this);
        }
    }
}
