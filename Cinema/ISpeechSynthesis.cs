using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    interface ISpeechSynthesis
    {
        void InitializeSpeechSynthesis();

        void Speak(string message);

        void Speak(Prompt prompt);

        void StopSpeak();
    }
}
