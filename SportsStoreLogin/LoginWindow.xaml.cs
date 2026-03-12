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

namespace SportsStoreLogin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private StoreDBEntities1 db = new StoreDBEntities1();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Проверка полей
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError("Введите email");
                return;
            }

            if (txtPassword.Password == "")
            {
                ShowError("Введите пароль");
                return;
            }

            try
            {
                // Ищем пользователя в базе
                var user = db.Users.FirstOrDefault(u => u.Email == txtUsername.Text);

                if (user == null)
                {
                    ShowError("Пользователь не найден");
                    return;
                }

                // Проверяем пароль
                if (user.PasswordHash == txtPassword.Password)
                {
                    // Успешный вход
                    MessageBox.Show($"Добро пожаловать, {user.Email}!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    DataGrid dataGridWin = new DataGrid(user.Email);

                    dataGridWin.Show();

                    this.Close();
                }
                else
                {
                    ShowError("Неверный пароль");
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка подключения к БД: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e) 
        {
            RegisterWindow registerWin = new RegisterWindow();

            registerWin.Show();

            this.Close();
        }
    }
}
