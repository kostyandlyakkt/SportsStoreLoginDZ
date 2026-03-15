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
        private Products currentProduct;

        public ProductWindow(int? productId = null)
        {
            InitializeComponent();

            if (productId.HasValue)
            {
                currentProduct = db.Products.Find(productId.Value);
                this.DataContext = new { WindowTitle = "Редактирование товара", AddedDate = currentProduct.AddedDate };
            }
            else
            {
                currentProduct = new Products { AddedDate = DateTime.Now };
                this.DataContext = new { WindowTitle = "Добавление товара", AddedDate = currentProduct.AddedDate };
            }

            FillFields();
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

                if (string.IsNullOrEmpty(name) || selectedCategoryItem == null)
                {
                    Message.ShowWarn("Заполните обязательные поля");
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    Message.ShowWarn("Введите корректную цену");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    Message.ShowWarn("Введите корректное количество");
                    return;
                }

                string categoryName = selectedCategoryItem.Content.ToString();
                var category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
                if (category == null)
                {
                    Message.ShowWarn("Выбранная категория не найдена в базе данных");
                    return;
                }

                currentProduct.Name = name;
                currentProduct.CategoryId = category.Id;
                currentProduct.Price = price;
                currentProduct.Quantity = quantity;

                var selectedStatusItem = cmbStatus.SelectedItem as ComboBoxItem;
                currentProduct.Status = selectedStatusItem?.Content.ToString() ?? "В наличии";

                currentProduct.Manufacturer = string.IsNullOrWhiteSpace(txtManufacturer.Text) ? null : txtManufacturer.Text.Trim();
                currentProduct.Article = string.IsNullOrWhiteSpace(txtArticle.Text) ? null : txtArticle.Text.Trim();
                currentProduct.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim();
                currentProduct.Size = string.IsNullOrWhiteSpace(txtSize.Text) ? null : txtSize.Text.Trim();
                currentProduct.Color = string.IsNullOrWhiteSpace(txtColor.Text) ? null : txtColor.Text.Trim();
                currentProduct.Material = string.IsNullOrWhiteSpace(txtMaterial.Text) ? null : txtMaterial.Text.Trim();

                if (!string.IsNullOrWhiteSpace(txtWeight.Text))
                {
                    if (decimal.TryParse(txtWeight.Text.Trim(), out decimal weightValue))
                    {
                        currentProduct.Weight = weightValue;
                    }
                    else
                    {
                        Message.ShowWarn("Вес должен быть числом!");
                        return;
                    }
                }
                else
                {
                    currentProduct.Weight = null;
                }

                if (currentProduct.Id == 0)
                {
                    if (currentProduct.AddedDate == default(DateTime))
                    {
                        currentProduct.AddedDate = DateTime.Now;
                    }
                    db.Products.Add(currentProduct);
                }

                db.SaveChanges();

                if (currentProduct.Id == 0) 
                {
                    Message.ShowInfo("Товар успешно добавлен");
                }
                else
                {
                    Message.ShowInfo("Данные товара обновлены");
                }

                DataGrid dataGridWin = new DataGrid();
                dataGridWin.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Message.ShowError($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private void FillFields()
        {
            if (currentProduct.Id == 0)
            {
                return;
            }

            txtName.Text = currentProduct.Name;
            txtPrice.Text = currentProduct.Price.ToString();
            txtQuantity.Text = currentProduct.Quantity.ToString();
            txtManufacturer.Text = currentProduct.Manufacturer;
            txtArticle.Text = currentProduct.Article;
            txtDescription.Text = currentProduct.Description;
            txtWeight.Text = currentProduct.Weight.ToString();
            txtSize.Text = currentProduct.Size;
            txtColor.Text = currentProduct.Color;
            txtMaterial.Text = currentProduct.Material;

            foreach (ComboBoxItem item in cmbCategory.Items)
            {
                if (item.Content.ToString() == currentProduct.Categories?.Name)
                {
                    cmbCategory.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Content.ToString() == currentProduct.Status)
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
