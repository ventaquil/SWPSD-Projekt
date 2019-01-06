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
    /// Interaction logic for ConfirmationPage.xaml
    /// </summary>
    public partial class ConfirmationPage : SpeechPage
    {
        private string BookerName;

        private Price Price;
        
        private Seat Seat;

        private int TicketId;

        public ConfirmationPage(Window window, SqlConnectionFactory sqlConnectionFactory, Seat seat, Price price, string bookerName) : base(window, sqlConnectionFactory)
        {
            InitializeComponent();
            Loaded += (sender, args) => SpeechControl.SetParent(this);
            
            Seat = seat;
            Price = price;
            BookerName = bookerName;
            TicketId = GetTicketId();

            ShowTicketInfo();
        }

        private int GetTicketId()
        {
            int ticketId;

            using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT Id " +
                        "FROM Tickets " +
                        "WHERE (seatID = " + Seat.Id + ") AND " +
                            "(screeningID = " + Seat.Screening.Id + ") AND " +
                            "(priceID = " + Price.Id + ") AND " +
                            "(bookerName = '" + BookerName + "')";

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Read();

                    ticketId = int.Parse(string.Format("{0}", sqlDataReader[0]));

                    sqlDataReader.Close();
                }
            }

            return ticketId;
        }

        protected override SpeechControl GetSpeechControl()
        {
            return SpeechControl;
        }

        private void ShowTicketInfo()
        {
            Screening screening = Seat.Screening;
            Movie movie = screening.Movie;

            TicketInfoItemsControl.Items.Add(string.Format("Numer zamówienia: {0:D8}", TicketId));
            TicketInfoItemsControl.Items.Add(string.Format("Film: {0}", movie.Title));
            TicketInfoItemsControl.Items.Add(string.Format("Data: {0}", screening.Date));
            TicketInfoItemsControl.Items.Add(string.Format("Godzina: {0}", screening.Time));
            TicketInfoItemsControl.Items.Add(string.Format("Sala: {0}", screening.Auditorium));
            TicketInfoItemsControl.Items.Add(string.Format("Miejsce: rząd {0}, miejsce {1}", Seat.Row, Seat.No));
            TicketInfoItemsControl.Items.Add(string.Format("Zamawiający: {0}", BookerName));
            TicketInfoItemsControl.Items.Add(string.Format("Cena: {0} zł", Price.Value));
        }

        public override void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            base.InitializeSpeech(sender, e);

            SpeakHello();
        }

        private void SpeakHello()
        {
            Speak("Dziękujemy za złożenie zamówienia!");
            Speak("Zapisz numer swojego zamówienia aby podać go w kasie.");
        }

        private void SpeakHelp()
        {
            Speak("Aby przejść do menu głównego powiedz STRONA GŁÓWNA.");
            Speak("Aby wyjść powiedz WYJDŹ.");
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
                        case "help":
                            SpeakHelp();
                            break;
                        case "mainmenu":
                            GoToMainMenu();
                            break;
                        case "quit":
                            SpeakQuit();
                            Close();
                            break;
                    }
                });
            }
        }
        
        private void GoToMainMenu()
        {
            ChangePage(((MainWindow)window).MainPage);
        }

        private void MainMenuButtonButton_Click(object sender, RoutedEventArgs e)
        {
            GoToMainMenu();
        }
    }
}
