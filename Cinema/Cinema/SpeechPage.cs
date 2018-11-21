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

namespace Cinema
{
    public class SpeechPage : Page, ISpeechRecognize, ISpeechSynthesis
    {
        private SpeechRecognitionEngine speechRecognitionEngine;

        private SpeechSynthesizer speechSynthesizer;

        public SpeechPage() : this(null, null, null)
        {
        }

        public SpeechPage(Window window, SqlConnection sqlConnection) : this(window, null, sqlConnection)
        {
        }

        public SpeechPage(Window window, Page previousPage, SqlConnection sqlConnection) : base(window, previousPage, sqlConnection)
        {
            ExecuteBackgroundAction(InitializeSpeech);
        }

        protected void Dispatch(Action action)
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

        protected void ExecuteBackgroundAction(DoWorkEventHandler action)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += action;
            backgroundWorker.RunWorkerAsync();
        }

        public Grammar GetSpeechGrammar()
        {
            SrgsDocument srgsDocument = new SrgsDocument("./Resources/" + GetType().Name + ".srgs");

            return new Grammar(srgsDocument);
        }

        public virtual void InitializeSpeech(object sender, DoWorkEventArgs e)
        {
            InitializeSpeechSynthesis();

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

        public void Speak(string message)
        {
            StopSpeechRecognition();

            speechSynthesizer.Speak(message);

            EnableSpeechRecognition();
        }

        protected virtual void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RecognitionResult result = e.Result;

            Console.WriteLine("[" + result.Semantics.Value + "] " + result.Text + " (" + result.Confidence + ")");
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
