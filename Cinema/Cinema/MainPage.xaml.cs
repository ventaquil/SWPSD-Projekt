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
    public partial class MainPage : Page, ISpeechRecognize, ISpeechSynthesis
    {
        private SpeechRecognitionEngine speechRecognitionEngine;

        private SpeechSynthesizer speechSynthesizer;

        public MainPage(Window window, SqlConnection sqlConnection) : base(window, sqlConnection)
        {
            InitializeComponent();

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += InitializeSpeech;
            backgroundWorker.RunWorkerAsync();
        }

        private void Close()
        {
            window.Close();
        }

        private void Dispatch(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        public void EnableSpeechRecognition()
        {
            try
            {
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        public Grammar GetSpeechGrammar()
        {
            SrgsDocument srgsDocument = new SrgsDocument("./Resources/MainPage.srgs");

            return new Grammar(srgsDocument);
        }

        private void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            InitializeSpeechSynthesis();

            SpeakHello();

            InitializeSpeechRecognition();

            EnableSpeechRecognition();
        }

        public void InitializeSpeechRecognition()
        {
            CultureInfo cultureInfo = new CultureInfo("pl-PL");

            speechRecognitionEngine = new SpeechRecognitionEngine(cultureInfo);
            speechRecognitionEngine.LoadGrammarAsync(GetSpeechGrammar());
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        }

        public void InitializeSpeechSynthesis()
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

        public void Speak(String message)
        {
            StopSpeechRecognition();

            speechSynthesizer.Speak(message);

            EnableSpeechRecognition();
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

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RecognitionResult result = e.Result;

            Console.WriteLine(result.Semantics.Value + ") " + result.Text + " (" + result.Confidence + ")");

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
                        Dispatch(MoveToOrderPage);
                        break;
                    case "search":
                        Dispatch(MoveToSearchPage);
                        break;
                    case "quit":
                        SpeakQuit();
                        Dispatch(Close);
                        break;
                }
            }
        }

        public void StopSpeechRecognition()
        {
            try
            {
                speechRecognitionEngine.RecognizeAsyncCancel();
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        public void WaitForSpeechRecognition()
        {
            try
            {
                speechRecognitionEngine.RecognizeAsyncStop();
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }
    }
}
