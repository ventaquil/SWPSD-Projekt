using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for TicketDataPage.xaml
    /// </summary>
    public partial class TicketDataPage : SpeechPage
    {
        private Price[] Prices;

        private Seat Seat;

        public TicketDataPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Seat seat) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();

            Seat = seat;

            InitializeComboBox();
        }

        private void SpeakHelp()
        {
            Speak("Aby pokazać dostępne rodzaje biletów powiedz POKAŻ DOSTĘPNE RODZAJE BILETÓW.");
            Speak("Aby wybrać bilet powiedz WYBIERZ RODZAJ BILETU.");
            Speak("Aby kontynuować powiedz GOTOWE.");
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
                        case "order":
                            Order();
                            break;
                        case "ticket":
                            PriceComboBox.SelectedIndex = int.Parse(command.Skip(1).First());
                            PriceComboBox.IsDropDownOpen = false;
                            break;
                        case "tickets":
                            PriceComboBox.IsDropDownOpen = true;
                            break;
                        case "quit":
                            SpeakQuit();
                            Close();
                            break;
                    }
                });
            }
        }

        private void Order()
        {
            if (PriceComboBox.SelectedIndex == -1)
            {
                Speak("Musisz najpierw wybrać rodzaj biletu.");
            }
            else if (NameTextBox.Text.Length == 0)
            {
                Speak("Podaj swoje imię i nazwisko.");
            }
            else
            {
                Price price = GetPrice(PriceComboBox.SelectedIndex);
                string bookerName = string.Format("{0}", NameTextBox.Text);

                ChangePage(new SummaryPage(window, this, sqlConnectionFactory, Seat, price, bookerName));
            }
        }

        protected override void AddCustomSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
            SrgsRule ticketSrgsRule;

            {
                SrgsOneOf ticketSrgsOneOf = new SrgsOneOf();

                int i = 0;
                foreach (Price price in GetPrices())
                {
                    SrgsItem srgsItem = new SrgsItem(price.Description);
                    srgsItem.Add(new SrgsSemanticInterpretationTag("out=\"ticket." + i++ + "\";"));

                    ticketSrgsOneOf.Add(srgsItem);
                }

                SrgsItem ticketSrgsItem = new SrgsItem("Wybierz");
                ticketSrgsItem.Add(new SrgsItem(0, 1, "bilet"));

                SrgsItem phraseSrgsItem = new SrgsItem();
                phraseSrgsItem.Add(ticketSrgsItem);
                phraseSrgsItem.Add(ticketSrgsOneOf);

                ticketSrgsRule = new SrgsRule("ticket", phraseSrgsItem);
            }

            srgsRules.Add(ticketSrgsRule);

            {
                SrgsItem srgsItem = new SrgsItem();
                srgsItem.Add(new SrgsRuleRef(ticketSrgsRule));

                SrgsRule rootSrgsRule = srgsRules.Where(rule => rule.Id == "root").First();
                SrgsOneOf srgsOneOf = (SrgsOneOf)rootSrgsRule.Elements.Where(element => element is SrgsOneOf).First();
                srgsOneOf.Add(srgsItem);
            }
        }

        private Price[] GetPrices()
        {
            if (Prices == null)
            {
                List<Price> prices = new List<Price>();

                using (SqlConnection sqlConnection = sqlConnectionFactory.Create())
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "SELECT id, priceDescription, price FROM Prices";

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            int id = int.Parse(string.Format("{0}", sqlDataReader[0]));
                            string description = string.Format("{0}", sqlDataReader[1]);
                            float price = float.Parse(string.Format("{0}", sqlDataReader[2]));

                            prices.Add(new Price(id, price, description));
                        }
                        sqlDataReader.Close();
                    }

                    sqlConnection.Close();
                }

                Prices = prices.ToArray();
            }

            return Prices;
        }

        private void InitializeComboBox()
        {
            foreach (Price price in GetPrices())
            {
                PriceComboBox.Items.Add(string.Format("{0} ({1} zł)", price.Description, price.Value));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order();
        }

        private Price GetPrice(int index)
        {
            try
            {
                return GetPrices()[index];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }
    }
}
