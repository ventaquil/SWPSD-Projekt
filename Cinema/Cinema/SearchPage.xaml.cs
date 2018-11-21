using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class SearchPage : SpeechPage
    {
        private static string[] categories =
        {
            "Wszystkie",
            "Gatunek",
            "Najpopularniejsze"
        };

        public SearchPage(Window window, Page previousPage, SqlConnection sqlConnection) : base(window, previousPage, sqlConnection)
        {
            InitializeComponent();

            InitComboBoxes();
        }

        public override void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            base.InitializeSpeech(sender, e);

            SpeakHello();
        }

        private void SpeakHello()
        {
            Speak("Witaj w wyszukiwarce.");
        }

        private void SpeakHelp()
        {
            Speak("Pomoc.");
        }

        private void SpeakRepeat()
        {
            Speak("Powtórz proszę.");
        }

        private void SpeakQuit()
        {
            Speak("Zapraszam ponownie.");
        }

        protected override void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            base.SpeechRecognitionEngine_SpeechRecognized(sender, e);

            RecognitionResult result = e.Result;

            if (result.Confidence < 0.6)
            {
                SpeakRepeat();
            }
            else
            {
                string command = result.Semantics.Value.ToString().ToLower();
                switch (command)
                {
                    case "back":
                        Dispatch(MoveBack);
                        break;
                    case "help":
                        SpeakHelp();
                        break;
                    case "quit":
                        SpeakQuit();
                        Dispatch(Close);
                        break;
                }
            }
        }

        private void InitComboBoxes()
        {
            foreach (string category in categories)
            {
                CategoryComboBox.Items.Add(category);
            }

            sqlConnection.Open();

            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandText = "select genre from Genres";

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    GenreComboBox.Items.Add(String.Format("{0}", sqlDataReader[0]));
                }
                sqlDataReader.Close();
            }

            sqlConnection.Close();

            CategoryComboBox.DropDownClosed += CategoryComboBox_DropDownClosed;
        }

        private void CategoryComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (CategoryComboBox.SelectedItem != null)
            {
                GenreComboBox.Visibility = CategoryComboBox.SelectedItem.Equals("Gatunek") ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsListBox.Items.Clear();

            if (CategoryComboBox.SelectedItem != null)
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    if (CategoryComboBox.SelectedItem.Equals("Wszystkie"))
                    {
                        sqlCommand.CommandText = "select distinct Movies.title " +
                            "from Movies, Screenings " +
                            "where Movies.id = Screenings.movieID and " +
                            "Screenings.screeningDate = CONVERT(date,  GETDATE())";
                    }
                    else if (CategoryComboBox.SelectedItem.Equals("Gatunek") && (GenreComboBox.SelectedItem != null))
                    {
                        sqlCommand.CommandText = "execute procedure_GetMoviesByGenre " + String.Format("'{0}'", GenreComboBox.SelectedItem);
                    }
                    else if (CategoryComboBox.SelectedItem.Equals("Najpopularniejsze"))
                    {
                        sqlCommand.CommandText = "execute procedure_MostPopularMovies";
                    }

                    if (sqlCommand.CommandText.Length > 0)
                    {
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            ResultsListBox.Items.Add(String.Format("{0}", sqlDataReader[0]));
                        }
                        sqlDataReader.Close();
                    }
                }

                sqlConnection.Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void ResultsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (String.Format("{0}", ResultsListBox.SelectedItem).Length > 0)
            {
                new DescriptionWindow(window, this, sqlConnection, String.Format("{0}", ResultsListBox.SelectedItem)).Show();
            }
        }
    }
}
