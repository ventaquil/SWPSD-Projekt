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
        private string BookerName;

        private Price Price;

        private Seat Seat;

        public SummaryPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Seat seat, Price price, string bookerName) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();
            //Loaded += (sender, args) => speechControl.SetParent(this);

            Seat = seat;
            Price = price;
            BookerName = bookerName;

            ShowOrderData();
        }

        private void ShowOrderData()
        {
            Screening screening = Seat.Screening;
            Movie movie = screening.Movie;
        
            OrderDataItemsControl.Items.Add(string.Format("Film: {0}", movie.Title));
            OrderDataItemsControl.Items.Add(string.Format("Data: {0}", screening.Date));
            OrderDataItemsControl.Items.Add(string.Format("Godzina: {0}", screening.Time));
            OrderDataItemsControl.Items.Add(string.Format("Sala: {0}", screening.Auditorium));
            OrderDataItemsControl.Items.Add(string.Format("Miejsce: rząd {0}, miejsce {1}", Seat.Row, Seat.No));
            OrderDataItemsControl.Items.Add(string.Format("Zamawiający: {0}", BookerName));
            OrderDataItemsControl.Items.Add(string.Format("Cena: {0} zł", Price.Value));
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "INSERT INTO Tickets(seatID, screeningID, priceID, bookerName) " +
                        "VALUES (" + Seat.Id + "," + Seat.Screening.Id + "," + Price.Id + ",'" + BookerName + "')";

                    sqlCommand.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }

            MessageBox.Show("Bilet został zamówiony!");

            ChangePage(((MainWindow)window).MainPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }
    }
}
