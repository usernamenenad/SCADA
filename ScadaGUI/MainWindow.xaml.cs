using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using DataConcentrator;
using DataConcentrator.src;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.Entity.Validation;

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

            Manager.SetOutputs();
            Manager.SimulatorManager.StartPLCSimulator();

            SubsrcibeToEvents();

            StartScanning();
        }
        void SubsrcibeToEvents()
        {
            foreach (var analogInput in Context.AnalogInputs)
            {
                foreach(var alarm in analogInput.Alarms)
                {
                    alarm.PropertyChanged += UpdateAlarms;
                }
            }
        }
        private void AddAnalogInput(object sender, RoutedEventArgs e)
        {
            AddAnalogInput addAnalogInput = new AddAnalogInput(Manager, AnalogInputsList, AlarmList)
            {
                Owner = this
            };
            addAnalogInput.Show();
        }
        private void AddAnalogOutput(object sender, RoutedEventArgs e)
        {
            AddAnalogOutput addAnalogOutput = new AddAnalogOutput(Manager, AnalogOutputsList)
            {
                Owner = this
            };
            addAnalogOutput.Show();
        }
        private void AddDigitalInput(object sender, RoutedEventArgs e)
        {
            AddDigitalInput addDigitalInput = new AddDigitalInput(Manager, DigitalInputsList)
            {
                Owner = this
            };
            addDigitalInput.Show();
        }
        private void AddDigitalOutput(object sender, RoutedEventArgs e)
        {
            AddDigitalOutput addDigitalOutput = new AddDigitalOutput(Manager, DigitalOutputsList)
            {
                Owner = this
            };
            addDigitalOutput.Show();
        }

        // Double click on grid row event handlers
        private void EditAnalogInput(object sender, MouseButtonEventArgs e)
        {
            AnalogInput analogInput = (sender as DataGridRow)?.DataContext as AnalogInput;
            EditAnalogInput editAnalogInput = new EditAnalogInput(analogInput, Manager, AnalogInputsList, AlarmList)
            {
                Owner = this
            };
            editAnalogInput.Show();
        }
        private void EditAnalogOutput(object sender, MouseButtonEventArgs e)
        {
            AnalogOutput analogOutput = (sender as DataGridRow)?.DataContext as AnalogOutput;
            EditAnalogOutput editAnalogOutput = new EditAnalogOutput(analogOutput, Manager, AnalogOutputsList)
            {
                Owner = this
            };
            editAnalogOutput.Show();
        }
        private void EditDigitalInput(object sender, MouseButtonEventArgs e)
        {
            DigitalInput digitalInput = (sender as DataGridRow)?.DataContext as DigitalInput;
            EditDigitalInput editDigitalInput = new EditDigitalInput(digitalInput, Manager, DigitalInputsList)
            {
                Owner = this
            };
            editDigitalInput.Show();
        }
        private void EditDigitalOutput(object sender, MouseButtonEventArgs e)
        {
            DigitalOutput digitalOutput = (sender as DataGridRow)?.DataContext as DigitalOutput;
            EditDigitalOutput editDigitalOutput = new EditDigitalOutput(digitalOutput, Manager, DigitalOutputsList)
            {
                Owner = this
            };
            editDigitalOutput.Show();
        }
        public void StartScanning()
        {
            foreach(var analogInput in Context.AnalogInputs)
            {
                analogInput.PropertyChanged += UpdateAnalogDataGrid;
                analogInput.Scanner = new Thread(() => analogInput.Scan(Manager));
                analogInput.Scanner.Start();
            }
            foreach (var digitalInput in Context.DigitalInputs)
            {
                digitalInput.PropertyChanged += UpdateDigitalDataGrid;
                digitalInput.Scanner = new Thread(() => digitalInput.Scan(Manager));
                digitalInput.Scanner.Start();
            }
        }
        public void UpdateAnalogDataGrid(object sender, PropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AnalogInputsList.ItemsSource = Context.AnalogInputs.ToList();
            });
        }
        public void UpdateDigitalDataGrid(object sender, PropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DigitalInputsList.ItemsSource = Context.DigitalInputs.ToList();
            });
        }
        public void UpdateAlarms(object sender, PropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var TriggeredAlarm = sender as Alarm;
                var row = FindAlarmOnDataGrid(TriggeredAlarm);
                if(row is null)
                {
                    return;
                }

                if (!TriggeredAlarm.IsActive)
                {
                    MessageBox.Show("Alarm prihvaćen!", "Alarm", MessageBoxButton.OK, MessageBoxImage.Information);

                    row.Background = Brushes.White;
                    AlarmList.SelectedIndex = -1;

                    return;
                }

                try
                {
                    AlarmHistorySample alarmHistorySample = new AlarmHistorySample()
                    {
                        AlarmId = TriggeredAlarm.Id,
                        VarName = TriggeredAlarm.AnalogInputName,
                        Message = TriggeredAlarm.Description,
                        TimeStamp = DateTime.Now
                    };
                    alarmHistorySample.Id = $"{alarmHistorySample.Id}_{alarmHistorySample.TimeStamp}";

                    Context.AlarmHistory.Add(alarmHistorySample);
                    Context.SaveChanges();

                    row.Background = Brushes.Red;
                    AlarmList.SelectedIndex = -1;
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            MessageBox.Show($"{ve.PropertyName}, {ve.ErrorMessage}");
                        }
                    }
                    return;
                }
            });
        }
        private void AcknowledgeAlarm(object sender, MouseButtonEventArgs e)
        {
            Alarm alarm = (sender as DataGridRow)?.DataContext as Alarm;
            alarm.IsAcknowledged = true;
            alarm.IsActive = false;
        }
        private DataGridRow FindAlarmOnDataGrid(Alarm alarm)
        {
            foreach (var dataGridItem in AlarmList.Items)
            {
                if (dataGridItem == alarm)
                {
                    return (DataGridRow)AlarmList.ItemContainerGenerator.ContainerFromIndex(AlarmList.Items.IndexOf(dataGridItem));
                }
            }
            return null;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            foreach (var analogInput in Context.AnalogInputs)
            {
                analogInput.Scanner.Abort();
                analogInput.PropertyChanged -= UpdateAnalogDataGrid;

                foreach(var alarm in analogInput.Alarms)
                {
                    alarm.PropertyChanged -= UpdateAlarms;
                }
            }
            foreach (var digitalInput in Context.DigitalInputs)
            {
                digitalInput.Scanner.Abort();
                digitalInput.PropertyChanged -= UpdateDigitalDataGrid;
            }
            Manager.SimulatorManager.Abort();
        }
    }
}
