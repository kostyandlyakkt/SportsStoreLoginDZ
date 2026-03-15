using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace SportsStoreLogin
{
    public partial class LoginWindow : Window
    {
        private StoreDBEntities1 db = new StoreDBEntities1();
        private CancellationTokenSource _cts;

        public LoginWindow()
        {
            InitializeComponent();
            loadLoginData();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            await login(txtUsername.Text, txtPassword.Password);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWin = new RegisterWindow();
            registerWin.Show();
            this.Close();
        }

        private async Task login(string userEmail, string userPassword)
        {
            _cts = new CancellationTokenSource();

            try
            {
                var animationTask = Gui.loadAnimation(btnLogin, _cts.Token);

                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    Message.ShowError("Введите email");
                    _cts.Cancel();
                    return;
                }

                if (!Validation.IsValidEmail(userEmail))
                {
                    Message.ShowError("Введите корректный email");
                    _cts.Cancel();
                    return;
                }

                if (userPassword == "")
                {
                    Message.ShowError("Введите пароль");
                    _cts.Cancel();
                    return;
                }

                var user = await Task.Run(() => db.Users.FirstOrDefault(u => u.Email == userEmail));

                if (user == null)
                {
                    Message.ShowError("Пользователь не найден");
                    _cts.Cancel();
                    return;
                }

                if (user.PasswordHash == userPassword)
                {
                    if (chkRemember.IsChecked == true)
                    {
                        writeLoginData(user.Email, user.PasswordHash);
                    }

                    _cts.Cancel();
                    user.LastLogin = DateTime.Now;
                    db.SaveChanges();
                    Message.ShowInfo($"Добро пожаловать, {user.Email}!");

                    if (user.Role == "superadmin")
                    {
                        UserGrid userGridWin = new UserGrid();
                        userGridWin.Show();
                        this.Close();
                    }
                    else
                    {
                        DataGrid dataGridWin = new DataGrid(userEmail);
                        dataGridWin.Show();
                        this.Close();
                    }
                }
                else
                {
                    Message.ShowError("Неверный пароль");
                    _cts.Cancel();
                }
            }
            catch (Exception ex)
            {
                Message.ShowError($"Ошибка подключения к БД: {ex.Message}");
                _cts.Cancel();
            }
        }

        public static string GetSessionFilePath()
        {
            return System.IO.Path.Combine(Directory.GetCurrentDirectory(), "session");
        }

        private bool isCreatedSessionFile()
        {
            if (File.Exists(GetSessionFilePath()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void writeLoginData(string userEmail, string userPassword)
        {
            using (FileStream fstream = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "session"), FileMode.OpenOrCreate))
            {
                string loginData = userEmail + "\n" + userPassword;
                byte[] buffer = Encoding.Default.GetBytes(loginData);
                fstream.Write(buffer, 0, buffer.Length);
            }
        }

        private void loadLoginData()
        {
            if (isCreatedSessionFile())
            {
                using (FileStream fstream = File.OpenRead(GetSessionFilePath()))
                {
                    byte[] buffer = new byte[fstream.Length];
                    fstream.Read(buffer, 0, buffer.Length);
                    string textFromFile = Encoding.Default.GetString(buffer);
                    var parts = textFromFile.Split('\n');

                    login(parts[0], parts[1]);
                }
            }
        }
    }
}
