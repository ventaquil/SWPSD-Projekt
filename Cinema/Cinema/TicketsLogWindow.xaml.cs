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
using System.Windows.Shapes;

namespace Cinema
{
    /// <summary>
    /// Interaction logic for TicketsLogWindow.xaml
    /// </summary>
    public partial class TicketsLogWindow : Window
    {
        private SqlConnectionFactory sqlConnectionFactory;

        public TicketsLogWindow(SqlConnectionFactory sqlConnectionFactory)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            InitializeComponent();
            getTicketsFromDB();
            Show();
        }

        private void RefreshDataButton_Click(object sender, RoutedEventArgs e)
        {
            getTicketsFromDB();
        }

        private void getTicketsFromDB()
        {
            TicketsOrdersListBox.Items.Clear();
            using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "execute procedure_GetTicketsInfoList";

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        //Seats.rowNo, Seats.seatNo, Movies.title, Screenings.screeningDate, Screenings.screeningTime, Prices.priceDescription, Prices.price, Tickets.bookerName
                        TicketsOrdersListBox.Items.Add(
                            "Kupujący: " + String.Format("{0}", sqlDataReader[7]) +
                            ";\tFilm: " + String.Format("{0}", sqlDataReader[2]) +
                            ";\tData: " + String.Format("{0}", sqlDataReader[3]) + " " + String.Format("{0}", sqlDataReader[4]) +
                            ";\tMiejsce: " + String.Format("{0}", sqlDataReader[0]) + "/" + String.Format("{0}", sqlDataReader[1]) +
                            ";\tCena: " + String.Format("{0}", sqlDataReader[5]) + " (" + String.Format("{0}", sqlDataReader[6]) + "zł)");
                    }
                    sqlDataReader.Close();
                }

                sqlConnection.Close();
            }
        }
    }
}
