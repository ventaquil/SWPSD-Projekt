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
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        private static string[] categories =
        {
            "Wszystkie",
            "Gatunek",
            "Najpopularniejsze"
        };
        private Window window;
        private Page lastPage;
        private SqlConnection dbConnection;

        public SearchPage(Window window, Page lastPage, SqlConnection dbConnection)
        {
            this.window = window;
            this.lastPage = lastPage;
            this.dbConnection = dbConnection;
            InitializeComponent();

            InitComboBoxes();

            CategoryComboBox.DropDownClosed += CategoryComboBox_DropDownClosed;
        }

        private void InitComboBoxes()
        {
            foreach (string category in categories)
                CategoryComboBox.Items.Add(category);

            dbConnection.Open();
            SqlCommand command = new SqlCommand("select genre from Genres", dbConnection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                GenreComboBox.Items.Add(String.Format("{0}",reader[0]));
            dbConnection.Close();
        }

        private void CategoryComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (CategoryComboBox.SelectedItem != null)
            {
                if (CategoryComboBox.SelectedItem.Equals("Gatunek"))
                    GenreComboBox.Visibility = Visibility.Visible;
                else
                    GenreComboBox.Visibility = Visibility.Hidden;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsListBox.Items.Clear();
            if (CategoryComboBox.SelectedItem != null) {
                if (CategoryComboBox.SelectedItem.Equals("Wszystkie"))
                { 
                    dbConnection.Open();
                    SqlCommand command = new SqlCommand(
                        "select distinct Movies.title " +
                        "from Movies, Screenings " +
                        "where Movies.id = Screenings.movieID and " +
                        "Screenings.screeningDate = CONVERT(date,  GETDATE())", 
                        dbConnection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                        ResultsListBox.Items.Add(String.Format("{0}", reader[0]));
                    dbConnection.Close();
                }
                else if (CategoryComboBox.SelectedItem.Equals("Gatunek") && GenreComboBox.SelectedItem != null)
                {
                    dbConnection.Open();
                    SqlCommand command = new SqlCommand("execute procedure_GetMoviesByGenre " + String.Format("'{0}'", GenreComboBox.SelectedItem),dbConnection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                        ResultsListBox.Items.Add(String.Format("{0}", reader[0]));
                    dbConnection.Close();
                }
                else if (CategoryComboBox.SelectedItem.Equals("Najpopularniejsze"))
                {
                    dbConnection.Open();
                    SqlCommand command = new SqlCommand("execute procedure_MostPopularMovies", dbConnection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                        ResultsListBox.Items.Add(String.Format("{0}", reader[0]));
                    dbConnection.Close();
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            window.Content = lastPage;
        }

        private void ResultsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(!String.Format("{0}", ResultsListBox.SelectedItem).Equals(""))
                new DescriptionWindow(window, this, dbConnection, String.Format("{0}", ResultsListBox.SelectedItem)).Show();
        }
    }
}
