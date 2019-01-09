using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using Microsoft.Speech.Synthesis;
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

        private int? Row;

        private int Rows;

        private bool RowsSet = false;

        private Screening Screening;

        private int? Seat;

        private Seat[] Seats;

        public MovieSeatsPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Screening screening) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();
            Loaded += (sender, args) => SpeechControl.SetParent(this);

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

        protected override SpeechControl GetSpeechControl()
        {
            return SpeechControl;
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection rules)
        {
            AddRowSpeechGrammarRules(rules);
            AddRowSeatSpeechGrammarRules(rules);
            AddSeatSpeechGrammarRules(rules);
        }

        private void AddRowSpeechGrammarRules(SrgsRulesCollection rules)
        {
            SrgsRule rowSrgsRule;

            {
                SrgsOneOf rowSrgsOneOf = new SrgsOneOf();

                List<int> rows = new List<int>();

                foreach (Seat seat in GetSeats())
                {
                    if (!rows.Contains(seat.Row))
                    {
                        rows.Add(seat.Row);
                    }
                }

                foreach (int row in rows)
                {
                    SrgsItem srgsItem = new SrgsItem("Rząd " + row);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"row." + row + "\";"));

                    rowSrgsOneOf.Add(srgsItem);
                }

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(new SrgsItem(0, 1, "Wybierz"));
                phraseSrgsItem.Add(rowSrgsOneOf);

                rowSrgsRule = new SrgsRule("row", phraseSrgsItem);
            }

            rules.Add(rowSrgsRule);

            {
                SrgsItem srgsItem = new SrgsItem();
                srgsItem.Add(new SrgsRuleRef(rowSrgsRule));

                SrgsRule rootSrgsRule = rules.Where(rule => rule.Id == "root").First();
                SrgsOneOf srgsOneOf = (SrgsOneOf)rootSrgsRule.Elements.Where(element => element is SrgsOneOf).First();
                srgsOneOf.Add(srgsItem);
            }
        }

        private void AddRowSeatSpeechGrammarRules(SrgsRulesCollection rules)
        {
            SrgsRule rowSeatSrgsRule;

            {
                SrgsOneOf rowSeatSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (Seat seat in GetSeats())
                {
                    SrgsItem srgsItem = new SrgsItem("Rząd " + seat.Row + " miejsce " + seat.No);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"rowseat." + i++ + "\";"));

                    rowSeatSrgsOneOf.Add(srgsItem);
                }

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(new SrgsItem(0, 1, "Wybierz"));
                phraseSrgsItem.Add(rowSeatSrgsOneOf);

                rowSeatSrgsRule = new SrgsRule("rowseat", phraseSrgsItem);
            }

            rules.Add(rowSeatSrgsRule);

            {
                SrgsItem srgsItem = new SrgsItem();
                srgsItem.Add(new SrgsRuleRef(rowSeatSrgsRule));

                SrgsRule rootSrgsRule = rules.Where(rule => rule.Id == "root").First();
                SrgsOneOf srgsOneOf = (SrgsOneOf)rootSrgsRule.Elements.Where(element => element is SrgsOneOf).First();
                srgsOneOf.Add(srgsItem);
            }
        }

        private void AddSeatSpeechGrammarRules(SrgsRulesCollection rules)
        {
            SrgsRule seatSrgsRule;

            {
                SrgsOneOf seatSrgsOneOf = new SrgsOneOf();

                List<int> seats = new List<int>();

                foreach (Seat seat in GetSeats())
                {
                    if (!seats.Contains(seat.No))
                    {
                        seats.Add(seat.No);
                    }
                }

                foreach (int seat in seats)
                {
                    SrgsItem srgsItem = new SrgsItem("Miejsce " + seat);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"seat." + seat + "\";"));

                    seatSrgsOneOf.Add(srgsItem);
                }

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(new SrgsItem(0, 1, "Wybierz"));
                phraseSrgsItem.Add(seatSrgsOneOf);

                seatSrgsRule = new SrgsRule("seat", phraseSrgsItem);
            }

            rules.Add(seatSrgsRule);

            {
                SrgsItem srgsItem = new SrgsItem();
                srgsItem.Add(new SrgsRuleRef(seatSrgsRule));

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
            PromptBuilder promptBuilder = new PromptBuilder();
            promptBuilder.AppendText("Aby wybrać miejsce powiedz WYBIERZ RZĄD");
            promptBuilder.AppendSsmlMarkup("<prosody rate=\"slow\"><say-as interpret-as=\"characters\">K</say-as></prosody>");
            promptBuilder.AppendText("MIEJSCE");
            promptBuilder.AppendSsmlMarkup("<prosody rate=\"slow\"><say-as interpret-as=\"characters\">L</say-as></prosody>");

            Prompt prompt = new Prompt(promptBuilder);

            Speak(prompt);
            Speak("Aby wrócić powiedz WRÓĆ.");
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
                        case "help":
                            SpeakHelp();
                            break;
                        case "row":
                            if (Row != null)
                            {
                                Seat = null;
                            }
                            Row = int.Parse(command.Skip(1).First());
                            TakeSeat(Row, Seat);
                            break;
                        case "rowseat":
                            TakeSeat(int.Parse(command.Skip(1).First()));
                            break;
                        case "seat":
                            if (Seat != null)
                            {
                                Row = null;
                            }
                            Seat = int.Parse(command.Skip(1).First());
                            TakeSeat(Row, Seat);
                            break;
                        case "quit":
                            SpeakQuit();
                            Close();
                            break;
                    }
                });
            }
        }

        private void TakeSeat(int? row, int? seat)
        {
            if (row == null)
            {
                if (GetRows() > 1)
                {
                    Speak("Który rząd?");
                } else
                {
                    row = 1;
                }
            }

            if (seat == null)
            {
                if (GetColumns() > 1)
                {
                    Speak("Które miejsce?");
                } else
                {
                    seat = 1;
                }
            }

            if ((row != null) && (seat != null))
            {
                int index = ((int) seat - 1) + (GetColumns() * ((int) row - 1));

                TakeSeat(index);
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
            if (seat.Taken)
            {
                Speak("To miejsce jest zajęte.");
            }
            else
            {
                DispatchAsync(() =>
                {
                    ChangePage(new TicketDataPage(window, this, sqlConnectionFactory, seat));
                });
            }
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
                SeatButton seatButton = new SeatButton(seat, () =>
                {
                    TakeSeat(seat);
                });

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
