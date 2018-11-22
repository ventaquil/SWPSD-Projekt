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
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : SpeechPage
    {
        public class Movie
        {
            public readonly String Name;

            public Movie(String name)
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

        private Movie[] Movies;

        public OrderPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Window ticketWindow) : base(window, previousPage, sqlConnectionFactory, ticketWindow)
        {
            InitializeComponent();

            ListMovies();
        }

        public Movie[] GetMovies()
        {
            if (Movies == null)
            {
                List<Movie> movies = new List<Movie>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "select distinct Movies.title " +
                            "from Movies, Screenings " +
                            "where Movies.id = Screenings.movieID " +
                            "and Screenings.screeningDate = CONVERT(date,  GETDATE())";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            string name = string.Format("{0}", sqlDataReader[0]);

                            movies.Add(new Movie(name));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                Movies = movies.ToArray();
            }

            return Movies;
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection rules)
        {
            SrgsRule movieSrgsRule;

            {
                SrgsOneOf movieSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (Movie movie in GetMovies())
                {
                    SrgsItem srgsItem = new SrgsItem(movie.Name);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"movie." + i++ + "\";"));

                    movieSrgsOneOf.Add(srgsItem);
                }

                SrgsItem movieSrgsItem = new SrgsItem(0, 1, "Wybierz");
                movieSrgsItem.Add(new SrgsItem(0, 1, "film"));

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(movieSrgsItem);
                phraseSrgsItem.Add(movieSrgsOneOf);

                movieSrgsRule = new SrgsRule("movie", phraseSrgsItem);
            }

            rules.Add(movieSrgsRule);

            {
                SrgsItem srgsItem = new SrgsItem();
                srgsItem.Add(new SrgsRuleRef(movieSrgsRule));

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
            Speak("Wybierz film który Cię interesuje.");
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
                        case "movie":
                            ChangePage(new MovieHoursPage(window, this, sqlConnectionFactory, String.Format("{0}", MoviesListBox.Items.GetItemAt(int.Parse(command[1]))), ticketWindow));
                            break;
                        case "help":
                            SpeakHelp();
                            break;
                        case "quit":
                            SpeakQuit();
                            Close();
                            break;
                    }
                });
            }
        }

        private void ListMovies()
        {
            using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
            {
                sqlConnection.Open();

                foreach(Movie movie in GetMovies())
                {
                    MoviesListBox.Items.Add(movie.Name);
                }

                sqlConnection.Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void MoviesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.Format("{0}", MoviesListBox.SelectedItem).Equals(""))
            {
                ChangePage(new MovieHoursPage(window, this, sqlConnectionFactory, String.Format("{0}", MoviesListBox.SelectedItem), ticketWindow));
            }
        }
    }
}
