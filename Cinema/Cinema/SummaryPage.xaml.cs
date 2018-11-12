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
        private Window window;
        private Page lastPage;
        private SqlConnection dbConnection;

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

        public SummaryPage(int screeningId, int seatId, int priceId, float price, string bookerName, Window window, Page lastPage, SqlConnection dbConnection)
        {
            this.screeningId = screeningId;
            this.seatId = seatId;
            this.priceId = priceId;
            this.price = price;
            this.bookerName = bookerName;
            this.window = window;
            this.lastPage = lastPage;
            this.dbConnection = dbConnection;

            InitializeComponent();
            ShowOrderData();
        }

        private void ShowOrderData()
        {
            dbConnection.Open();

            SqlCommand titleCommand = new SqlCommand(
                "select Movies.title " +
                "from Movies, Screenings " +
                "where Screenings.movieID = Movies.id and Screenings.id = " + screeningId, 
                dbConnection);
            SqlDataReader titleReader = titleCommand.ExecuteReader();
            titleReader.Read();
            OrderDataComboBox.Items.Add(dataTags[0] + String.Format("{0}", titleReader[0]));
            titleReader.Close();

            SqlCommand screeningCommand = new SqlCommand(
                "select Screenings.screeningDate, Screenings.screeningTime, Screenings.auditoriumID " +
                "from Screenings " +
                "where Screenings.id = " + screeningId, 
                dbConnection);
            SqlDataReader screeningReader = screeningCommand.ExecuteReader();
            screeningReader.Read();

            string date = String.Format("{0}", screeningReader[0]).Split(' ')[0];

            string[] hourDivided = String.Format("{0}", screeningReader[1]).Split(':');
            string hour = hourDivided[0] + ":" + hourDivided[1];

            OrderDataComboBox.Items.Add(dataTags[1] + String.Format("{0}", date));
            OrderDataComboBox.Items.Add(dataTags[2] + String.Format("{0}", hour));
            OrderDataComboBox.Items.Add(dataTags[3] + String.Format("{0}", screeningReader[2]));
            screeningReader.Close();

            SqlCommand seatsCommand = new SqlCommand(
                "select Seats.rowNo, Seats.seatNo " +
                "from Seats " +
                "where Seats.id = " + seatId,
                dbConnection);
            SqlDataReader seatsReader = seatsCommand.ExecuteReader();
            seatsReader.Read();
            OrderDataComboBox.Items.Add(dataTags[4] + " rząd " + String.Format("{0}", seatsReader[0]) + ", miejsce " + String.Format("{0}", seatsReader[1]));

            OrderDataComboBox.Items.Add(dataTags[5] + bookerName);
            OrderDataComboBox.Items.Add(dataTags[6] + price + " zł");

            dbConnection.Close();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            dbConnection.Open();

            SqlCommand command = new SqlCommand(
                "insert into Tickets " +
                "(seatID, screeningID, priceID, bookerName) " +
                "values (" +
                seatId + "," + screeningId + "," + priceId + ",'" + bookerName + "')",
                dbConnection);

            command.ExecuteNonQuery();

            dbConnection.Close();

            MessageBox.Show("Bilet został zamówiony!");

            window.Content = new MainPage(window, dbConnection);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = lastPage;
        }
    }
}
