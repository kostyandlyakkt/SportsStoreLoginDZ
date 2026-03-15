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
using System.Xml.Linq;

namespace SportsStoreLogin
{
    public partial class UserWindow : Window
    {
        private StoreDBEntities1 db = new StoreDBEntities1();
        private Users currentUser;

        public UserWindow(int? userId = null)
        {
            InitializeComponent();

            if (userId.HasValue)
            {
                currentUser = db.Users.Find(userId.Value);
                this.DataContext = new { WindowTitle = "Редактирование пользователя" };
            }
            else
            {
                currentUser = new Users { CreatedAt = DateTime.Now };
                this.DataContext = new { WindowTitle = "Добавление пользователя" };
            }

            FillFields();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) 
        {
            UserGrid userGrid = new UserGrid();
            userGrid.Show();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserGrid userGrid = new UserGrid();
            userGrid.Show();
            this.Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                var selectedRoleItem = cmbRole.SelectedItem as ComboBoxItem;

                if (string.IsNullOrEmpty(email) || selectedRoleItem == null)
                {
                    Message.ShowWarn("Заполните обязательные поля");
                    return;
                }

                if (!Validation.IsValidEmail(email))
                {
                    Message.ShowWarn("Введите корректный email");
                    return;
                }

                var existingUser = await Task.Run(() => db.Users.FirstOrDefault(u => u.Email == email));
                if (existingUser != null)
                {
                    Message.ShowError("Пользователь с таким email уже зарегистрирован");
                    return;
                }

                currentUser.Email = email;
                currentUser.Role = cmbRole.Text.Trim();
                currentUser.PasswordHash = txtPassword.Text.Trim();

                if (currentUser.Id == 0)
                {
                    if (currentUser.CreatedAt == default(DateTime))
                    {
                        currentUser.CreatedAt = DateTime.Now;
                    }
                    db.Users.Add(currentUser);
                }

                db.SaveChanges();

                if (currentUser.Id == 0)
                {
                    Message.ShowInfo("Пользователь успешно добавлен");
                }
                else
                {
                    Message.ShowInfo("Данные пользователя обновлены");
                }

                UserGrid userGrid = new UserGrid();
                userGrid.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Message.ShowError($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private void FillFields()
        {
            if (currentUser.Id == 0)
            {
                return;
            }

            txtEmail.Text = currentUser.Email;
            txtPassword.Text = currentUser.PasswordHash;

            foreach (ComboBoxItem item in cmbRole.Items)
            {
                if (item.Content.ToString() == currentUser.Role)
                {
                    cmbRole.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
