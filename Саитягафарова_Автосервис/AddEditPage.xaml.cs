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
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Service _currentService = new Service();
        private bool _isEditing = false;

        public AddEditPage(Service SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
            {
                _currentService = SelectedService;
                _isEditing = true; // Режим редактирования
            }
            else
            {
                _currentService = new Service();
                _isEditing = false; // Режим добавления
            }

            DataContext = _currentService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Title))
                errors.AppendLine("Укажите название услуги");
            if (_currentService.Cost == 0)
                errors.AppendLine("Укажите стоимость услуги");

            if (_currentService.Duration == 0)
                errors.AppendLine("Укажите длительность услуги");

            if (_currentService.Duration > 240 || _currentService.Duration < 0)
                errors.AppendLine("Длительность не может быть больше 240 минут или меньше нуля");

            if (_currentService.Discount < 0 || _currentService.Discount > 100)
                errors.AppendLine("Укажите скидку от 0 до 100");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Проверка на дублирование названия услуги
            if (!_isEditing) // Только при добавлении новой услуги
            {
                var existingService = СаитягафароваавтосервисEntities.GetContext().Service
                    .FirstOrDefault(s => s.Title == _currentService.Title);

                if (existingService != null)
                {
                    MessageBox.Show("Уже существует такая услуга");
                    return;
                }
            }
            else // При редактировании - проверяем, не занято ли название другой услугой
            {
                var existingService = СаитягафароваавтосервисEntities.GetContext().Service
                    .FirstOrDefault(s => s.Title == _currentService.Title && s.ID != _currentService.ID);

                if (existingService != null)
                {
                    MessageBox.Show("Уже существует другая услуга с таким названием");
                    return;
                }
            }

            // Сохранение изменений
            if (_currentService.ID == 0) // Новая услуга
            {
                СаитягафароваавтосервисEntities.GetContext().Service.Add(_currentService);
            }

            try
            {
                СаитягафароваавтосервисEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}
