using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataConcentrator;
using DataConcentrator.src;
using System;
using System.Data.Entity.Validation;
using System.Threading;

namespace ScadaGUI
{
    public partial class AddAnalogInput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public DataGrid AnalogInputsList;
        public DataGrid AlarmList;
        public List<Alarm> AlarmsToRemove {  get; set; } = new List<Alarm>();
        public AnalogInput AnalogInput = new AnalogInput()
        {
            Alarms = new List<Alarm>()
        };
        public AddAnalogInput(PLCManager manager, DataGrid analogInputsList, DataGrid alarmList)
        {
            InitializeComponent();

            Manager = manager;
            Address.ItemsSource = Manager.AvailibleAnalogInputs;
            OnOffScan.ItemsSource = new List<string> { "Uključi", "Isključi" };

            AnalogInputsList = analogInputsList;
            AlarmList = alarmList;
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            if(ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Context.AnalogInputs.Add(AnalogInput);

                if (Owner is MainWindow mainWindow)
                {
                    AnalogInput.Scanner = new Thread(() => AnalogInput.Scan(Manager));
                    AnalogInput.Scanner.Start();
                }

                Context.SaveChanges();
            }
            catch (DbEntityValidationException ex) 
            {
                foreach(var eve in ex.EntityValidationErrors)
                {
                    foreach(var ve in eve.ValidationErrors)
                    {
                        MessageBox.Show($"{ve.PropertyName}, {ve.ErrorMessage}");
                    }
                }
                return;
            }

            AnalogInputsList.ItemsSource = Context.AnalogInputs.ToList();
            AlarmList.ItemsSource = Context.Alarms.ToList();
            Manager.TakeAnalogInput(AnalogInput.Address);
            MessageBox.Show("Uspješno uređen analogni ulaz!", "Dodaj analogni ulaz", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Cancel(object sender, RoutedEventArgs e) 
        {
            var result = MessageBox.Show("Da li ste sigurni da želite otkazati dodavanje?", "Greška", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        private void AddAnalogInputAlarm(object sender, RoutedEventArgs e)
        {
            if(CheckIfCanAddAlarm(out string errorMessage, out string TagId, out string TagName, out double LowLimitValue, out double HighLimitValue))
            {
                MessageBox.Show($"Nemoguće urediti alarme! Razlog - {errorMessage}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            AddAlarmOnNewAI addAlarmOnNewAI = new AddAlarmOnNewAI(TagId, TagName, LowLimitValue, HighLimitValue, AnalogInput.Alarms, AlarmsToRemove)
            {
                Owner = this
            };
            addAlarmOnNewAI.Show();
        }
        private bool CheckIfCanAddAlarm(out string errorMessage, out string TagId, out string TagName, out double LowLimitValue, out double HighLimitValue)
        {
            errorMessage = string.Empty;
            LowLimitValue = double.MinValue;
            HighLimitValue = double.MaxValue;
            TagId = string.Empty;
            TagName = string.Empty;

            // Is the TAG given
            if (string.IsNullOrEmpty(Tag.Text))
            {
                errorMessage += "Niste upisali tag!";
                return true;
            }

            // Is the Name given
            if (string.IsNullOrEmpty(Name.Text))
            {
                errorMessage += "Niste upisali ime!";
                return true;
            }

            // Are limits selected and valid
            if (!double.TryParse(HighLimit.Text, out double highLimitValue))
            {
                errorMessage += "Nevalidna gornja granica!";
                return true;
            }
            if (!double.TryParse(LowLimit.Text, out double lowLimitValue))
            {
                errorMessage += "Nevalidna donja granica!";
                return true;
            }
            if (HighLimitValue <= LowLimitValue)
            {
                errorMessage += "Nevalidne granice!";
                return true;
            }

            TagId = Tag.Text;
            TagName = Name.Text;
            LowLimitValue = lowLimitValue;
            HighLimitValue = highLimitValue;
            return false;
        }
        private bool ValidateInputs(out string errorMessage)
        {
            errorMessage = string.Empty;
            AnalogInput.Description = Description.Text;

            // Is the TAG given
            if (string.IsNullOrEmpty(Tag.Text)) 
            {
                errorMessage += "Niste upisali tag!";
                return true;
            }
            AnalogInput.Id = Tag.Text;

            // Is that tag already in the table
            if (Context.AnalogInputs.Any(analogInputs => analogInputs.Id == Tag.Text))
            {
                errorMessage += "Već postoji takav tag!";
                return true;
            }

            // Is the Name given
            if (string.IsNullOrEmpty(Name.Text))
            {
                errorMessage += "Niste upisali ime!";
                return true;
            }
            AnalogInput.Name = Name.Text;

            // Is the I/O ADDRESS selected
            if (Address.SelectedItem == null)
            {
                errorMessage += "Niste selektovali adresu!";
                return true;
            }
            AnalogInput.Address = Address.Text;

            // Is the scanning selected
            if (OnOffScan.SelectedItem == null)
            {
                errorMessage += "Niste selektovali da li želite uključeno ili isključeno skeniranje";
                return true;
            }
            AnalogInput.OnOffScan = OnOffScan.Text == "Uključi";

            // Is the Scan Time valid
            if (!double.TryParse(ScanTime.Text, out double ScanTimeValue)) 
            {
                errorMessage += "Nevalidno vrijeme skeniranja!";
                return true;
            }
            else if(ScanTimeValue <= 0)
            {
                errorMessage += "Vrijeme skeniranja ne može biti manje od nule!";
                return true;
            }
            AnalogInput.ScanTime = ScanTimeValue;

            // Are limits selected and valid
            if(!double.TryParse(HighLimit.Text, out double HighLimitValue))
            {
                errorMessage += "Nevalidna gornja granica!";
                return true;
            }
            if (!double.TryParse(LowLimit.Text, out double LowLimitValue))
            {
                errorMessage += "Nevalidna donja granica!";
                return true;
            }
            if(HighLimitValue <= LowLimitValue)
            {
                errorMessage += "Nevalidne granice!";
                return true;
            }
            AnalogInput.HighLimit = HighLimitValue;
            AnalogInput.LowLimit = LowLimitValue;

            // Is the unit given
            if(string.IsNullOrEmpty(Unit.Text) || double.TryParse(Unit.Text, out _))
            {
                errorMessage += "Nepravilna jedinica!";
                return true;
            }
            AnalogInput.Units = Unit.Text;

            return false;
        }
    }
}
