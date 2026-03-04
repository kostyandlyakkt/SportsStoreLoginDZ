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
    public partial class DataGrid : Window
    {
        private StoreDBEntities1 db = new StoreDBEntities1();

        public DataGrid()
        {
            InitializeComponent();
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
    }
}
