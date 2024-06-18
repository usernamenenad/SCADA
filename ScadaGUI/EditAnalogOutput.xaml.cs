using DataConcentrator.src;
using DataConcentrator;
using System;
using System.Collections.Generic;
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
using System.Data.Entity.Validation;

namespace ScadaGUI
{
    public partial class EditAnalogOutput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public AnalogOutput AnalogOutput;
        public DataGrid AnalogOutputsList;
        public EditAnalogOutput(AnalogOutput analogOutput, PLCManager manager, DataGrid analogOutputsList)
        {
            InitializeComponent();

            Manager = manager;
            AnalogOutput = analogOutput;
            AnalogOutputsList = analogOutputsList;

            Tag.Text = analogOutput.Id;
            Name.Text = analogOutput.Name;
            Description.Text = analogOutput.Description;

            List<string> AddressDisplay = new List<string>(Manager.AvailibleAnalogOutputs) 
            {
                analogOutput.Address
            };
            AddressDisplay.Sort();

            Address.ItemsSource = AddressDisplay;
            Address.SelectedItem = analogOutput.Address;

            LowLimit.Text = analogOutput.LowLimit.ToString();
            HighLimit.Text = analogOutput.HighLimit.ToString();
            CurrentValue.Text = analogOutput.Value.ToString();

            Units.Text = analogOutput.Units.ToString();
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            if(ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AnalogOutput.Name = Name.Text;
            AnalogOutput.Description = Description.Text;

            string OldAddress = AnalogOutput.Address;
            AnalogOutput.Address = Address.Text;

            AnalogOutput.LowLimit = double.Parse(LowLimit.Text);
            AnalogOutput.HighLimit = double.Parse(HighLimit.Text);
            AnalogOutput.Value = double.Parse(CurrentValue.Text);

            AnalogOutput.Units = Units.Text;

            try
            {
                Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        MessageBox.Show($"{ve.PropertyName}, {ve.ErrorMessage}", "Greška");
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Greška");
                return;
            }

            AnalogOutputsList.ItemsSource = Context.AnalogOutputs.ToList();

            if(OldAddress != Address.Text)
            {
                Manager.FreeAnalogOutput(OldAddress);
                Manager.TakeAnalogOutput(AnalogOutput.Address);
            }

            MessageBox.Show("Uspješno uređen analogni izlaz!", "Uredi analogni izlaz", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }
        public void Delete(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da želite obrisati analogni izlaz?", "Upozorenje", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Context.AnalogOutputs.Remove(AnalogOutput);
                    Context.SaveChanges();

                    Manager.FreeAnalogOutput(AnalogOutput.Address);
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
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AnalogOutputsList.ItemsSource = Context.AnalogOutputs.ToList();
                
                Close();
            }
        }
        public void Cancel(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da želite otkazati dodavanje?", "Greška", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        private bool ValidateInputs(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Is the TAG given
            if (string.IsNullOrEmpty(Tag.Text))
            {
                errorMessage += "Niste upisali tag!";
                return true;
            }

            // Is that tag already in the table
            if (Context.AnalogOutputs.Any(analogOutput => analogOutput.Id == Tag.Text))
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

            // Is the I/O ADDRESS selected
            if (Address.SelectedItem == null)
            {
                errorMessage += "Niste selektovali adresu!";
                return true;
            }

            // Check if high and low limits are valid
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

            // Is the Current Value valid
            if (string.IsNullOrEmpty(CurrentValue.Text) || !double.TryParse(CurrentValue.Text, out double CurrentValueValue))
            {
                errorMessage += "Nevalidna trenutna vrijednost!";
                return true;
            }
            if (CurrentValueValue > HighLimitValue || CurrentValueValue < LowLimitValue)
            {
                errorMessage += "Nevalidna trenutna vrijednost!";
                return true;
            }

            // Is the unit given
            if (string.IsNullOrEmpty(Units.Text) || double.TryParse(Units.Text, out _))
            {
                errorMessage += "Nepravilna jedinica!";
                return true;
            }

            return false;
        }
    }
}
