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
        private Page previousPage;
        private SqlConnection sqlConnection;
        private string movieTitle;

        public DescriptionWindow(Window window, Page previousPage, SqlConnection sqlConnection, string movieTitle)
        {
            this.window = window;
            this.previousPage = previousPage;
            this.sqlConnection = sqlConnection;
            this.movieTitle = movieTitle;

            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            TitleTextBlock.Text = movieTitle;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = new MovieHoursPage(window, previousPage, sqlConnection, movieTitle);

            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
