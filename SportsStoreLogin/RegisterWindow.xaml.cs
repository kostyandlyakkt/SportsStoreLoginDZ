using System;
using System.Collections.Generic;
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

namespace SportsStoreLogin
{
    public partial class RegisterWindow : Window
    {
        private StoreDBEntities1 db = new StoreDBEntities1();
        private CancellationTokenSource _cts;

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            _cts = new CancellationTokenSource();
            var animationTask = Gui.loadAnimation(btnRegister, _cts.Token);

            string email = txtRegUsername.Text.Trim();
            string password = txtRegPassword.Password;

            if (string.IsNullOrWhiteSpace(email))
            {
                Message.ShowWarn("Введите email");
                _cts.Cancel();
                return;
            }

            if (!Validation.IsValidEmail(email)) 
            {
                Message.ShowError("Введите корректный email");
                _cts.Cancel();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Message.ShowWarn("Введите пароль");
                _cts.Cancel();
                return;
            }

            try
            {
                var existingUser = await Task.Run(() => db.Users.FirstOrDefault(u => u.Email == email));
                if (existingUser != null)
                {
                    Message.ShowError("Пользователь с таким email уже зарегистрирован");
                    _cts.Cancel();
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

                _cts.Cancel();

                Message.ShowInfo("Регистрация успешно завершена!");

                LoginWindow loginWin = new LoginWindow();
                loginWin.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Message.ShowError($"Ошибка при сохранении: {ex.Message}");
                _cts.Cancel();
            }
        }
    
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
