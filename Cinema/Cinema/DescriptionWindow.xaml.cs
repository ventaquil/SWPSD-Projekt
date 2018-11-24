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
        private SqlConnectionFactory sqlConnectionFactory;
        private string movieTitle;
        private string movieDescription;

        public DescriptionWindow(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, string movieTitle)
        {
            this.window = window;
            this.previousPage = previousPage;
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.movieTitle = movieTitle;

            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            TitleTextBlock.Text = movieTitle;
        }

        public DescriptionWindow(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, string movieTitle, string movieDescription) : this(window, previousPage, sqlConnectionFactory, movieTitle)
        {
            this.movieDescription = movieDescription;

            DescriptionTextBox.Text = movieDescription;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = new MovieHoursPage(window, previousPage, sqlConnectionFactory, movieTitle);

            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
