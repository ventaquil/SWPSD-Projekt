using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : SpeechPage
    {
        public MainPage(Window window, SqlConnectionFactory sqlConnectionFactory) : base(window, sqlConnectionFactory)
        {
            InitializeComponent();
        }

        public override void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            base.InitializeSpeech(sender, e);

            SpeakHello();
        }

        private void MoveToOrderPage()
        {
            ChangePage(new OrderPage(window, this, sqlConnectionFactory));
        }

        private void MoveToSearchPage()
        {
            ChangePage(new SearchPage(window, this, sqlConnectionFactory));
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            MoveToOrderPage();
        }

        private void SpeakHello()
        {
            Speak("Witaj w automacie kinowym gdzie możesz wyszukać filmy lub kupić bilety. Powiedz POMOC w razie potrzeby.");
        }

        private void SpeakHelp()
        {
            Speak("Aby kupić bilet powiedz ZAMÓW BILET. Aby wyszukać film powiedz WYSZUKIWARKA FILMÓW. Aby wyjść powiedz ZAKOŃCZ.");
        }

        private void SpeakRepeat()
        {
            Speak("Powtórz proszę.");
        }

        private void SpeakQuit()
        {
            Speak("Zapraszam ponownie.");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MoveToSearchPage();
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
                string command = result.Semantics.Value.ToString().ToLower();
                switch (command)
                {
                    case "help":
                        SpeakHelp();
                        break;
                    case "order":
                        DispatchAsync(MoveToOrderPage);
                        break;
                    case "search":
                        DispatchAsync(MoveToSearchPage);
                        break;
                    case "quit":
                        SpeakQuit();
                        DispatchAsync(Close);
                        break;
                }
            }
        }
    }
}
