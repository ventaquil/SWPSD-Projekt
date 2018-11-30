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
        private CultureInfo CultureInfo = new CultureInfo("pl-PL");

        private SpeechRecognitionEngine speechRecognitionEngine;

        private SpeechSynthesizer speechSynthesizer;

        private SpeechControl speechControl;

        public SpeechPage() : this(null, null, null)
        {
        }

        public SpeechPage(Window window, SqlConnectionFactory sqlConnectionFactory) : this(window, null, sqlConnectionFactory)
        {
        }

        public SpeechPage(Window window, Page previousPage, SqlConnectionFactory sqlConnectionFactory) : base(window, previousPage, sqlConnectionFactory)
        {
            ExecuteBackgroundAction(InitializeSpeech);
        }

        protected virtual void AddCustomSpeechGrammarRules(SrgsRulesCollection srgsRules)
        {
        }

        protected void DispatchAsync(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        protected void DispatchSync(Action action)
        {
            Dispatcher.Invoke(action);
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

            AddCustomSpeechGrammarRules(srgsDocument.Rules);

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
            speechRecognitionEngine = new SpeechRecognitionEngine(CultureInfo);
            ReloadGrammars();
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        }

        public void InitializeSpeechSynthesis()
        {
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
        }

        protected void LoadGrammarAsync(Grammar grammar)
        {
            speechRecognitionEngine.LoadGrammarAsync(grammar);
        }

        protected void LoadGrammarSync(Grammar grammar)
        {
            speechRecognitionEngine.LoadGrammar(grammar);
        }

        protected void ReloadGrammars()
        {
            speechRecognitionEngine.UnloadAllGrammars();

            LoadGrammarAsync(GetSpeechGrammar());
        }

        public void Speak(string message, SpeechControl speechControl)
        {
            if (this.speechControl == null) this.speechControl = speechControl;

            PromptBuilder promptBuilder = new PromptBuilder(CultureInfo);
                promptBuilder.AppendText(message);

                Prompt prompt = new Prompt(promptBuilder);

                Speak(prompt);
                
        }

        public void Speak(Prompt prompt)
        {
            DispatchAsync(new Action(() =>
            {
                speechControl.SpeakOnImage.Visibility = Visibility.Hidden;
                speechControl.SpeakOffImage.Visibility = Visibility.Visible;
            }));
            

            StopSpeechRecognition();

            try
            {
                speechSynthesizer.Speak(prompt);

                EnableSpeechRecognition();

                DispatchAsync(new Action(() =>
                {
                    speechControl.SpeakOffImage.Visibility = Visibility.Hidden;
                    speechControl.SpeakOnImage.Visibility = Visibility.Visible;
                }));
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected virtual void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RecognitionResult result = e.Result;

            Console.WriteLine(GetType().Name + "[" + result.Semantics.Value + "] " + result.Text + " (" + result.Confidence + ")");
        }

        public void StopSpeak()
        {
            speechSynthesizer.SpeakAsyncCancelAll();
            DispatchAsync(new Action(() =>
            {
                speechControl.SpeakOffImage.Visibility = Visibility.Hidden;
                speechControl.SpeakOnImage.Visibility = Visibility.Visible;
            }));
            EnableSpeechRecognition();
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
