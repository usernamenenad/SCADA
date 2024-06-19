using DataConcentrator;
using DataConcentrator.src;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScadaGUI
{
    public partial class EditAnalogInput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public AnalogInput AnalogInput;
        public DataGrid AnalogInputsList;
        public DataGrid AlarmList;
        public List<Alarm> AlarmsToRemove {  get; set; } = new List<Alarm>();
        public EditAnalogInput(AnalogInput analogInput, PLCManager manager, DataGrid analogInputsList, DataGrid alarmList)
        {
            InitializeComponent();

            AnalogInput = analogInput;
            Manager = manager;
            AnalogInputsList = analogInputsList;
            AlarmList = alarmList;

            Tag.Text = analogInput.Id;
            Name.Text = analogInput.Name;
            Description.Text = analogInput.Description;

            List<string> AddressDisplay = new List<string>(Manager.AvailibleAnalogInputs)
            {
                analogInput.Address
            };
            AddressDisplay.Sort();

            Address.ItemsSource = AddressDisplay;
            Address.SelectedItem = analogInput.Address;

            ScanTime.Text = analogInput.ScanTime.ToString();

            OnOffScan.ItemsSource = new List<string> { "Uključi", "Isključi" };
            OnOffScan.SelectedItem = analogInput.OnOffScan ? "Uključi" : "Isključi";

            LowLimit.Text = analogInput.LowLimit.ToString();
            HighLimit.Text = analogInput.HighLimit.ToString();
            Units.Text = analogInput.Units;
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AnalogInput.Name = Name.Text;
            AnalogInput.Description = Description.Text;

            string OldAddress = AnalogInput.Address;
            AnalogInput.Address = Address.Text;

            AnalogInput.OnOffScan = OnOffScan.Text == "Uključi";
            AnalogInput.ScanTime = double.Parse(ScanTime.Text);

            AnalogInput.HighLimit = double.Parse(HighLimit.Text);
            AnalogInput.LowLimit = double.Parse(LowLimit.Text);

            AnalogInput.Units = Units.Text;

            try
            {
                foreach (var alarm in AlarmsToRemove)
                {
                    Context.Alarms.Remove(alarm);
                }
                Context.SaveChanges();
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

            AnalogInputsList.ItemsSource = Context.AnalogInputs.ToList();
            AlarmList.ItemsSource = Context.Alarms.ToList();

            var mainWindow = Owner as MainWindow;
            mainWindow.NumberOfAnalogInputs.Text = Context.AnalogInputs.Count().ToString();

            if (OldAddress != Address.Text)
            {
                Manager.FreeAnalogOutput(OldAddress);
                Manager.TakeAnalogOutput(AnalogInput.Address);
            }

            MessageBox.Show("Uspješno uređen analogni ulaz!", "Uredi analogni ulaz", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }
        private void Delete(object sender, RoutedEventArgs e) 
        {
            var result = MessageBox.Show("Da li ste sigurni da želite obrisati analogni ulaz?", "Upozorenje", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var alarm in AlarmsToRemove)
                    {
                        Context.Alarms.Remove(alarm);
                    }

                    AnalogInput.Scanner.Abort();
                    Context.AnalogInputs.Remove(AnalogInput);
                    Context.SaveChanges();

                    Manager.FreeAnalogInput(AnalogInput.Address);
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

                AnalogInputsList.ItemsSource = Context.AnalogInputs.ToList();
                AlarmList.ItemsSource = Context.Alarms.ToList();

                var mainWindow = Owner as MainWindow;
                mainWindow.NumberOfAnalogInputs.Text = Context.AnalogInputs.Count().ToString();

                Close();
            }
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
            if (CheckIfCanAddAlarm(out string errorMessage, out string TagId, out string TagName, out double LowLimitValue, out double HighLimitValue))
            {
                MessageBox.Show($"Nemoguće dodati alarm! Razlog - {errorMessage}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
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

            // Is the TAG written
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

            // Is the I/O ADDRESS selected
            if (Address.SelectedItem == null)
            {
                errorMessage += "Niste selektovali adresu!";
                return true;
            }

            // Is the Scan Time valid
            if (!double.TryParse(ScanTime.Text, out double ScanTimeValue))
            {
                errorMessage += "Nevalidno vrijeme skeniranja!";
                return true;
            }
            else if (ScanTimeValue <= 0)
            {
                errorMessage += "Vrijeme skeniranja ne može biti manje od nule!";
                return true;
            }

            // Is the scanning selected
            if (OnOffScan.SelectedItem == null)
            {
                errorMessage += "Niste selektovali da li želite uključeno ili isključeno skeniranje";
                return true;
            }

            // Are limits selected and valid
            if (!double.TryParse(HighLimit.Text, out double HighLimitValue))
            {
                errorMessage += "Nevalidna gornja granica!";
                return true;
            }
            if (!double.TryParse(LowLimit.Text, out double LowLimitValue))
            {
                errorMessage += "Nevalidna donja granica!";
                return true;
            }
            if (HighLimitValue <= LowLimitValue)
            {
                errorMessage += "Nevalidne granice!";
                return true;
            }

            // Is the unit written
            if (string.IsNullOrEmpty(Units.Text) || double.TryParse(Units.Text, out _))
            {
                errorMessage += "Nepravilna jedinica!";
                return true;
            }

            return false;
        }
    }
}
