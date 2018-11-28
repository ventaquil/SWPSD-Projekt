using Microsoft.Speech.Recognition;
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

        void Speak(String message);

        void StopSpeak();
    }
}
