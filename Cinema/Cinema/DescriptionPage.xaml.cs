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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema
{
    /// <summary>
    /// Logika interakcji dla klasy DescriptionPage.xaml
    /// </summary>
    public partial class DescriptionPage : Page
    {
        private readonly Movie Movie;

        public DescriptionPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Movie movie) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();

            Movie = movie;

            InitializeMovieData();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void InitializeMovieData()
        {
            TitleTextBlock.Text = Movie.Name;

            DescriptionTextBox.Text = Movie.Description;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(new MovieHoursPage(window, previousPage, sqlConnectionFactory, Movie));
        }
    }
}
