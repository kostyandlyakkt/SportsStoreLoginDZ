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
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
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

        public void SaveButton_Click(Object sender, RoutedEventArgs e)
        {
            return;
        }
    }
}
