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
        private int screeningId, rowNo, seatNo;
        private Window window;
        private Page lastPage;
        private SqlConnection dbConnection;
        private List<float> prices;

        public TicketDataPage(int screeningId, int rowNo, int seatNo, Window window, Page lastPage, SqlConnection dbConnection)
        {
            this.screeningId = screeningId;
            this.rowNo = rowNo;
            this.seatNo = seatNo;
            this.window = window;
            this.lastPage = lastPage;
            this.dbConnection = dbConnection;

            InitializeComponent();
            InitComboBox();
        }

        private void InitComboBox()
        {
            dbConnection.Open();
            SqlCommand command = new SqlCommand("select priceDescription, price from Prices", dbConnection);
            SqlDataReader reader = command.ExecuteReader();
            prices = new List<float>();
            while (reader.Read())
            {
                PriceComboBox.Items.Add(String.Format("{0} ({1} zł)", reader[0], reader[1]));
                prices.Add(float.Parse(String.Format("{0}", reader[1])));
            }
            dbConnection.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = lastPage;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (PriceComboBox.SelectedIndex >= 0 && NameTextBox.Text != "")
            {
                float price = prices[PriceComboBox.SelectedIndex];
                int seatId = GetSeatId();
                string bookerName = String.Format("{0}", NameTextBox.Text);

                window.Content = new SummaryPage(screeningId, seatId, PriceComboBox.SelectedIndex+1, price, bookerName, window, this, dbConnection);
            }
        }

        private int GetSeatId()
        {
            dbConnection.Open();
            SqlCommand command = new SqlCommand(
                "select Seats.id " +
                "from Seats, Screenings, Auditoriums " +
                "where Auditoriums.id = Seats.auditoriumID and " +
                "Screenings.auditoriumID = Auditoriums.id and " +
                "Seats.rowNo = " + rowNo +
                " and Seats.seatNo = " + seatNo +
                " and Screenings.id = " + screeningId, 
                dbConnection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int seatId = int.Parse(String.Format("{0}", reader[0]));
            dbConnection.Close();
            return seatId;
        }
    }
}
