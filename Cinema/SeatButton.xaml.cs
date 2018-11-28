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
        private Page lastPage;
        private int rowNo, screeningId, seatNo;
        private SqlConnectionFactory sqlConnectionFactory;
        private bool taken;
        private Window window;

        public SeatButton(Window window, Page lastPage, SqlConnectionFactory sqlConnectionFactory, int screeningId, int rowNo, int seatNo)
        {
            this.window = window;
            this.lastPage = lastPage;
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.screeningId = screeningId;
            this.rowNo = rowNo;
            this.seatNo = seatNo;

            InitializeComponent();

            CheckIfTaken();

            InitSeatButton();
        }

        private void CheckIfTaken()
        {
            using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select count(Tickets.id) " +
                        "from Tickets, Screenings, Seats " +
                        "where Tickets.seatID = Seats.id and " +
                        "Tickets.screeningID = Screenings.id and " +
                        "Screenings.id = " + screeningId +
                        "and Seats.rowNo = " + rowNo +
                        "and Seats.seatNo = " + seatNo;

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Read();

                    taken = (int.Parse(String.Format("{0}", sqlDataReader[0])) > 0);

                    sqlDataReader.Close();
                }

                sqlConnection.Close();
            }
        }

        private void InitSeatButton()
        {
            Seat.Content = "Rząd: " + rowNo + "\nMiejsce: " + seatNo;

            Seat.Background = (taken) ? Brushes.Red : Brushes.Green;
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            PerformAction();
        }

        public void PerformAction()
        {
            if (taken)
            {
                MessageBox.Show("To miejsce jest już zajęte!");
            }
            else
            {
                window.Content = new TicketDataPage(window, lastPage, sqlConnectionFactory, screeningId, rowNo, seatNo);
            }
        }

        public int GetRowNo()
        {
            return this.rowNo;
        }

        public int GetSeatNo()
        {
            return this.seatNo;
        }
    }
}
