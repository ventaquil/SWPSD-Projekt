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
    /// Interaction logic for SeatButton.xaml
    /// </summary>
    public partial class SeatButton : UserControl
    {
        private int screeningId, rowNo, seatNo;
        private bool taken;
        private Window window;
        private Page lastPage;
        private SqlConnection dbConnection;

        public SeatButton(int screeningId, int rowNo, int seatNo, Window window, Page lastPage, SqlConnection dbConnection)
        {
            this.screeningId = screeningId;
            this.rowNo = rowNo;
            this.seatNo = seatNo;
            this.window = window;
            this.lastPage = lastPage;
            this.dbConnection = dbConnection;

            InitializeComponent();

            CheckIfTaken();
            InitSeatButton();
        }

        private void CheckIfTaken()
        {
            dbConnection.Open();
            SqlCommand command = new SqlCommand(
                "select count(Tickets.id) " +
                "from Tickets, Screenings, Seats " +
                "where Tickets.seatID = Seats.id and " +
                "Tickets.screeningID = Screenings.id and " +
                "Screenings.id = " + screeningId +
                "and Seats.rowNo = " + rowNo +
                "and Seats.seatNo = " + seatNo,
                dbConnection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            if (int.Parse(String.Format("{0}", reader[0])) > 0) taken = true;
            else taken = false;
            dbConnection.Close();
        }

        private void InitSeatButton()
        {
            Seat.Content = "Rząd: " + rowNo + "\nMiejsce: " + seatNo;
            if (taken)
                Seat.Background = Brushes.Red;
            else
                Seat.Background = Brushes.Green;
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            if (taken)
                MessageBox.Show("To miejsce jest już zajęte!");
            else
            {
                window.Content = new TicketDataPage(screeningId, rowNo, seatNo, window, lastPage, dbConnection);
            }
        }
    }
}
