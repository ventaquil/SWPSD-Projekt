using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
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
        public class Genre
        {
            public readonly String Name;

            public Genre(String name)
            {
                Name = name;
            }
        };

        private static string[] categories =
        {
            "Wszystkie",
            "Gatunek",
            "Najpopularniejsze"
        };

        private Genre[] Genres;

        public SearchPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Window ticketWindow) : base(window, previousPage, sqlConnectionFactory, ticketWindow)
        {
            InitializeComponent();

            InitComboBoxes();
        }

        public Genre[] GetGenres()
        {
            if (Genres == null)
            {
                List<Genre> genres = new List<Genre>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "select genre from Genres";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            string name = string.Format("{0}", sqlDataReader[0]);

                            genres.Add(new Genre(name));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                Genres = genres.ToArray();
            }

            return Genres;
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection rules)
        {
            SrgsRule genreSrgsRule;

            {
                SrgsOneOf genreSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (Genre genre in GetGenres())
                {
                    SrgsItem srgsItem = new SrgsItem(genre.Name);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"genre." + i++ + "\";"));

                    genreSrgsOneOf.Add(srgsItem);
                }

                SrgsItem genreSrgsItem = new SrgsItem("Wybierz");
                genreSrgsItem.Add(new SrgsItem(0, 1, "gatunek"));

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(genreSrgsItem);
                phraseSrgsItem.Add(genreSrgsOneOf);

                genreSrgsRule = new SrgsRule("genre", phraseSrgsItem);
            }

            rules.Add(genreSrgsRule);

            {
                SrgsItem srgsItem = new SrgsItem();
                srgsItem.Add(new SrgsRuleRef(genreSrgsRule));

                SrgsRule rootSrgsRule = rules.Where(rule => rule.Id == "root").First();
                SrgsOneOf srgsOneOf = (SrgsOneOf)rootSrgsRule.Elements.Where(element => element is SrgsOneOf).First();
                srgsOneOf.Add(srgsItem);
            }
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
                string[] command = result.Semantics.Value.ToString().ToLower().Split('.');
                DispatchAsync(() =>
                {
                    switch (command.First()) // TODO show info about movie
                    {
                        case "back":
                            MoveBack();
                            break;
                        case "genre":
                            CategoryComboBox.SelectedIndex = 1;
                            GenreComboBox.SelectedIndex = int.Parse(command.Skip(1).First());
                            GenreComboBox.IsDropDownOpen = false;
                            Search();
                            ResultsListBox.Focus();
                            break;
                        case "genres":
                            CategoryComboBox.SelectedIndex = 1;
                            GenreComboBox.Focus();
                            GenreComboBox.IsDropDownOpen = true;
                            break;
                        case "help":
                            SpeakHelp();
                            break;
                        case "search":
                            switch (command.Skip(1).First())
                            {
                                case "all":
                                    CategoryComboBox.SelectedIndex = 0;
                                    break;
                                case "popular":
                                    CategoryComboBox.SelectedIndex = 2;
                                    break;
                            }
                            Search();
                            break;
                        case "quit":
                            SpeakQuit();
                            Close();
                            break;
                    }
                });
            }
        }

        private void InitComboBoxes()
        {
            foreach (string category in categories)
            {
                CategoryComboBox.Items.Add(category);
            }

            foreach (Genre genre in GetGenres())
            {
                GenreComboBox.Items.Add(genre.Name);
            }

            CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem != null)
            {
                GenreComboBox.Visibility = CategoryComboBox.SelectedItem.Equals("Gatunek") ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            ResultsListBox.Items.Clear();

            if (CategoryComboBox.SelectedItem != null)
            {
                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
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
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void ResultsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (String.Format("{0}", ResultsListBox.SelectedItem).Length > 0)
            {
                new DescriptionWindow(window, this, sqlConnectionFactory, String.Format("{0}", ResultsListBox.SelectedItem), ticketWindow).Show();
            }
        }
    }
}
