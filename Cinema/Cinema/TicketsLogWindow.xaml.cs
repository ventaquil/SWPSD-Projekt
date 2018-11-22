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
                    sqlCommand.CommandText = "select * from Tickets";

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        TicketsOrdersListBox.Items.Add(String.Format("{0}", sqlDataReader[4]));
                    }
                    sqlDataReader.Close();
                }

                sqlConnection.Close();
            }
        }
    }
}
