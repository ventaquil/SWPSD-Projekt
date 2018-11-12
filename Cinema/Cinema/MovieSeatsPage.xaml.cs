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
        private Window window;
        private SqlConnection dbConnection;
        private Page lastPage;
        private int screeningId;

        public MovieSeatsPage(Window window, SqlConnection dbConnection, Page lastPage, int screeningId)
        {
            this.window = window;
            this.dbConnection = dbConnection;
            this.lastPage = lastPage;
            this.screeningId = screeningId;

            InitializeComponent();
            InitSeatsView();
        }

        private void InitSeatsView()
        {
            Dictionary<int, List<int>> auditoriumSeats = new Dictionary<int, List<int>>();

            dbConnection.Open();
            SqlCommand command = new SqlCommand(
                "select Seats.rowNo, Seats.seatNo " +
                "from Seats, Auditoriums, Screenings " +
                "where Seats.auditoriumID = Auditoriums.id and " +
                "Auditoriums.id = Screenings.auditoriumID and " +
                "Screenings.id = " + screeningId, 
                dbConnection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int rowNo = int.Parse(String.Format("{0}", reader[0]));
                int seatNo = int.Parse(String.Format("{0}", reader[1]));

                try
                {
                    auditoriumSeats[rowNo].Add(seatNo);
                }
                catch (KeyNotFoundException e)
                {
                    auditoriumSeats[rowNo] = null;
                    auditoriumSeats[rowNo] = new List<int> { seatNo };
                }
            }
            dbConnection.Close();

            List<RowDefinition> gridRows = new List<RowDefinition>();
            List<ColumnDefinition> gridColumns = new List<ColumnDefinition>();

            for(int i = 0; i < auditoriumSeats.Count; i++)
                SeatsGrid.RowDefinitions.Add(new RowDefinition());

            //Assuming, that every row's seats quantity is equal...
            for (int i = 0; i < auditoriumSeats[1].Count; i++)
                SeatsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int row = 1; row <= auditoriumSeats.Count; row++)
            {
                foreach (int seat in auditoriumSeats[row])
                {
                    SeatButton seatButton = new SeatButton(screeningId, row, seat, window, this, dbConnection);
                    Grid.SetRow(seatButton, row-1);
                    Grid.SetColumn(seatButton, seat-1);
                    SeatsGrid.Children.Add(seatButton);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = lastPage;
        }
    }
}
