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
        private Window window;
        private SqlConnection dbConnection;
        private Page lastPage;
        private string movieTitle;
        private List<int> screeningId;

        public MovieHoursPage(Window window, SqlConnection dbConnection, Page lastPage, string movieTitle)
        {
            this.window = window;
            this.dbConnection = dbConnection;
            this.lastPage = lastPage;
            this.movieTitle = movieTitle;

            InitializeComponent();
            ListHours();
        }

        private void ListHours()
        {
            screeningId = new List<int>();
            dbConnection.Open();
            SqlCommand command = new SqlCommand(
                "select distinct Screenings.id, Movies.title, Screenings.auditoriumID, Screenings.screeningTime " +
                "from Movies, Screenings " +
                "where Movies.id = Screenings.movieID " +
                "and Movies.title = '" + movieTitle + 
                "' and Screenings.screeningDate = CONVERT(date,  GETDATE())", 
                dbConnection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                screeningId.Add(int.Parse(String.Format("{0}",reader[0])));
                string[] hourDivided = String.Format("{0}", reader[3]).Split(':');
                string hour = hourDivided[0] + ":" + hourDivided[1];
                HoursListBox.Items.Add(String.Format("{0} \t sala {1} \t godzina {2}", reader[1], reader[2], hour));
            }
            dbConnection.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = lastPage;
        }

        private void HoursListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.Format("{0}", HoursListBox.SelectedItem).Equals(""))
            {
                window.Content = new MovieSeatsPage(window, dbConnection, this, screeningId[HoursListBox.SelectedIndex]);
            }
        }
    }
}
