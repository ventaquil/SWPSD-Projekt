using Microsoft.Speech.Recognition;
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
        private SqlConnection dbConnection;
        private SpeechRecognitionEngine speechRecognitionEngine;
        private Window window;

        public MainPage(Window window, SqlConnection dbConnection)
        {
            this.window = window;
            this.dbConnection = dbConnection;

            InitializeComponent();

            InitializeSpeechRecognition();

            EnableSpeechRecognition();
        }

        private void ChangePage(Page page)
        {
            StopSpeechRecognition();

            window.Content = page;
        }

        public void EnableSpeechRecognition()
        {
            speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        public Grammar GetSpeechGrammar()
        {
            Choices choices = new Choices();
            choices.Add("Raz");
            choices.Add("Dwa");
            choices.Add("Trzy");

            GrammarBuilder grammarBuilder = new GrammarBuilder(choices);

            return new Grammar(grammarBuilder);
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
            ChangePage(new OrderPage(window, this, dbConnection));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(new SearchPage(window, this, dbConnection));
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Text);
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
