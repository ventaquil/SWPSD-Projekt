using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Window window;
        private SqlConnection dbConnection;

        public MainPage(Window window, SqlConnection dbConnection)
        {
            this.window = window;
            this.dbConnection = dbConnection;
            InitializeComponent();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = new OrderPage(window, this, dbConnection);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = new SearchPage(window, this, dbConnection);
        }
    }
}
