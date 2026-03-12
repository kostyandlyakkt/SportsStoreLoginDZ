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
    public partial class ProductWindow : Window
    {
        private StoreDBEntities1 db = new StoreDBEntities1();

        public ProductWindow()
        {
            InitializeComponent();
        }

        public void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid dataGridWin = new DataGrid();
            dataGridWin.Show();
            this.Close();
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid dataGridWin = new DataGrid();
            dataGridWin.Show();
            this.Close();
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                var selectedCategoryItem = cmbCategory.SelectedItem as ComboBoxItem;
                string categoryName = selectedCategoryItem?.Content.ToString();

                if (string.IsNullOrEmpty(name) || selectedCategoryItem == null)
                {
                    MessageBox.Show("Заполните обязательные поля");
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Введите корректную цену");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    MessageBox.Show("Введите корректное количество");
                    return;
                }

                var selectedStatusItem = cmbStatus.SelectedItem as ComboBoxItem;
                string status = selectedStatusItem?.Content.ToString() ?? "В наличии";

                var category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
                if (category == null)
                {
                    MessageBox.Show("Выбранная категория не найдена в базе данных");
                    return;
                }

                Products newProduct = new Products
                {
                    Name = name,
                    CategoryId = category.Id,
                    Price = price,
                    Quantity = quantity,
                    Status = status,
                    AddedDate = DateTime.Now,
                };

                db.Products.Add(newProduct);
                db.SaveChanges();

                MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DataGrid dataGridWin = new DataGrid();
                dataGridWin.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}", "Ошибка");
            }
        }
    }
}
