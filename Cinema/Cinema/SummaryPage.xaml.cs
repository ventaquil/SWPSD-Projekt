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
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SummaryPage : Page
    {
        private int screeningId, seatId, priceId;
        private float price;
        private string bookerName;

        private string[] dataTags =
        {
            "Film: ",
            "Data: ",
            "Godzina: ",
            "Sala: ",
            "Miejsce: ",
            "Zamawiający: ",
            "Cena: "
        };

        public SummaryPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, int screeningId, int seatId, int priceId, float price, string bookerName, Window ticketWindow) : base(window, previousPage, sqlConnectionFactory, ticketWindow)
        {
            this.screeningId = screeningId;
            this.seatId = seatId;
            this.priceId = priceId;
            this.price = price;
            this.bookerName = bookerName;

            InitializeComponent();

            ShowOrderData();
        }

        private void ShowOrderData()
        {using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select Movies.title " +
                        "from Movies, Screenings " +
                        "where Screenings.movieID = Movies.id and Screenings.id = " + screeningId;

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Read();
                    OrderDataComboBox.Items.Add(dataTags[0] + String.Format("{0}", sqlDataReader[0]));
                    sqlDataReader.Close();
                }

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select Screenings.screeningDate, Screenings.screeningTime, Screenings.auditoriumID " +
                        "from Screenings " +
                        "where Screenings.id = " + screeningId;

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Read();

                    string date = String.Format("{0}", sqlDataReader[0]).Split(' ')[0];

                    string[] hourDivided = String.Format("{0}", sqlDataReader[1]).Split(':');
                    string hour = hourDivided[0] + ":" + hourDivided[1];

                    OrderDataComboBox.Items.Add(dataTags[1] + String.Format("{0}", date));
                    OrderDataComboBox.Items.Add(dataTags[2] + String.Format("{0}", hour));
                    OrderDataComboBox.Items.Add(dataTags[3] + String.Format("{0}", sqlDataReader[2]));

                    sqlDataReader.Close();
                }

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select Seats.rowNo, Seats.seatNo " +
                        "from Seats " +
                        "where Seats.id = " + seatId;

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Read();

                    OrderDataComboBox.Items.Add(dataTags[4] + " rząd " + String.Format("{0}", sqlDataReader[0]) + ", miejsce " + String.Format("{0}", sqlDataReader[1]));
                    OrderDataComboBox.Items.Add(dataTags[5] + bookerName);
                    OrderDataComboBox.Items.Add(dataTags[6] + price + " zł");

                    sqlDataReader.Close();
                }

                sqlConnection.Close();
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "insert into Tickets " +
                        "(seatID, screeningID, priceID, bookerName) " +
                        "values (" + seatId + "," + screeningId + "," + priceId + ",'" + bookerName + "')";

                    sqlCommand.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }

            MessageBox.Show("Bilet został zamówiony!");

            ChangePage(new MainPage(window, sqlConnectionFactory, ticketWindow));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }
    }
}
