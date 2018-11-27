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
        public class ScreeningData
        {
            public readonly String Data;

            public ScreeningData(String data)
            {
                Data = data;
            }
        };

        public class ScreeningTime
        {
            public readonly string Hour, Minutes;

            public ScreeningTime(string hour, string minutes)
            {
                Hour = GetHourSpoken(hour);
                Minutes = GetMinutesSpoken(minutes);
            }

            private string GetHourSpoken(string hour)
            {
                switch (hour)
                {
                    case "10":
                        return "dziesiąta";
                    case "12":
                        return "dwunasta";
                    case "14":
                        return "czternasta";
                    case "16":
                        return "szesnasta";
                    case "18":
                        return "osiemnasta";
                    case "20":
                        return "dwudziesta";
                    case "22":
                        return "dwudziesta druga";
                }

                return null;
            }

            private string GetMinutesSpoken(string minutes)
            {
                switch (minutes)
                {
                    case "00":
                        return "";
                    case "15":
                        return "piętnaście";
                    case "30":
                        return "trzydzieści";
                    case "45":
                        return "czterdzieści pięć";
                }

                return null;
            }
        };

        private ScreeningData[] ScreeningsData;
        private ScreeningTime[] ScreeningsTime;

        private String movieTitle;

        private List<int> screeningId;

        public MovieHoursPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, String movieTitle, Window ticketWindow) : base(window, previousPage, sqlConnectionFactory, ticketWindow)
        {
            this.movieTitle = movieTitle;

            InitializeComponent();

            ListHours();
        }

        

        public ScreeningTime[] GetScreeningsTime()
        {
            if (ScreeningsData == null)
            {
                screeningId = new List<int>();
                List<ScreeningData> screeningsData = new List<ScreeningData>();
                List<ScreeningTime> screeningsTime = new List<ScreeningTime>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "select distinct Screenings.id, Movies.title, Screenings.auditoriumID, Screenings.screeningTime " +
                            "from Movies, Screenings " +
                            "where Movies.id = Screenings.movieID " +
                            "and Movies.title = '" + movieTitle +
                            "' and Screenings.screeningDate = CONVERT(date,  GETDATE())";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            screeningId.Add(int.Parse(String.Format("{0}", sqlDataReader[0])));

                            string[] hourDivided = String.Format("{0}", sqlDataReader[3]).Split(':');
                            string hour = hourDivided[0] + ":" + hourDivided[1];

                            string data = String.Format("{0} \t sala {1} \t godzina {2}", sqlDataReader[1], sqlDataReader[2], hour);

                            screeningsData.Add(new ScreeningData(data));
                            screeningsTime.Add(new ScreeningTime(hourDivided[0], hourDivided[1]));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                ScreeningsData = screeningsData.ToArray();
                ScreeningsTime = screeningsTime.ToArray();
            }

            return ScreeningsTime;
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection rules)
        {
            SrgsRule movieSrgsRule;

            {
                SrgsOneOf movieSrgsOneOfHours = new SrgsOneOf();

                int i = 0;
                foreach (ScreeningTime time in GetScreeningsTime())
                {
                    SrgsItem srgsItem = new SrgsItem(time.Hour);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"time." + i++ + "\";"));

                    movieSrgsOneOfHours.Add(srgsItem);
                }

                SrgsOneOf movieSrgsOneOfMinutes = new SrgsOneOf();

                i = 0;
                foreach (ScreeningTime time in GetScreeningsTime())
                {
                    SrgsItem srgsItem = new SrgsItem(time.Minutes);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"time." + i++ + "\";"));

                    movieSrgsOneOfMinutes.Add(srgsItem);
                }

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(movieSrgsOneOfHours);
                phraseSrgsItem.Add(movieSrgsOneOfMinutes);

                movieSrgsRule = new SrgsRule("time", phraseSrgsItem);
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
                    switch (command.First()) // TODO show info about movie
                    {
                        case "back":
                            MoveBack();
                            break;
                        case "time":
                            ChangePage(new MovieSeatsPage(window, this, sqlConnectionFactory, screeningId[int.Parse(command[1])], ticketWindow));
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

        private void ListHours()
        {
            GetScreeningsTime();
            foreach(ScreeningData data in ScreeningsData)
            {
                HoursListBox.Items.Add(data.Data);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void HoursListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.Format("{0}", HoursListBox.SelectedItem).Equals(""))
            {
                ChangePage(new MovieSeatsPage(window, this, sqlConnectionFactory, screeningId[HoursListBox.SelectedIndex], ticketWindow));
            }
        }
    }
}
