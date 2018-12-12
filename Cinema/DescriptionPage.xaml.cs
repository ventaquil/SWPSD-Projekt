using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logika interakcji dla klasy DescriptionPage.xaml
    /// </summary>
    public partial class DescriptionPage : SpeechPage
    {
        private readonly Movie Movie;

        public DescriptionPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory, Movie movie) : base(window, previousPage, sqlConnectionFactory)
        {
            InitializeComponent();
            Loaded += (sender, args) => SpeechControl.SetParent(this);

            Movie = movie;

            InitializeMovieData();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        protected override SpeechControl GetSpeechControl()
        {
            return SpeechControl;
        }

        private void InitializeMovieData()
        {
            TitleTextBlock.Text = Movie.Title;

            DescriptionTextBox.Text = Movie.Description;
        }

        public override void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            base.InitializeSpeech(sender, e);
        }

        private void Order()
        {
            ChangePage(new MovieHoursPage(window, previousPage, sqlConnectionFactory, Movie));
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            Order();
        }
        
        private void SpeakHelp()
        {
            Speak("Aby zamówić bilet powiedz ZAMÓW BILET.");
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
                        case "quit":
                            SpeakQuit();
                            Close();
                            break;
                    }
                });
            }
        }
    }
}
