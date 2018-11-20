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
    /// Interaction logic for MovieSeatsPage.xaml
    /// </summary>
    public partial class MovieSeatsPage : Page
    {
        private int screeningId;

        public MovieSeatsPage(Window window, Page previousPage, SqlConnection sqlConnection, int screeningId) : base(window, previousPage, sqlConnection)
        {
            this.screeningId = screeningId;

            InitializeComponent();
            
            InitSeatsView();
        }

        private void InitSeatsView()
        {
            Dictionary<int, List<int>> auditoriumSeats = new Dictionary<int, List<int>>();

            sqlConnection.Open();

            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = "select Seats.rowNo, Seats.seatNo " +
                    "from Seats, Auditoriums, Screenings " +
                    "where Seats.auditoriumID = Auditoriums.id and " +
                    "Auditoriums.id = Screenings.auditoriumID and " +
                    "Screenings.id = " + screeningId;

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    int rowNo = int.Parse(String.Format("{0}", sqlDataReader[0]));
                    int seatNo = int.Parse(String.Format("{0}", sqlDataReader[1]));

                    try
                    {
                        auditoriumSeats[rowNo].Add(seatNo);
                    }
                    catch (KeyNotFoundException)
                    {
                        auditoriumSeats[rowNo] = null;
                        auditoriumSeats[rowNo] = new List<int> { seatNo };
                    }
                }
                sqlDataReader.Close();
            }

            sqlConnection.Close();

            List<RowDefinition> gridRows = new List<RowDefinition>();
            List<ColumnDefinition> gridColumns = new List<ColumnDefinition>();

            for (int i = 0; i < auditoriumSeats.Count; i++)
            {
                SeatsGrid.RowDefinitions.Add(new RowDefinition());
            }

            //Assuming, that every row's seats quantity is equal...
            for (int i = 0; i < auditoriumSeats[1].Count; i++)
            {
                SeatsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 1; row <= auditoriumSeats.Count; row++)
            {
                foreach (int seat in auditoriumSeats[row])
                {
                    SeatButton seatButton = new SeatButton(window, this, sqlConnection, screeningId, row, seat);
                    Grid.SetRow(seatButton, row-1);
                    Grid.SetColumn(seatButton, seat-1);
                    SeatsGrid.Children.Add(seatButton);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = previousPage;
        }
    }
}
