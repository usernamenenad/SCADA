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
    public partial class EditDigitalInput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public DigitalInput DigitalInput;
        public DataGrid DigitalInputsList;
        public EditDigitalInput(DigitalInput digitalInput, PLCManager manager, DataGrid digitalInputsList)
        {
            InitializeComponent();

            DigitalInput = digitalInput;
            Manager = manager;
            DigitalInputsList = digitalInputsList;

            Tag.Text = digitalInput.Id;
            Name.Text = digitalInput.Name;
            Description.Text = digitalInput.Description;

            List<string> AddressDisplay = new List<string>(Manager.AvailibleDigitalInputs)
            {
                digitalInput.Address
            };
            AddressDisplay.Sort();

            Address.ItemsSource = AddressDisplay;
            Address.SelectedItem = digitalInput.Address;

            ScanTime.Text = digitalInput.ScanTime.ToString();

            OnOffScan.ItemsSource = new List<string> { "Uključi", "Isključi" };
            OnOffScan.SelectedItem = digitalInput.OnOffScan ? "Uključi" : "Isključi";
        }
        public void Save(object sender, RoutedEventArgs e)
        {
            if(ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DigitalInput.Name = Name.Text;
            DigitalInput.Description = Description.Text;

            string OldAddress = DigitalInput.Address;
            DigitalInput.Address = Address.Text;

            DigitalInput.OnOffScan = OnOffScan.Text == "Uključi";
            DigitalInput.ScanTime = double.Parse(ScanTime.Text);

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
                        MessageBox.Show($"{ve.PropertyName}, {ve.ErrorMessage}");
                    }
                }
                return;
            }

            DigitalInputsList.ItemsSource = Context.DigitalInputs.ToList();

            var mainWindow = Owner as MainWindow;
            mainWindow.NumberOfDigitalInputs.Text = Context.DigitalInputs.Count().ToString();

            if (OldAddress != Address.Text)
            {            
                Manager.FreeDigitalInput(OldAddress);
                Manager.TakeDigitalInput(Address.Text);
            }

            MessageBox.Show("Uspješno uređen digitalni ulaz!", "Uredi analogni ulaz", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }
        public void Delete(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da želite obrisati analogni izlaz?", "Upozorenje", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DigitalInput.Scanner.Abort();
                    Context.DigitalInputs.Remove(DigitalInput);
                    Context.SaveChanges();

                    Manager.FreeDigitalInput(DigitalInput.Address);
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

                DigitalInputsList.ItemsSource = Context.DigitalInputs.ToList();

                var mainWindow = Owner as MainWindow;
                mainWindow.NumberOfDigitalInputs.Text = Context.DigitalInputs.Count().ToString();

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

            // Is the scanning selected
            if (OnOffScan.SelectedItem == null)
            {
                errorMessage += "Niste selektovali da li želite uključeno ili isključeno skeniranje";
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

            return false;
        }
    }
}
