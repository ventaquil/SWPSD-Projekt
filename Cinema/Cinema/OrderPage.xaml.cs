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
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        private Window window;
        private Page lastPage;
        private SqlConnection dbConnection;

        public OrderPage(Window window, Page lastPage, SqlConnection dbConnection)
        {
            this.window = window;
            this.lastPage = lastPage;
            this.dbConnection = dbConnection;

            InitializeComponent();
            ListMovies();
        }

        private void ListMovies()
        {
            dbConnection.Open();
            SqlCommand command = new SqlCommand(
                "select distinct Movies.title " +
                "from Movies, Screenings " +
                "where Movies.id = Screenings.movieID " +
                "and Screenings.screeningDate = CONVERT(date,  GETDATE())", 
                dbConnection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                MoviesListBox.Items.Add(String.Format("{0}", reader[0]));
            dbConnection.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = lastPage;
        }

        private void MoviesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.Format("{0}", MoviesListBox.SelectedItem).Equals(""))
                window.Content = new MovieHoursPage(window, dbConnection, this, String.Format("{0}", MoviesListBox.SelectedItem));
        }
    }
}
