using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    public partial class DataGrid : Window
    {
        private StoreDBEntities1 db = new StoreDBEntities1();

        public DataGrid(String username = "Администратор")
        {
            InitializeComponent();
            txtUsername.Text = username;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var productsList = db.Products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    Category = p.Categories.Name,
                    p.Price,
                    p.Quantity,
                    p.Status,
                    p.AddedDate
                }).ToList();

                dgProducts.ItemsSource = productsList;

                txtTotalItems.Text = productsList.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        public void btnExit_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(LoginWindow.GetSessionFilePath());

            LoginWindow loginWin = new LoginWindow();
            loginWin.Show();
            this.Close();
        }

        public void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();

            productWindow.Show();

            this.Close();
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgProducts.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите товар для удаления.");
                return;
            }

            dynamic product = selectedItem;
            int productId = product.Id;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить товар с ID {productId}?",
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var productToDelete = db.Products.Find(productId);
                    if (productToDelete != null)
                    {
                        db.Products.Remove(productToDelete);
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Товар успешно удален.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;

            if (status == "В наличии")
            {
                return new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
            else if (status == "Мало")
            {
                return new SolidColorBrush(Color.FromRgb(255, 193, 7));
            }
            else if (status == "Нет в наличии")
            {
                return new SolidColorBrush(Color.FromRgb(244, 67, 54));
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
