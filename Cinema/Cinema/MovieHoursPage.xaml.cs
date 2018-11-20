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
    /// Interaction logic for MovieHoursPage.xaml
    /// </summary>
    public partial class MovieHoursPage : Page
    {
        private String movieTitle;

        private List<int> screeningId;

        public MovieHoursPage(Window window, Page previousPage, SqlConnection sqlConnection, String movieTitle) : base(window, previousPage, sqlConnection)
        {
            this.movieTitle = movieTitle;

            InitializeComponent();

            ListHours();
        }

        private void ListHours()
        {
            screeningId = new List<int>();

            sqlConnection.Open();

            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = "select distinct Screenings.id, Movies.title, Screenings.auditoriumID, Screenings.screeningTime " +
                    "from Movies, Screenings " +
                    "where Movies.id = Screenings.movieID " +
                    "and Movies.title = '" + movieTitle +
                    "' and Screenings.screeningDate = CONVERT(date,  GETDATE())";

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    screeningId.Add(int.Parse(String.Format("{0}", sqlDataReader[0])));

                    string[] hourDivided = String.Format("{0}", sqlDataReader[3]).Split(':');
                    string hour = hourDivided[0] + ":" + hourDivided[1];

                    HoursListBox.Items.Add(String.Format("{0} \t sala {1} \t godzina {2}", sqlDataReader[1], sqlDataReader[2], hour));
                }
                sqlDataReader.Close();
            }

            sqlConnection.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void HoursListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.Format("{0}", HoursListBox.SelectedItem).Equals(""))
            {
                ChangePage(new MovieSeatsPage(window, this, sqlConnection, screeningId[HoursListBox.SelectedIndex]));
            }
        }
    }
}
