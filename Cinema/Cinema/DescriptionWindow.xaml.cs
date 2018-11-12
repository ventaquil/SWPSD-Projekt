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
using System.Windows.Shapes;

namespace Cinema
{
    /// <summary>
    /// Interaction logic for DescriptionWindow.xaml
    /// </summary>
    public partial class DescriptionWindow : Window
    {
        private Window window;
        private Page lastPage;
        private SqlConnection dbConnection;
        private string movieTitle;

        public DescriptionWindow(Window window, Page lastPage, SqlConnection dbConnection, string movieTitle)
        {
            this.window = window;
            this.lastPage = lastPage;
            this.dbConnection = dbConnection;
            this.movieTitle = movieTitle;

            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            TitleTextBlock.Text = movieTitle;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = new MovieHoursPage(window, dbConnection, lastPage, movieTitle);
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
