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
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dbPath = "C:\\Users\\Daniel\\Desktop\\Git\\SWPSD-Projekt\\Cinema\\Cinema\\";
        private SqlConnection dbConnection;

        public MainWindow()
        {
            InitializeComponent();

            dbConnection = new SqlConnection
            {
                ConnectionString = "Data Source = (localDB)\\MSSQLLocalDB; AttachDbFilename = " + dbPath + "CinemaDatabase.mdf"
            };

            this.Content = new MainPage(this, dbConnection);
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
    }
}
