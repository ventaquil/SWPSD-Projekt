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
        private Price[] Prices;

        private Seat Seat;

        public TicketDataPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Seat seat) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();

            Seat = seat;

            InitializeComboBox();
        }

        private Price[] GetPrices()
        {
            if (Prices == null)
            {
                List<Price> prices = new List<Price>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "SELECT id, priceDescription, price FROM Prices";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            int id = int.Parse(string.Format("{0}", sqlDataReader[0]));
                            string description = string.Format("{0}", sqlDataReader[1]);
                            float price = float.Parse(string.Format("{0}", sqlDataReader[2]));

                            prices.Add(new Price(id, price, description));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                Prices = prices.ToArray();
            }

            return Prices;
        }

        private void InitializeComboBox()
        {
            foreach (Price price in GetPrices())
            {
                PriceComboBox.Items.Add(string.Format("{0} ({1} zł)", price.Description, price.Value));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if ((PriceComboBox.SelectedIndex >= 0) && (NameTextBox.Text.Length > 0))
            {
                Price price = GetPrices()[PriceComboBox.SelectedIndex];
                string bookerName = string.Format("{0}", NameTextBox.Text);

                ChangePage(new SummaryPage(window, this, sqlConnectionFactory, Seat, price, bookerName));
            }
        }
    }
}
