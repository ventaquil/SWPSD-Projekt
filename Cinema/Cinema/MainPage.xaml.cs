using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
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

        public MainPage(Window window, SqlConnection sqlConnection) : base(window, sqlConnection)
        {
            InitializeComponent();

            InitializeSpeechRecognition();

            EnableSpeechRecognition();
        }

        public void EnableSpeechRecognition()
        {
            try
            {
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            } catch (InvalidOperationException)
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

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(new OrderPage(window, this, sqlConnection));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(new SearchPage(window, this, sqlConnection));
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Semantics.Value + ") " + e.Result.Text);
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
