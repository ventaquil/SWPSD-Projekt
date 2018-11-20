using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    interface SpeechControllable
    {
        void InitializeSpeechRecognition();

        void EnableSpeechRecognition();

        Grammar GetSpeechGrammar();

        void StopSpeechRecognition();

        void WaitForSpeechRecognition();
    }
}
