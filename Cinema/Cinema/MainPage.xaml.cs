using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
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
    public partial class MainPage : Page, SpeechControllable
    {
        private SpeechRecognitionEngine speechRecognitionEngine;

        private SpeechSynthesizer speechSynthesizer;

        public MainPage(Window window, SqlConnection sqlConnection) : base(window, sqlConnection)
        {
            InitializeComponent();

            InitializeSpeechRecognition();

            EnableSpeechRecognition();

            InitializeSpeechSynthesis();

            SpeakHello();
        }

        public void EnableSpeechRecognition()
        {
            try
            {
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (InvalidOperationException)
            {
                // pass
            }
        }

        public Grammar GetSpeechGrammar()
        {
            SrgsDocument srgsDocument = new SrgsDocument("./Resources/MainPage.srgs");

            return new Grammar(srgsDocument);
        }

        public void InitializeSpeechRecognition()
        {
            CultureInfo cultureInfo = new CultureInfo("pl-PL");

            speechRecognitionEngine = new SpeechRecognitionEngine(cultureInfo);
            speechRecognitionEngine.LoadGrammarAsync(GetSpeechGrammar());
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        }

        private void InitializeSpeechSynthesis()
        {
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
        }

        private void MoveToOrderPage()
        {
            ChangePage(new OrderPage(window, this, sqlConnection));
        }

        private void MoveToSearchPage()
        {
            ChangePage(new SearchPage(window, this, sqlConnection));
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            MoveToOrderPage();
        }

        private void SpeakHello()
        {
            speechSynthesizer.SpeakAsync("Witaj w automacie kinowym gdzie możesz wyszukać filmy lub kupić bilety. Powiedz POMOC w razie potrzeby.");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MoveToSearchPage();
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RecognitionResult result = e.Result;

            Console.WriteLine(result.Semantics.Value + ") " + result.Text + " (" + result.Confidence + ")");

            if (result.Confidence < 0.6)
            {
                // repeat
            }
            else
            {
                string command = result.Semantics.Value.ToString().ToLower();
                switch (command)
                {
                    case "help":
                        break;
                    case "order":
                        MoveToOrderPage();
                        break;
                    case "search":
                        MoveToSearchPage();
                        break;
                    case "quit":
                        window.Close();
                        break;
                }
            }
        }

        public void StopSpeechRecognition()
        {
            speechRecognitionEngine.RecognizeAsyncCancel();
        }

        public void WaitForSpeechRecognition()
        {
            speechRecognitionEngine.RecognizeAsyncStop();
        }
    }
}
