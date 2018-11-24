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
        private Movie[] Movies;

        public OrderPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();

            ListMovies();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        public Movie[] GetMovies()
        {
            if (Movies == null)
            {
                List<Movie> movies = new List<Movie>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "SELECT DISTINCT Movies.title, Movies.description " +
                            "FROM Movies, Screenings " +
                            "WHERE (Movies.id = Screenings.movieID) AND (Screenings.screeningDate = CONVERT(date, GETDATE()))";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            string name = string.Format("{0}", sqlDataReader[0]);
                            string description = string.Format("{0}", sqlDataReader[1]);

                            movies.Add(new Movie(name, description));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                Movies = movies.ToArray();
            }

            return Movies;
        }
        private void ListMovies()
        {
            foreach (Movie movie in GetMovies())
            {
                MoviesListBox.Items.Add(movie.Name);
            }
        }

        private void MoviesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Movie movie = GetMovies()[MoviesListBox.SelectedIndex];

                ChangePage(new MovieHoursPage(window, this, sqlConnectionFactory, movie));
            }
            catch (IndexOutOfRangeException)
            {
            }
        }
    }
}
