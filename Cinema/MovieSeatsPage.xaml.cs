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
        private int Columns;

        private bool ColumnsSet = false;

        private int Rows;

        private bool RowsSet = false;

        private Screening Screening;

        private Seat[] Seats;

        public MovieSeatsPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Screening screening) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();

            Screening = screening;

            InitializeSeatsView();
        }

        public int GetColumns()
        {
            if (!ColumnsSet)
            {
                Columns = 0;

                foreach (Seat seat in GetSeats())
                {
                    Columns = Math.Max(Columns, seat.No);
                }

                ColumnsSet = true;
            }

            return Columns;
        }

        public int GetRows()
        {
            if (!RowsSet)
            {
                Rows = 0;

                foreach (Seat seat in GetSeats())
                {
                    Rows = Math.Max(Rows, seat.Row);
                }

                RowsSet = true;
            }

            return Rows;
        }

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
                        sqlCommand.CommandText = "SELECT DISTINCT Seats.id, Seats.rowNo, Seats.seatNo, Tickets.id AS ticket " +
                            "FROM Auditoriums, Screenings, " +
                                "(Seats LEFT JOIN Tickets ON (Tickets.seatID = Seats.id) AND (Tickets.screeningID = " + Screening.Id + ")) " +
                            "WHERE (Seats.auditoriumID = Auditoriums.id) AND (Auditoriums.id = " + Screening.Auditorium + ")";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            int id = int.Parse(string.Format("{0}", sqlDataReader[0]));
                            int row = int.Parse(string.Format("{0}", sqlDataReader[1]));
                            int no = int.Parse(string.Format("{0}", sqlDataReader[2]));
                            bool taken = string.Format("{0}", sqlDataReader[3]) != string.Empty;

                            seats.Add(new Seat(Screening, id, no, row, taken));
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
                    SrgsItem srgsItem = new SrgsItem("Rząd " + seat.Row + " miejsce " + seat.No);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"seat." + i++ + "\";"));

                    movieSrgsOneOf.Add(srgsItem);
                }
                
                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(new SrgsItem(0, 1, "Wybierz"));
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
                            TakeSeat(int.Parse(command.Skip(1).First()));
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

        private void TakeSeat(int index)
        {
            try
            {
                TakeSeat(GetSeats()[index]);
            }
            catch (IndexOutOfRangeException)
            {
            }
        }

        private void TakeSeat(Seat seat)
        {
            ChangePage(new TicketDataPage(window, this, sqlConnectionFactory, seat));
        }

        private void InitializeSeatsView()
        {
            for (int i = 0; i < GetRows(); ++i)
            {
                SeatsGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < GetColumns(); ++i)
            {
                SeatsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            foreach (Seat seat in GetSeats())
            {
                SeatButton seatButton = new SeatButton(seat, new Action(() => {
                    TakeSeat(seat);
                }));

                Grid.SetRow(seatButton, seat.Row - 1);
                Grid.SetColumn(seatButton, seat.No - 1);

                SeatsGrid.Children.Add(seatButton);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }
    }
}
