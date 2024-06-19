using DataConcentrator;
using DataConcentrator.src;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class AddDigitalInput : Window
    {
        private readonly DataConcentratorContext Context = DataConcentratorContext.Instance;
        public PLCManager Manager;
        public DataGrid DigitalInputsList;
        public DigitalInput DigitalInput = new DigitalInput();
        public AddDigitalInput(PLCManager manager, DataGrid digitalInputsList)
        {
            InitializeComponent();

            Manager = manager;
            Address.ItemsSource = Manager.AvailibleDigitalInputs;
            OnOffScan.ItemsSource = new List<string> { "Uključi", "Isključi" };

            DigitalInputsList = digitalInputsList;
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var mainWindow = Owner as MainWindow;

            try
            {
                Context.DigitalInputs.Add(DigitalInput);
                DigitalInput.Scanner = new Thread(() => DigitalInput.Scan(Manager));
                DigitalInput.PropertyChanged += mainWindow.UpdateDigitalDataGrid;
                DigitalInput.Scanner.Start();

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

            mainWindow.NumberOfDigitalInputs.Text = Context.DigitalInputs.Count().ToString();

            Manager.TakeDigitalInput(DigitalInput.Address);
            MessageBox.Show("Uspješno uređen digitalni ulaz!", "Dodaj digitalni ulaz", MessageBoxButton.OK, MessageBoxImage.Information);
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
            DigitalInput.Description = Description.Text;

            // Is the TAG given
            if (string.IsNullOrEmpty(Tag.Text))
            {
                errorMessage += "Niste upisali tag!";
                return true;
            }
            DigitalInput.Id = Tag.Text;

            // Is that tag already in the table
            if (Context.DigitalInputs.Any(digitalInput => digitalInput.Id == Tag.Text))
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
            DigitalInput.Name = Name.Text;

            // Is the I/O ADDRESS selected
            if (Address.SelectedItem == null)
            {
                errorMessage += "Niste selektovali adresu!";
                return true;
            }
            DigitalInput.Address = Address.Text;

            // Is the scanning selected
            if (OnOffScan.SelectedItem == null)
            {
                errorMessage += "Niste selektovali da li želite uključeno ili isključeno skeniranje";
                return true;
            }
            DigitalInput.OnOffScan = OnOffScan.Text == "Uključi";

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
            DigitalInput.ScanTime = ScanTimeValue;

            return false;
        }
    }
}
