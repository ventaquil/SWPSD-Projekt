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
            public readonly string Name;

            public Genre(string name)
            {
                Name = name;
            }
        };

        public class Movie
        {
            public readonly string Description;

            public readonly string Name;

            public Movie(string name)
            {
                Name = name;
            }

            public Movie(string name, string description) : this(name)
            {
                Description = description;
            }
        }

        private static string[] Categories =
        {
            "Wszystkie",
            "Gatunek",
            "Najpopularniejsze"
        };

        private Genre[] Genres;

        private Movie[] Movies;

        private string MovieLatestQuery;

        public SearchPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();

            InitializeComboBoxes();
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            AddGenresSpeechGrammarRulers(srgsRules);

            AddMoviesSpeechGrammarRules(srgsRules);
        }

        private void AddGenresSpeechGrammarRulers(SrgsRulesCollection srgsRules)
        {
            SrgsRule genreSrgsRule;

            {
                SrgsOneOf genreSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (Genre genre in GetGenres())
                {
                    SrgsItem srgsItem = new SrgsItem(genre.Name);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"search.genres." + i++ + "\";"));

                    genreSrgsOneOf.Add(srgsItem);
                }

                SrgsItem genreSrgsItem = new SrgsItem("Wybierz");
                genreSrgsItem.Add(new SrgsItem(0, 1, "gatunek"));

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(genreSrgsItem);
                phraseSrgsItem.Add(genreSrgsOneOf);

                genreSrgsRule = new SrgsRule("genre", phraseSrgsItem);
            }

            srgsRules.Add(genreSrgsRule);

            {
                SrgsItem srgsItem = new SrgsItem();
                srgsItem.Add(new SrgsRuleRef(genreSrgsRule));

                SrgsRule rootSrgsRule = srgsRules.Where(rule => rule.Id == "root").First();
                SrgsOneOf srgsOneOf = (SrgsOneOf)rootSrgsRule.Elements.Where(element => element is SrgsOneOf).First();
                srgsOneOf.Add(srgsItem);
            }
        }

        private void AddMoviesSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            Movie[] movies = null;

            DispatchSync(() =>
            {
                movies = GetMovies();
            });

            if ((movies != null) && (movies.Length > 0))
            {
                SrgsRule movieSrgsRule;

                {
                    SrgsOneOf movieSrgsOneOf = new SrgsOneOf();

                    int i = 0;
                    foreach (Movie movie in movies)
                    {
                        SrgsItem srgsItem = new SrgsItem(movie.Name);
                        srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"movies." + i++ + "\";"));

                        movieSrgsOneOf.Add(srgsItem);
                    }

                    SrgsItem movieSrgsItem = new SrgsItem();
                    SrgsOneOf srgsOneOf = new SrgsOneOf();
                    srgsOneOf.Add(new SrgsItem("Wyświetl"));
                    srgsOneOf.Add(new SrgsItem("Pokaż"));
                    srgsOneOf.Add(new SrgsItem("Wybierz"));
                    movieSrgsItem.Add(srgsOneOf);
                    movieSrgsItem.Add(new SrgsItem(0, 1, "film"));

                    SrgsItem phraseSrgsItem = new SrgsItem();
                    phraseSrgsItem.Add(movieSrgsItem);
                    phraseSrgsItem.Add(movieSrgsOneOf);

                    movieSrgsRule = new SrgsRule("movie", phraseSrgsItem);
                }

                srgsRules.Add(movieSrgsRule);

                {
                    SrgsItem srgsItem = new SrgsItem();
                    srgsItem.Add(new SrgsRuleRef(movieSrgsRule));

                    SrgsRule rootSrgsRule = srgsRules.Where(rule => rule.Id == "root").First();
                    SrgsOneOf srgsOneOf = (SrgsOneOf)rootSrgsRule.Elements.Where(element => element is SrgsOneOf).First();
                    srgsOneOf.Add(srgsItem);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem != null)
            {
                GenreComboBox.Visibility = (CategoryComboBox.SelectedIndex == 1) ? Visibility.Visible : Visibility.Hidden;
            }
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
                        sqlCommand.CommandText = "SELECT genre FROM Genres";

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

        public Movie[] GetMovies()
        {
            string query = null;

            switch (CategoryComboBox.SelectedIndex)
            {
                case 0:
                    query = "SELECT DISTINCT Movies.title, CONVERT(VARCHAR(MAX), Movies.description) AS description " +
                        "FROM Movies, Screenings " +
                        "WHERE (Movies.id = Screenings.movieID) AND (Screenings.screeningDate = CONVERT(date, GETDATE()))";
                    break;
                case 1:
                    if (GenreComboBox.SelectedItem != null)
                    {
                        query = "EXECUTE procedure_GetMoviesByGenre " + string.Format("'{0}'", GenreComboBox.SelectedItem);
                    }
                    break;
                case 2:
                    query = "EXECUTE procedure_MostPopularMovies";
                    break;
            }

            if (query == null)
            {
                Movies = null;
            }
            else if ((Movies == null) || (query != MovieLatestQuery)) // query != null
            {
                List<Movie> movies = new List<Movie>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = query;

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            string name = string.Format("{0}", sqlDataReader[0]);
                            string description = string.Format("{0}", sqlDataReader[1]);
                            movies.Add(new Movie(name, description));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                Movies = movies.ToArray();
            }

            MovieLatestQuery = query;

            return Movies;
        }

        private void InitializeComboBoxes()
        {
            foreach (string category in Categories)
            {
                CategoryComboBox.Items.Add(category);
            }

            foreach (Genre genre in GetGenres())
            {
                GenreComboBox.Items.Add(genre.Name);
            }

            CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
        }

        public override void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            base.InitializeSpeech(sender, e);

            SpeakHello();
        }

        private void ResultsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowDescription(ResultsListBox.SelectedIndex);
        }

        private void ShowDescription(int movieIndex)
        {
            try
            {
                Movie movie = GetMovies()[movieIndex];
                DescriptionWindow descriptionWindow = new DescriptionWindow(window, this, sqlConnectionFactory, movie.Name, movie.Description);
                descriptionWindow.Show();
                descriptionWindow.Focus();
            }
            catch (IndexOutOfRangeException)
            {
            }
        }

        private void Search()
        {
            ResultsListBox.Items.Clear();

            if (GetMovies()?.Length > 0)
            {
                foreach (Movie movie in GetMovies())
                {
                    ResultsListBox.Items.Add(movie.Name);
                }
            }

            ReloadGrammars();

            if (!ResultsListBox.Items.IsEmpty)
            {
                ResultsListBox.Focus();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
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
                        case "help":
                            SpeakHelp();
                            break;
                        case "movies":
                            ShowDescription(int.Parse(command.Skip(1).First()));
                            break;
                        case "search":
                            switch (command.Skip(1).First())
                            {
                                case "all":
                                    CategoryComboBox.SelectedIndex = 0;
                                    break;
                                case "genres":
                                    CategoryComboBox.SelectedIndex = 1;
                                    switch (command.Length)
                                    {
                                        case 2:
                                            GenreComboBox.Focus();
                                            GenreComboBox.IsDropDownOpen = true;
                                            break;
                                        default:
                                            GenreComboBox.SelectedIndex = int.Parse(command.Skip(2).First());
                                            GenreComboBox.IsDropDownOpen = false;
                                            break;
                                    }
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
    }
}
