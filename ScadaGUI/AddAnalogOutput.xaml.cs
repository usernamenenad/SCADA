using DataConcentrator;
using DataConcentrator.src;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ScadaGUI
{
    public partial class AddAnalogOutput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public DataGrid AnalogOutputsList;
        public AnalogOutput AnalogOutput = new AnalogOutput();

        public AddAnalogOutput(PLCManager manager, DataGrid analogOutputsList)
        {
            InitializeComponent();
            Manager = manager;
            AnalogOutputsList = analogOutputsList;
            Address.ItemsSource = Manager.AvailibleAnalogOutputs;
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
                Context.AnalogOutputs.Add(AnalogOutput);
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

            AnalogOutputsList.ItemsSource = Context.AnalogOutputs.ToList();
            Manager.TakeAnalogOutput(AnalogOutput.Address);

            MessageBox.Show("Uspješno dodat analogni izlaz!", "Dodaj analogni izlaz", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }
        private void Cancel(object sender, RoutedEventArgs e)
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
            AnalogOutput.Description = Description.Text;

            // Is the TAG given
            if (string.IsNullOrEmpty(Tag.Text))
            {
                errorMessage += "Niste upisali tag!";
                return true;
            }
            AnalogOutput.Id = Tag.Text;

            // Is that tag already in the table
            if (Context.AnalogOutputs.Any(analogOutputs => analogOutputs.Id == Tag.Text))
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
            AnalogOutput.Name = Name.Text;

            // Is the I/O ADDRESS selected
            if (Address.SelectedItem == null)
            {
                errorMessage += "Niste selektovali adresu!";
                return true;
            }
            AnalogOutput.Address = Address.Text;

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
            AnalogOutput.HighLimit = HighLimitValue;
            AnalogOutput.LowLimit = LowLimitValue;

            // Is the Initial Value valid
            if (!double.TryParse(InitialValue.Text, out double InitialValueValue))
            {
                if(InitialValueValue > HighLimitValue || InitialValueValue < LowLimitValue)
                {
                    errorMessage += "Nevalidna početna vrijednost!";
                    return true;
                }
            }
            AnalogOutput.InitialValue = InitialValueValue;
            AnalogOutput.Value = InitialValueValue;

            // Is the unit given
            if (string.IsNullOrEmpty(Units.Text) || double.TryParse(Units.Text, out _))
            {
                errorMessage += "Nepravilna jedinica!";
                return true;
            }
            AnalogOutput.Units = Units.Text;

            return false;
        }
    }
}
