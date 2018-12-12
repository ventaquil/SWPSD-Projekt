using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
        private static string DatabasePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "CinemaDatabase.mdf");

        private static string ConnectionString = "Data Source=(localDB)\\MSSQLLocalDB; AttachDbFilename=" + DatabasePath;

        public readonly MainPage MainPage;

        private readonly Window TicketsWindow;

        public MainWindow()
        {
            InitializeComponent();

            // Create database if not exists
            if (!File.Exists(DatabasePath))
            {
                CreateSqlDatabase();

                ExecuteCreateQueries();
            }

            SqlConnectionFactory sqlConnectionFactory = CreateSqlConnectionFactory();

            TicketsWindow = new TicketsLogWindow(sqlConnectionFactory);
            Content = MainPage = new MainPage(this, sqlConnectionFactory);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public SqlConnectionFactory CreateSqlConnectionFactory()
        {
            return new SqlConnectionFactory(ConnectionString);
        }

        public void CreateSqlDatabase()
        {
            string databaseName = System.IO.Path.GetFileNameWithoutExtension(DatabasePath);

            using (SqlConnection sqlConnection = new SqlConnection("Data Source=(localDB)\\MSSQLLocalDB"))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = string.Format("CREATE DATABASE {0} ON PRIMARY (NAME={0}, FILENAME='{1}')", databaseName, DatabasePath);
                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.CommandText = string.Format("EXEC sp_detach_db '{0}', 'true'", databaseName);
                    sqlCommand.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }
        }

        public void ExecuteCreateQueries()
        {

            using (SqlConnection sqlConnection = CreateSqlConnectionFactory().Create())
            {
                try
                {
                    sqlConnection.Open();
                }
                catch (SqlException sqlException)
                {
                    MessageBox.Show(sqlException.Message.ToString(), "Error message");
                }

                string[] commands = new string[] {
                    Properties.Resources.CreateTables,
                    Properties.Resources.Inserts,
                    Properties.Resources.GetMoviesByGenre,
                    Properties.Resources.GetTicketsInfoList,
                    Properties.Resources.MostPopularMovies
                };

                foreach (string command in commands)
                {
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = command;

                        try
                        {
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch (SqlException sqlException)
                        {
                            MessageBox.Show(sqlException.Message.ToString(), "Error message");
                        }
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TicketsWindow.Close();
        }
    }
}
