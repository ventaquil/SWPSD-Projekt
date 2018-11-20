using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public OrderPage(Window window, Page previousPage, SqlConnection sqlConnection) : base(window, previousPage, sqlConnection)
        {
            InitializeComponent();

            ListMovies();
        }

        private void ListMovies()
        {
            sqlConnection.Open();

            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = "select distinct Movies.title " +
                    "from Movies, Screenings " +
                    "where Movies.id = Screenings.movieID " +
                    "and Screenings.screeningDate = CONVERT(date,  GETDATE())";

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    MoviesListBox.Items.Add(String.Format("{0}", sqlDataReader[0]));
                }
                sqlDataReader.Close();
            }

            sqlConnection.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void MoviesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.Format("{0}", MoviesListBox.SelectedItem).Equals(""))
            {
                ChangePage(new MovieHoursPage(window, this, sqlConnection, String.Format("{0}", MoviesListBox.SelectedItem)));
            }
        }
    }
}
