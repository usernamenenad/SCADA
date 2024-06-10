using System.Data.Entity;

namespace DataConcentrator.src
{
    public class DataConcentratorContext : DbContext
    {
        public DbSet<AnalogInput> AnalogInputs { get; set; }
        public DbSet<AnalogOutput> AnalogOutputs { get; set; }
        public DbSet<DigitalInput> DigitalInputs { get; set; }
        public DbSet<DigitalOutput> DigitalOutputs { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<AlarmHistorySample> AlarmHistory {  get; set; }

        // Singleton principle
        private static DataConcentratorContext instance;
        public static DataConcentratorContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataConcentratorContext();
                }
                return instance;
            }
        }
    }
}
