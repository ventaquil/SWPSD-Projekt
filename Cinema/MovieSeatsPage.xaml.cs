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
    /// Interaction logic for MovieSeatsPage.xaml
    /// </summary>
    public partial class MovieSeatsPage : SpeechPage
    {
        private Screening Screening;

        public MovieSeatsPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Screening screening) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();

            Screening = screening;

            InitializeSeatsView();
        }

        public class Seat
        {
            public static int Rows = 0, Cols = 0;
            public SeatButton Button;
            public readonly string SeatPositionTag;

            public Seat(SeatButton button)
            {
                Button = button;
                //SeatPositionTag = "Rząd " + GetRowNoSpoken(button.GetRowNo()) + " miejsce " + GetSeatNoSpoken(button.GetSeatNo());
                SeatPositionTag = "Rząd " + button.GetRowNo() + " miejsce " + button.GetSeatNo();

                if (button.GetRowNo() > Rows) Rows = button.GetRowNo();
                if (button.GetSeatNo() > Cols) Cols = button.GetSeatNo();
            }

            private string GetRowNoSpoken(int rowNo)
            {
                switch (rowNo)
                {
                    case 1:
                        return "pierwszy";
                    case 2:
                        return "drugi";
                    case 3:
                        return "trzeci";
                    case 4:
                        return "czwarty";
                    case 5:
                        return "piąty";
                    case 6:
                        return "szósty";
                    case 7:
                        return "siódmy";
                    case 8:
                        return "ósmy";
                    case 9:
                        return "dziewiąty";
                    case 10:
                        return "dziesiąty";
                }
                return null;
            }

            private string GetSeatNoSpoken(int seatNo)
            {
                switch (seatNo)
                {
                    case 1:
                        return "pierwsze";
                    case 2:
                        return "drugie";
                    case 3:
                        return "trzecie";
                    case 4:
                        return "czwarte";
                    case 5:
                        return "piąte";
                    case 6:
                        return "szóste";
                    case 7:
                        return "siódme";
                    case 8:
                        return "ósme";
                    case 9:
                        return "dziewiąte";
                    case 10:
                        return "dziesiąte";
                }
                return null;
            }
        };

        private Seat[] Seats;

        public Seat[] GetSeats()
        {
            if (Seats == null)
            {
                List<Seat> seats = new List<Seat>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "SELECT Seats.rowNo, Seats.seatNo " +
                            "FROM Seats, Auditoriums, Screenings " +
                            "WHERE (Seats.auditoriumID = Auditoriums.id) AND (Auditoriums.id = " + Screening.Auditorium + ")";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            int rowNo = int.Parse(string.Format("{0}", sqlDataReader[0]));
                            int seatNo = int.Parse(string.Format("{0}", sqlDataReader[1]));
                            
                            //Creating UI comps has to be done by STA thread, otherwise ecxeption is thrown
                            DispatchSync(() =>
                                seats.Add(new Seat(new SeatButton(window, previousPage, sqlConnectionFactory, Screening.Id, rowNo, seatNo))));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                Seats = seats.ToArray();
            }

            return Seats;
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection rules)
        {
            
            SrgsRule movieSrgsRule;

            {
                SrgsOneOf movieSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (Seat seat in GetSeats())
                {
                    SrgsItem srgsItem = new SrgsItem(seat.SeatPositionTag);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"seat." + i++ + "\";"));

                    movieSrgsOneOf.Add(srgsItem);
                }

                SrgsItem movieSrgsItem = new SrgsItem(0, 1, "Wybierz");

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(movieSrgsItem);
                phraseSrgsItem.Add(movieSrgsOneOf);

                movieSrgsRule = new SrgsRule("seat", phraseSrgsItem);
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
            Speak("Wybierz miejsce.");
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
                        case "seat":
                            Seats[int.Parse(command[1])].Button.PerformAction();
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

        private void InitializeSeatsView()
        {
            GetSeats();
            List<RowDefinition> gridRows = new List<RowDefinition>();
            List<ColumnDefinition> gridColumns = new List<ColumnDefinition>();

            for (int i = 0; i < Seat.Rows; i++)
            {
                SeatsGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < Seat.Cols; i++)
            {
                SeatsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            foreach (Seat seat in Seats)
            {
                Grid.SetRow(seat.Button, seat.Button.GetRowNo() - 1);
                Grid.SetColumn(seat.Button, seat.Button.GetSeatNo() - 1);
                SeatsGrid.Children.Add(seat.Button);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }
    }
}
