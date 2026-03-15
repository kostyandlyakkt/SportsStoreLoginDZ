using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    public partial class UserGrid : Window
    {
        private StoreDBEntities1 db = new StoreDBEntities1();

        public UserGrid(String username = "Суперадмин")
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var usersList = db.Users.Select(p => new
                {
                    p.Id,
                    p.Email,
                    p.Role,
                    p.CreatedAt,
                    p.LastLogin,
                }).ToList();

                dgUsers.ItemsSource = usersList;

                txtTotalItems.Text = usersList.Count.ToString();
            }
            catch (Exception ex)
            {
                Message.ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(LoginWindow.GetSessionFilePath());

            LoginWindow loginWin = new LoginWindow();
            loginWin.Show();
            this.Close();
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow();
            userWindow.Show();
            this.Close();
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgUsers.SelectedItem;
            if (selectedItem == null)
            {
                Message.ShowWarn("Выберите пользователя для редактирования");
                return;
            }

            dynamic user = selectedItem;
            int userId = user.Id;

            UserWindow editUserWin = new UserWindow(userId);
            editUserWin.Show();
            this.Close();
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgUsers.SelectedItem;
            if (selectedItem == null)
            {
                Message.ShowWarn("Выберите пользователя для удаления");
                return;
            }

            dynamic user = selectedItem;
            int userId = user.Id;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя с ID {userId}?",
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var userToDelete = db.Users.Find(userId);
                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                        LoadData();

                        Message.ShowInfo("Товар успешно удален");
                    }
                }
                catch (Exception ex)
                {
                    Message.ShowError($"Ошибка при удалении: {ex.Message}");
                }
            }
        }
    }
}
