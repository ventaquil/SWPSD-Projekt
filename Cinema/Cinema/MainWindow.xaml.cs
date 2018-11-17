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
using System.IO;

namespace Cinema
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String databasePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "CinemaDatabase.mdf");
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Create database if not exists
            if (!System.IO.File.Exists(databasePath))
            {
                CreateSqlDatabase();

                ExecuteCreateQueries();
            }
            
            Content = new MainPage(this, CreateSqlConnection());

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public SqlConnection CreateSqlConnection()
        {
            return new SqlConnection
            {
                ConnectionString = "Data Source=(localDB)\\MSSQLLocalDB; AttachDbFilename=" + databasePath
            };
        }

        public void CreateSqlDatabase()
        {
            string databaseName = System.IO.Path.GetFileNameWithoutExtension(databasePath);

            using (SqlConnection connection = new SqlConnection("Data Source=(localDB)\\MSSQLLocalDB"))
            {
                connection.Open();
                using (SqlCommand sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = String.Format("CREATE DATABASE {0} ON PRIMARY (NAME={0}, FILENAME='{1}')", databaseName, databasePath);
                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.CommandText = String.Format("EXEC sp_detach_db '{0}', 'true'", databaseName);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void ExecuteCreateQueries()
        {

            using (SqlConnection sqlConnection = CreateSqlConnection())
            {
                try
                {
                    sqlConnection.Open();
                }
                catch (SqlException sqlException)
                {
                    MessageBox.Show(sqlException.Message.ToString(), "Error message");
                }

                String[] commands = new String[] {
                    Properties.Resources.CreateTables,
                    Properties.Resources.Inserts,
                    Properties.Resources.GetMoviesByGenre,
                    Properties.Resources.MostPopularMovies
                };

                foreach (String command in commands)
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
    }
}
