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
    /// Interaction logic for MovieHoursPage.xaml
    /// </summary>
    public partial class MovieHoursPage : SpeechPage
    {
        private Movie Movie;

        private Screening[] Screenings;

        public MovieHoursPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Movie movie) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();
            Loaded += (sender, args) => SpeechControl.SetParent(this);

            Movie = movie;

            ListHours();
        }

        public Screening[] GetScreenings()
        {
            if (Screenings == null)
            {
                List<Screening> screenings = new List<Screening>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "SELECT DISTINCT Screenings.Id, Screenings.auditoriumID, Screenings.screeningDate, Screenings.screeningTime " +
                            "FROM Movies, Screenings " +
                            "WHERE (Movies.id = Screenings.movieID) AND " +
                                "(Movies.title = '" + Movie.Title + "') AND " +
                                "(Screenings.screeningDate = CONVERT(date,  GETDATE()))";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            int id = int.Parse(string.Format("{0}", sqlDataReader[0]));
                            int auditorium = int.Parse(string.Format("{0}", sqlDataReader[1]));
                            string date = string.Format("{0}", sqlDataReader[2]);
                            string time = string.Format("{0}", sqlDataReader[3]);

                            screenings.Add(new Screening(id, Movie, date, time, auditorium));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                Screenings = screenings.ToArray();
            }

            return Screenings;
        }

        protected override SpeechControl GetSpeechControl()
        {
            return SpeechControl;
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection rules)
        {
            SrgsRule movieSrgsRule;

            {
                SrgsOneOf srgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (Screening screening in GetScreenings())
                {
                    SrgsItem srgsItem = new SrgsItem();
                    srgsItem.Add(new SrgsItem(0, 1, "Sala " + screening.Auditorium));
                    srgsItem.Add(new SrgsItem(0, 1, new SrgsOneOf(
                        new SrgsItem(
                            new SrgsItem("na"),
                            new SrgsItem(0, 1, "godzinę")
                        ),
                        new SrgsItem(
                            new SrgsItem("o"),
                            new SrgsItem(0, 1, "godzinie")
                        ),
                        new SrgsItem("godzina")
                    )));
                    srgsItem.Add(new SrgsItem(screening.GetHour()));
                    if (screening.GetMinutes() == "00")
                    {
                        srgsItem.Add(new SrgsItem(0, 1, screening.GetMinutes()));
                    }
                    else
                    {
                        srgsItem.Add(new SrgsItem(screening.GetMinutes()));
                    }
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"time." + i++ + "\";"));

                    srgsOneOf.Add(srgsItem);
                }

                movieSrgsRule = new SrgsRule("time", srgsOneOf);
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
            Speak("Wybierz godzinę seansu.");
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
                    switch (command.First())
                    {
                        case "back":
                            MoveBack();
                            break;
                        case "time":
                            ChooseScreeningTime(int.Parse(command.Skip(1).First()));
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

        private void ChooseScreeningTime(int index)
        {
            try
            {
                ChooseScreeningTime(GetScreenings()[index]);
            }
            catch (IndexOutOfRangeException)
            {
            }
        }

        private void ChooseScreeningTime(Screening screening)
        {
            ChangePage(new MovieSeatsPage(window, this, sqlConnectionFactory, screening));
        }

        private void ListHours()
        {
            foreach (Screening screening in GetScreenings())
            {
                HoursListBox.Items.Add(string.Format("{0} \t sala {1} \t godzina {2}", screening.Movie.Title, screening.Auditorium, screening.Time));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void HoursListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChooseScreeningTime(HoursListBox.SelectedIndex);
        }
    }
}
