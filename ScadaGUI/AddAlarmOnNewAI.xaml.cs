using DataConcentrator.src;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;

namespace ScadaGUI
{
    public struct AlarmGUIItem
    {
        public string Type;
        public CheckBox CheckBox;
        public TextBox Id;
        public TextBox Name;
        public TextBox Description;
        public ComboBox IsActive;
        public TextBox ActivationValue;
        public ComboBox ActivatesAt;
    }
    public partial class AddAlarmOnNewAI : Window
    {
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string TagId { get; set; }
        public string TagName { get; set; }
        public List<Alarm> AnalogInputAlarms { get; set; }
        public static Dictionary<string, AlarmPriority> PriorityMap { get; set; } = new Dictionary<string, AlarmPriority>()
        {
            {"Najniži", AlarmPriority.Lowest },
            {"Nizak", AlarmPriority.Low },
            {"Srednji", AlarmPriority.Medium },
            {"Visok", AlarmPriority.High },
            {"Najviši", AlarmPriority.Highest},
        };
        public static Dictionary<string, AlarmActivationEdge> EdgeMap { get; set; } = new Dictionary<string, AlarmActivationEdge>()
        {
            {"Uzlazna ivica", AlarmActivationEdge.Rising },
            {"Silazna ivica", AlarmActivationEdge.Falling },
        };
        public AddAlarmOnNewAI(string tagId, string tagName, double LowLimitValue, double HighLimitValue, List<Alarm> analogInputAlarms)
        {
            InitializeComponent();

            List<string> EdgeValues = EdgeMap.Keys.ToList();
            List<string> IsActiveValues = new List<string>() { "Aktivan", "Neaktivan" };


            LowestAlarmActivatesAt.ItemsSource = EdgeValues;
            LowAlarmActivatesAt.ItemsSource = EdgeValues;
            MediumAlarmActivatesAt.ItemsSource = EdgeValues;
            HighAlarmActivatesAt.ItemsSource = EdgeValues;
            HighestAlarmActivatesAt.ItemsSource = EdgeValues;

            LowestAlarmIsActive.ItemsSource = IsActiveValues;
            LowAlarmIsActive.ItemsSource = IsActiveValues;
            MediumAlarmIsActive.ItemsSource = IsActiveValues;
            HighAlarmIsActive.ItemsSource = IsActiveValues;
            HighestAlarmIsActive.ItemsSource = IsActiveValues;


            TagId = tagId;
            TagName = tagName;
            LowLimit = LowLimitValue;
            HighLimit = HighLimitValue;
            AnalogInputAlarms = analogInputAlarms;
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            if(!ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Uspješno dodat alarm!");
            Close();
        }
        public bool ValidateInputs(out string errorMessage)
        {
            List<AlarmGUIItem> alarmGUIItems = new List<AlarmGUIItem>();

            errorMessage = string.Empty;
            alarmGUIItems.Add(new AlarmGUIItem()
            {
                Type = "Najniži",
                CheckBox = LowestAlarmCheckBox,
                Id = LowestAlarmId,
                Name = LowestAlarmName,
                Description = LowestAlarmDescription,
                IsActive = LowestAlarmIsActive,
                ActivationValue = LowestAlarmActivationValue,
                ActivatesAt = LowestAlarmActivatesAt,
            });
            alarmGUIItems.Add(new AlarmGUIItem()
            {
                Type = "Nizak",
                CheckBox = LowAlarmCheckBox,
                Id = LowAlarmId,
                Name = LowAlarmName,
                Description = LowAlarmDescription,
                IsActive = LowAlarmIsActive,
                ActivationValue = LowAlarmActivationValue,
                ActivatesAt = LowAlarmActivatesAt,
            });
            alarmGUIItems.Add(new AlarmGUIItem()
            {
                Type = "Srednji",
                CheckBox = MediumAlarmCheckBox,
                Id = MediumAlarmId,
                Name = MediumAlarmName,
                Description = MediumAlarmDescription,
                IsActive = MediumAlarmIsActive,
                ActivationValue = MediumAlarmActivationValue,
                ActivatesAt = MediumAlarmActivatesAt,
            });
            alarmGUIItems.Add(new AlarmGUIItem()
            {
                Type = "Visok",
                CheckBox = HighAlarmCheckBox,
                Id = HighAlarmId,
                Name = HighAlarmName,
                Description= HighAlarmDescription,
                IsActive = HighAlarmIsActive,
                ActivationValue = HighAlarmActivationValue,
                ActivatesAt = HighAlarmActivatesAt,
            });
            alarmGUIItems.Add(new AlarmGUIItem()
            {
                Type = "Najviši",
                CheckBox = HighestAlarmCheckBox,
                Id = HighestAlarmId,
                Name = HighestAlarmName,
                Description = HighestAlarmDescription,
                IsActive = HighestAlarmIsActive,
                ActivationValue = HighestAlarmActivationValue,
                ActivatesAt = HighestAlarmActivatesAt,
            });

            foreach(var alarmGUIItem in alarmGUIItems)
            {
                if(!ValidateSingleInput(alarmGUIItem, out string error))
                {
                    errorMessage = error;
                    AnalogInputAlarms.Clear();
                    return false;
                }
                if(alarmGUIItem.CheckBox.IsChecked == true)
                {
                    AnalogInputAlarms.Add(new Alarm()
                    {
                        Id = alarmGUIItem.Id.Text,
                        Name = alarmGUIItem.Name.Text,
                        Description = alarmGUIItem.Description.Text,
                        IsActive = alarmGUIItem.IsActive.Text == "Aktivan",
                        ActivationValue = double.Parse(alarmGUIItem.ActivationValue.Text),
                        ActivationEdge = EdgeMap[alarmGUIItem.ActivatesAt.Text],
                        Priority = PriorityMap[alarmGUIItem.Type],
                        AnalogInputId = TagId,
                        AnalogInputName = TagName,
                    });
                }
            }
            return true;
        }
        public bool ValidateSingleInput(AlarmGUIItem alarmGUIItem, out string error)
        {
            error = string.Empty;
            if (alarmGUIItem.CheckBox.IsChecked == true)
            {
                if (string.IsNullOrEmpty(alarmGUIItem.Id.Text))
                {
                    error += $"Niste unijeli ID alarma tipa \"{alarmGUIItem.Type}\"!";
                    return false;
                }

                if (string.IsNullOrEmpty(alarmGUIItem.Name.Text))
                {
                    error += $"Niste unijeli ime alarma tipa \"{alarmGUIItem.Type}\"!";
                    return false;
                }

                // Is the activation of the alarm set
                if (alarmGUIItem.IsActive.SelectedItem == null)
                {
                    error += $"Niste selektovali da li je alarm tipa \"{alarmGUIItem.Type}\" aktiviran!";
                    return false;
                }
                if(alarmGUIItem.ActivatesAt.SelectedItem == null)
                {
                    error += $"Niste selektovali na koju ivicu tipa \"{alarmGUIItem.Type}\" se aktivira!";
                    return false;
                }
                if (!double.TryParse(alarmGUIItem.ActivationValue.Text, out double value))
                {
                    error += $"Nevalidan alarm tipa \"{alarmGUIItem.Type}\"!";
                    return false;
                }
                if (value <= LowLimit || value > HighLimit)
                {
                    error += $"Vrijednost alarma tipa \"{alarmGUIItem.Type}\" ne može biti ispod ili iznad granica alarma!";
                    return false;
                }
            }
            return true;
        }
    }
}
