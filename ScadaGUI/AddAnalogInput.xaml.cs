using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataConcentrator;
using DataConcentrator.src;
using System;
using System.Data.Entity.Validation;

namespace ScadaGUI
{
    public partial class AddAnalogInput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public DataGrid AnalogInputsList;
        public AnalogInput analogInput = new AnalogInput()
        {
            Alarms = new List<Alarm>()
        };
        public AddAnalogInput(PLCManager manager, DataGrid analogInputsList)
        {
            InitializeComponent();
            Manager = manager;
            AnalogInputsList = analogInputsList;
            Address.ItemsSource = Manager.AvailibleAnalogInputs.Keys;
            OnOffScan.ItemsSource = new List<string> { "Uključi", "Isključi" };
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if(ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Context.AnalogInputs.Add(analogInput);
            Context.Alarms.AddRange(analogInput.Alarms);
            try
            {
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
            }

            MessageBox.Show("Uspješno dodat analogni ulaz!", "Dodaj analogni ulaz", MessageBoxButton.OK, MessageBoxImage.Information);
            AnalogInputsList.ItemsSource = Context.AnalogInputs.ToList();
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
                MessageBox.Show($"Nemoguće dodati alarm! Razlog - {errorMessage}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            AddAlarmOnNewAI addAlarmOnNewAI = new AddAlarmOnNewAI(TagId, TagName, LowLimitValue, HighLimitValue, analogInput.Alarms)
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
            analogInput.Description = Description.Text;

            // Is the TAG written
            if (string.IsNullOrEmpty(Tag.Text)) 
            {
                errorMessage += "Niste upisali tag!";
                return true;
            }
            analogInput.Id = Tag.Text;

            // Is the Name given
            if (string.IsNullOrEmpty(Name.Text))
            {
                errorMessage += "Niste upisali ime!";
                return true;
            }
            analogInput.Name = Name.Text;

            // Is the I/O ADDRESS selected
            if (Address.SelectedItem == null)
            {
                errorMessage += "Niste selektovali adresu!";
                return true;
            }
            analogInput.Address = Address.Text;

            // Is the Scan Time valid
            if(!double.TryParse(ScanTime.Text, out double ScanTimeValue)) 
            {
                errorMessage += "Nevalidno vrijeme skeniranja!";
                return true;
            }
            else if(ScanTimeValue <= 0)
            {
                errorMessage += "Vrijeme skeniranja ne može biti manje od nule!";
                return true;
            }
            analogInput.ScanTime = ScanTimeValue;

            // Is the scanning selected
            if(OnOffScan.SelectedItem == null)
            {
                errorMessage += "Niste selektovali da li želite uključeno ili isključeno skeniranje";
                return true;
            }
            analogInput.OnOffScan = OnOffScan.Text == "Uključi";

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
            analogInput.HighLimit = HighLimitValue;
            analogInput.LowLimit = LowLimitValue;

            // Is the unit written
            if(string.IsNullOrEmpty(Unit.Text) || double.TryParse(Unit.Text, out _))
            {
                errorMessage += "Nepravilna jedinica!";
                return true;
            }
            analogInput.Units = Unit.Text;

            return false;
        }
    }
}
