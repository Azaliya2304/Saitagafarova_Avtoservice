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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Саитягафарова_Автосервис
{
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;

            DataContext = _currentService;

            var _currentClient = СаитягафароваавтосервисEntities.GetContext().Client.ToList();
            ComboClient.ItemsSource = _currentClient;
        }
        private ClientService _currentClientService = new ClientService();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");
            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");
            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            // ПРОВЕРКА ВРЕМЕНИ НАЧАЛА (НЕ БОЛЕЕ 24:00)
            if (!string.IsNullOrEmpty(TBStart.Text))
            {
                try
                {
                    string[] timeParts = TBStart.Text.Split(':');
                    if (timeParts.Length == 2)
                    {
                        int hours = Convert.ToInt32(timeParts[0]);
                        int minutes = Convert.ToInt32(timeParts[1]);

                        // Проверка, что часы не больше 24
                        if (hours > 24)
                        {
                            errors.AppendLine("Часы не могут быть больше 24");
                        }
                        // Проверка, что если часы = 24, то минуты должны быть 0
                        else if (hours == 24 && minutes > 0)
                        {
                            errors.AppendLine("При 24 часах минуты должны быть 0 (24:00)");
                        }
                        // Проверка, что минуты в диапазоне 0-59
                        else if (minutes < 0 || minutes > 59)
                        {
                            errors.AppendLine("Минуты должны быть в диапазоне от 0 до 59");
                        }
                        // Проверка, что часы не отрицательные
                        else if (hours < 0)
                        {
                            errors.AppendLine("Часы не могут быть отрицательными");
                        }
                    }
                    else
                    {
                        errors.AppendLine("Время должно быть в формате ЧЧ:ММ");
                    }
                }
                catch (Exception)
                {
                    errors.AppendLine("Некорректный формат времени.");
                }
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);
            if (_currentClientService.ID == 0)
                СаитягафароваавтосервисEntities.GetContext().ClientService.Add(_currentClientService);

            try
            {
                СаитягафароваавтосервисEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length < 5 || !s.Contains(':'))
            {
                TBEnd.Text = "";
                return;
            }

            try
            {
                string[] start = s.Split(new char[] { ':' });
                int startHour = Convert.ToInt32(start[0].ToString());
                int startMin = Convert.ToInt32(start[1].ToString());
                // Проверка на корректность времени
                if (startHour > 24 || (startHour == 24 && startMin > 0) ||
                    startHour < 0 || startMin < 0 || startMin > 59)
                {
                    MessageBox.Show("Введено некорректное время начала");
                    return;
                }

                int startHourInMinutes = startHour * 60;
                int sum = startHourInMinutes + startMin + _currentService.Duration;

                int EndHour;
                int EndMin;
                bool nextDay = false;

                // Проверка, что окончание не превышает 24:00
                if (sum > 24 * 60)
                {
                    nextDay = true;
                    sum = sum - 1440; // Вычитаем 24 часа (переход на следующий день)

                }

                EndHour = sum / 60;
                EndMin = sum % 60;
                s = EndHour.ToString("D2") + ":" + EndMin.ToString("D2");

                if (nextDay)
                {
                    TBEnd.Text = s + " (след. день)";
                }
                else
                {
                    TBEnd.Text = s;
                }
            }
            catch
            {
                TBEnd.Text = "Ошибка ввода";
            }
        }
    }
}
