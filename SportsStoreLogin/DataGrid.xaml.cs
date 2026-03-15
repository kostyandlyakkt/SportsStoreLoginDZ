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

        private int currentPage = 1;
        private int pageSize = 10;
        private int totalItems = 0;

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
                totalItems = db.Products.Count();

                var productsList = db.Products
                    .OrderBy(p => p.Id)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
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

                txtTotalItems.Text = totalItems.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
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
                Message.ShowWarn("Выберите товар для удаления");
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

                        Message.ShowInfo("Товар успешно удален");
                    }
                }
                catch (Exception ex)
                {
                    Message.ShowError($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgProducts.SelectedItem;
            if (selectedItem == null)
            {
                Message.ShowWarn("Выберите товар для редактирования");
                return;
            }

            dynamic product = selectedItem;
            int productId = product.Id;

            ProductWindow editWin = new ProductWindow(productId);
            editWin.Show();
            this.Close();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;

                int page = int.Parse(leftBtn.Content.ToString());
                leftBtn.Content = page - 1;
                centreBtn.Content = page;
                rightBtn.Content = page + 1;

                checkLimit();
                LoadData();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage * pageSize < totalItems)
            {
                currentPage++;

                int page = int.Parse(leftBtn.Content.ToString());
                leftBtn.Content = page + 1;
                centreBtn.Content = page + 2;
                rightBtn.Content = page + 3;

                if (page + 1 * pageSize > totalItems)
                {
                    centreBtn.Content = 1;
                    rightBtn.Content = 2;
                }

                checkLimit();
                LoadData();
            }
        }

        private void btnPage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int page = int.Parse(btn.Content.ToString());

            leftBtn.Content = page;
            centreBtn.Content = page + 1;
            rightBtn.Content = page + 2;
            if (page * pageSize > totalItems)
            {
                centreBtn.Content = 1;
                rightBtn.Content = 2;
            }

            currentPage = page;
            
            checkLimit();
            LoadData();
        }

        private void checkLimit()
        {
            if (int.Parse(centreBtn.Content.ToString()) * pageSize - totalItems > pageSize)
            {
                centreBtn.Content = 1;
                rightBtn.Content = 2;
            }
            else if(int.Parse(rightBtn.Content.ToString()) * pageSize - totalItems > pageSize)
            {
                rightBtn.Content = 1;
            }
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
