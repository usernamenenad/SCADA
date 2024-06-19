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
    public partial class EditDigitalOutput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public DigitalOutput DigitalOutput;
        public DataGrid DigitalOutputsList;
        public EditDigitalOutput(DigitalOutput digitalOutput, PLCManager manager, DataGrid digitalOutputsList)
        {
            InitializeComponent();

            Manager = manager;
            DigitalOutput = digitalOutput;
            DigitalOutputsList = digitalOutputsList;

            Tag.Text = digitalOutput.Id;
            Name.Text = digitalOutput.Name;
            Description.Text = digitalOutput.Description;

            List<string> AddressDisplay = new List<string>(Manager.AvailibleDigitalOutputs)
            {
                digitalOutput.Address
            };
            AddressDisplay.Sort();

            Address.ItemsSource = AddressDisplay;
            Address.SelectedItem = digitalOutput.Address;

            CurrentValue.Text = digitalOutput.Value.ToString();
        }
        private void Save(object sender, RoutedEventArgs e) 
        {
            if (ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DigitalOutput.Name = Name.Text;
            DigitalOutput.Description = Description.Text;

            string OldAddress = DigitalOutput.Address;
            DigitalOutput.Address = Address.Text;

            DigitalOutput.Value = int.Parse(CurrentValue.Text);

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

            DigitalOutputsList.ItemsSource = Context.DigitalOutputs.ToList();

            var mainWindow = Owner as MainWindow;
            mainWindow.NumberOfDigitalOutputs.Text = Context.DigitalOutputs.Count().ToString();

            if (OldAddress != Address.Text)
            {
                Manager.FreeDigitalOutput(OldAddress);
                Manager.TakeDigitalOutput(DigitalOutput.Address);
            }

            MessageBox.Show("Uspješno uređen digitalni izlaz!", "Uredi digitalni izlaz", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da želite obrisati digitalni izlaz?", "Upozorenje", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Context.DigitalOutputs.Remove(DigitalOutput);
                    Context.SaveChanges();

                    Manager.FreeDigitalOutput(DigitalOutput.Address);
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

                DigitalOutputsList.ItemsSource = Context.DigitalOutputs.ToList();

                var mainWindow = Owner as MainWindow;
                mainWindow.NumberOfDigitalOutputs.Text = Context.DigitalOutputs.Count().ToString();

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

            // Is the Initial Value valid
            if (string.IsNullOrEmpty(CurrentValue.Text) || !int.TryParse(CurrentValue.Text, out int CurrentValueValue))
            {
                errorMessage += "Nevalidna trenutna vrijednost!";
                return true;
            }
            if (!new List<int>() { 0, 1 }.Contains(CurrentValueValue))
            {
                errorMessage += "Nevalidna početna vrijednost! Mora biti 0 ili 1!";
                return true;
            }

            return false;
        }
    }
}
