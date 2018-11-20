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
    /// Interaction logic for TicketDataPage.xaml
    /// </summary>
    public partial class TicketDataPage : Page
    {
        private List<float> prices;
        private int rowNo, screeningId, seatNo;

        public TicketDataPage(Window window, Page previousPage, SqlConnection sqlConnection, int screeningId, int rowNo, int seatNo) : base(window, previousPage, sqlConnection)
        {
            this.screeningId = screeningId;
            this.rowNo = rowNo;
            this.seatNo = seatNo;

            InitializeComponent();

            InitComboBox();
        }

        private void InitComboBox()
        {
            sqlConnection.Open();

            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = "select priceDescription, price from Prices";

                prices = new List<float>();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    PriceComboBox.Items.Add(String.Format("{0} ({1} zł)", sqlDataReader[0], sqlDataReader[1]));
                    prices.Add(float.Parse(String.Format("{0}", sqlDataReader[1])));
                }
                sqlDataReader.Close();
            }

            sqlConnection.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if ((PriceComboBox.SelectedIndex >= 0) && (NameTextBox.Text.Length > 0))
            {
                float price = prices[PriceComboBox.SelectedIndex];
                int seatId = GetSeatId();
                string bookerName = String.Format("{0}", NameTextBox.Text);

                ChangePage(new SummaryPage(window, this, sqlConnection, screeningId, seatId, PriceComboBox.SelectedIndex + 1, price, bookerName));
            }
        }

        private int GetSeatId()
        {
            int seatId;

            sqlConnection.Open();

            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = "select Seats.id " +
                    "from Seats, Screenings, Auditoriums " +
                    "where Auditoriums.id = Seats.auditoriumID and " +
                    "Screenings.auditoriumID = Auditoriums.id and " +
                    "Seats.rowNo = " + rowNo +
                    " and Seats.seatNo = " + seatNo +
                    " and Screenings.id = " + screeningId;

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                sqlDataReader.Read();
                seatId = int.Parse(String.Format("{0}", sqlDataReader[0]));
                sqlDataReader.Close();
            }

            sqlConnection.Close();

            return seatId;
        }
    }
}
