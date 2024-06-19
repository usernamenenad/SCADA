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
        public List<AlarmPriority> ExistingAlarms { get; set; } = new List<AlarmPriority>();
        public List<Alarm> AlarmsToRemove { get; set; } = new List<Alarm>();
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
        public AddAlarmOnNewAI(string tagId, string tagName, double LowLimitValue, double HighLimitValue, List<Alarm> analogInputAlarms, List<Alarm> alarmsToRemove)
        {
            InitializeComponent();

            List<string> EdgeValues = EdgeMap.Keys.ToList();

            if(analogInputAlarms.Any())
            {
                FillAlarms(analogInputAlarms);
            }

            LowestAlarmActivatesAt.ItemsSource = EdgeValues;
            LowAlarmActivatesAt.ItemsSource = EdgeValues;
            MediumAlarmActivatesAt.ItemsSource = EdgeValues;
            HighAlarmActivatesAt.ItemsSource = EdgeValues;
            HighestAlarmActivatesAt.ItemsSource = EdgeValues;

            TagId = tagId;
            TagName = tagName;
            LowLimit = LowLimitValue;
            HighLimit = HighLimitValue;
            AnalogInputAlarms = analogInputAlarms;
            AlarmsToRemove = alarmsToRemove;
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
        public void Cancel(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da želite otkazati dodavanje?", "Greška", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        public void FillAlarms(List<Alarm> alarms)
        {
            foreach (Alarm alarm in alarms)
            {
                ExistingAlarms.Add(alarm.Priority);

                switch(alarm.Priority)
                {
                    case AlarmPriority.Lowest:
                        LowestAlarmCheckBox.IsChecked = true;
                        LowestAlarmId.Text = alarm.Id;
                        LowestAlarmName.Text = alarm.Name;
                        LowestAlarmDescription.Text = alarm.Description;
                        LowestAlarmActivationValue.Text = alarm.ActivationValue.ToString();
                        LowestAlarmActivatesAt.SelectedItem = alarm.ActivationEdge == AlarmActivationEdge.Rising ? EdgeMap.Keys.ToList()[0] : EdgeMap.Keys.ToList()[1];
                        break;

                    case AlarmPriority.Low:
                        LowAlarmCheckBox.IsChecked = true;
                        LowAlarmId.Text = alarm.Id;
                        LowAlarmName.Text = alarm.Name;
                        LowAlarmDescription.Text = alarm.Description;
                        LowAlarmActivationValue.Text = alarm.ActivationValue.ToString();
                        LowAlarmActivatesAt.SelectedItem = alarm.ActivationEdge == AlarmActivationEdge.Rising ? EdgeMap.Keys.ToList()[0] : EdgeMap.Keys.ToList()[1];
                        break;

                    case AlarmPriority.Medium:
                        MediumAlarmCheckBox.IsChecked = true;
                        MediumAlarmId.Text = alarm.Id;
                        MediumAlarmName.Text = alarm.Name;
                        MediumAlarmDescription.Text = alarm.Description;
                        MediumAlarmActivationValue.Text = alarm.ActivationValue.ToString();
                        MediumAlarmActivatesAt.SelectedItem = alarm.ActivationEdge == AlarmActivationEdge.Rising ? EdgeMap.Keys.ToList()[0] : EdgeMap.Keys.ToList()[1];
                        break;

                    case AlarmPriority.High:
                        HighAlarmCheckBox.IsChecked = true;
                        HighAlarmId.Text = alarm.Id;
                        HighAlarmName.Text = alarm.Name;
                        HighAlarmDescription.Text = alarm.Description;
                        HighAlarmActivationValue.Text = alarm.ActivationValue.ToString();
                        HighAlarmActivatesAt.SelectedItem = alarm.ActivationEdge == AlarmActivationEdge.Rising ? EdgeMap.Keys.ToList()[0] : EdgeMap.Keys.ToList()[1];
                        break;

                    case AlarmPriority.Highest:
                        HighestAlarmCheckBox.IsChecked = true;
                        HighestAlarmId.Text = alarm.Id;
                        HighestAlarmName.Text = alarm.Name;
                        HighestAlarmDescription.Text = alarm.Description;
                        HighestAlarmActivationValue.Text = alarm.ActivationValue.ToString();
                        HighestAlarmActivatesAt.SelectedItem = alarm.ActivationEdge == AlarmActivationEdge.Rising ? EdgeMap.Keys.ToList()[0] : EdgeMap.Keys.ToList()[1];
                        break;
                }
            }
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
                    if (ExistingAlarms.Contains(PriorityMap[alarmGUIItem.Type]))
                    {
                        Alarm existingAlarm = AnalogInputAlarms.First(alarm => alarm.Priority == PriorityMap[alarmGUIItem.Type]);
                        existingAlarm.Id = alarmGUIItem.Id.Text;
                        existingAlarm.Name = alarmGUIItem.Name.Text;
                        existingAlarm.Description = alarmGUIItem.Description.Text;
                        existingAlarm.IsActive = false;
                        existingAlarm.IsAcknowledged = false;
                        existingAlarm.ActivationValue = double.Parse(alarmGUIItem.ActivationValue.Text);
                        existingAlarm.ActivationEdge = EdgeMap[alarmGUIItem.ActivatesAt.Text];
                        existingAlarm.Priority = PriorityMap[alarmGUIItem.Type];
                        existingAlarm.AnalogInputId = TagId;
                        existingAlarm.AnalogInputName = TagName;
                    }
                    else
                    {
                        var newAlarm = new Alarm()
                        {
                            Id = alarmGUIItem.Id.Text,
                            Name = alarmGUIItem.Name.Text,
                            Description = alarmGUIItem.Description.Text,
                            IsActive = false,
                            IsAcknowledged = false,
                            ActivationValue = double.Parse(alarmGUIItem.ActivationValue.Text),
                            ActivationEdge = EdgeMap[alarmGUIItem.ActivatesAt.Text],
                            Priority = PriorityMap[alarmGUIItem.Type],
                            AnalogInputId = TagId,
                            AnalogInputName = TagName,
                        };
                        if(Owner.Owner is MainWindow window)
                        {
                            newAlarm.PropertyChanged += window.UpdateAlarms;
                        }
                        AnalogInputAlarms.Add(newAlarm);
                    }
                }
                else
                {
                    if(ExistingAlarms.Contains(PriorityMap[alarmGUIItem.Type]))
                    {
                        Alarm existingAlarm = AnalogInputAlarms.First(alarm => alarm.Priority == PriorityMap[alarmGUIItem.Type]);
                        AlarmsToRemove.Add(existingAlarm);
                    }
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
