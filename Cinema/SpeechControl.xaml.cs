using System;
using System.Collections.Generic;
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
    /// Interaction logic for SpeechControl.xaml
    /// </summary>
    public partial class SpeechControl : UserControl
    {
        private SpeechPage Page;

        public SpeechControl()
        {
            InitializeComponent();
        }

        internal void SetParent(SpeechPage page)
        {
            Page = page;
        }

        private void SkipSpeechButton_Click(object sender, RoutedEventArgs e)
        {
            Page?.StopSpeak();
        }

        internal void SwitchOn()
        {
            SpeakOnImage.Visibility = Visibility.Visible;
            SpeakOffImage.Visibility = Visibility.Hidden;
        }

        internal void SwitchOff()
        {
            SpeakOnImage.Visibility = Visibility.Hidden;
            SpeakOffImage.Visibility = Visibility.Visible;
        }
    }
}
