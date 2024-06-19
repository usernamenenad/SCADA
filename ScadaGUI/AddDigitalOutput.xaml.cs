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
    public partial class AddDigitalOutput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public DataGrid DigitalOutputsList;
        public DigitalOutput DigitalOutput = new DigitalOutput();

        public AddDigitalOutput(PLCManager manager, DataGrid digitalOutputsList)
        {
            InitializeComponent();

            Manager = manager;
            DigitalOutputsList = digitalOutputsList;
            Address.ItemsSource = Manager.AvailibleDigitalOutputs;
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
                Context.DigitalOutputs.Add(DigitalOutput);
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

            DigitalOutputsList.ItemsSource = Context.DigitalOutputs.ToList();
            Manager.TakeDigitalOutput(DigitalOutput.Address);

            var mainWindow = Owner as MainWindow;
            mainWindow.NumberOfDigitalOutputs.Text = Context.DigitalOutputs.Count().ToString();

            MessageBox.Show("Uspješno dodat digitalni izlaz!", "Dodaj analogni izlaz", MessageBoxButton.OK, MessageBoxImage.Information);

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
            DigitalOutput.Description = Description.Text;

            // Is the TAG Given
            if (string.IsNullOrEmpty(Tag.Text))
            {
                errorMessage += "Niste upisali tag!";
                return true;
            }
            DigitalOutput.Id = Tag.Text;

            // Is that tag already in the table
            if (Context.DigitalOutputs.Any(digitalOutput => digitalOutput.Id == Tag.Text))
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
            DigitalOutput.Name = Name.Text;

            // Is the I/O ADDRESS selected
            if (Address.SelectedItem == null)
            {
                errorMessage += "Niste selektovali adresu!";
                return true;
            }
            DigitalOutput.Address = Address.Text;

            // Is the Initial Value valid
            if (string.IsNullOrEmpty(InitialValue.Text) || !int.TryParse(InitialValue.Text, out int InitialValueValue))
            {
                errorMessage += "Nevalidna početna vrijednost!";
                return true;
            }
            if (!new List<int>() { 0, 1 }.Contains(InitialValueValue))
            {
                errorMessage += "Nevalidna početna vrijednost! Mora biti 0 ili 1!";
                return true;
            }
            DigitalOutput.InitialValue = InitialValueValue;
            DigitalOutput.Value = InitialValueValue;

            return false;
        }
    }
}
