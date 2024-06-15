using System.Linq;
using System.Windows;
using DataConcentrator;
using DataConcentrator.src;

namespace ScadaGUI
{
    public partial class MainWindow : Window
    {
        public DataConcentratorContext Context { get; set; } = DataConcentratorContext.Instance;
        public PLCManager Manager { get; set; } = new PLCManager();
        public int NumberOfAnalogInputs { get; set; }
        public int NumberOfAnalogOutputs { get; set; }
        public int NumberOfDigitalInputs { get; set; }
        public int NumberOfDigitalOutputs { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            NumberOfAnalogInputs = Context.AnalogInputs.Count();
            NumberOfAnalogOutputs = Context.AnalogOutputs.Count();
            NumberOfDigitalInputs = Context.DigitalInputs.Count();
            NumberOfDigitalOutputs = Context.DigitalOutputs.Count();

            AnalogInputsList.ItemsSource = Context.AnalogInputs.ToList();
            AnalogOutputsList.ItemsSource = Context.AnalogOutputs.ToList();
            DigitalInputsList.ItemsSource = Context.DigitalInputs.ToList();
            DigitalOutputsList.ItemsSource = Context.DigitalOutputs.ToList();

            AlarmList.ItemsSource = Context.Alarms.ToList();    
        }
        private void AddAnalogInput(object sender, RoutedEventArgs e)
        {
            AddAnalogInput addAnalogInput = new AddAnalogInput(Manager, AnalogInputsList)
            {
                Owner = this
            };
            addAnalogInput.Show();
        }
        private void AddAnalogOutput(object sender, RoutedEventArgs e)
        {
            AddAnalogOutput addAnalogOutput = new AddAnalogOutput()
            {
                Owner = this
            };
            addAnalogOutput.Show();
        }
        private void AddDigitalInput(object sender, RoutedEventArgs e)
        {
            AddDigitalInput addDigitalInput = new AddDigitalInput()
            {
                Owner = this
            };
            addDigitalInput.Show();
        }
        private void AddDigitalOutput(object sender, RoutedEventArgs e)
        {
            AddDigitalOutput addDigitalOutput = new AddDigitalOutput()
            {
                Owner = this
            };
            addDigitalOutput.Show();
        }
        private void AddAlarm(object sender, RoutedEventArgs e)
        {
            AddAlarm addAlarm = new AddAlarm()
            {
                Owner = this
            };
            addAlarm.Show();
        }
    }
}
