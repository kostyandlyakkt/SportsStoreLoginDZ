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

namespace SportsStoreLogin
{
    public partial class RegisterWindow : Window
    {
        // Инициализируем контекст базы данных
        private StoreDBEntities1 db = new StoreDBEntities1();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";
            errorBorder.Visibility = Visibility.Collapsed;

            string email = txtRegUsername.Text.Trim();
            string password = txtRegPassword.Password;

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Введите email");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введите пароль");
                return;
            }

            try
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == email);
                if (existingUser != null)
                {
                    ShowError("Пользователь с таким email уже зарегистрирован");
                    return;
                }

                Users newUser = new Users
                {
                    Email = email,
                    PasswordHash = password,
                    Role = "admin",
                    CreatedAt = DateTime.Now,
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                MessageBox.Show("Регистрация успешно завершена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoginWindow loginWin = new LoginWindow();
                loginWin.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            errorBorder.Visibility = Visibility.Visible;
        }
    }
}
