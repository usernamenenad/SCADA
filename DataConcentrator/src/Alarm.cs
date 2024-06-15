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
    public enum AlarmActivationEdge
    {
        Rising,
        Falling
    }
    public class Alarm
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public double ActivationValue { get; set; }

        [Required]
        [EnumDataType(typeof(AlarmActivationEdge))]
        public AlarmActivationEdge ActivationEdge {  get; set; }

        [Required]
        [EnumDataType(typeof(AlarmPriority))]
        public AlarmPriority Priority { get; set; }

        [Required]
        public string AnalogInputId { get; set; }

        [Required]
        public string AnalogInputName { get; set; } 

        public static event Action<Alarm> OnAlarmTriggered;

        public void TriggerAlarm()
        {
            OnAlarmTriggered?.Invoke(this);
        }
    }
}
