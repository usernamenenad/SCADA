using System;
using System.ComponentModel;
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
    public class Alarm : INotifyPropertyChanged
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isactive;

        [NotMapped]
        public bool IsActive
        {
            get => _isactive;
            set
            {
                _isactive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive))); 
            }
        }

        [NotMapped]
        public bool IsAcknowledged { get; set; }

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
    }
}
