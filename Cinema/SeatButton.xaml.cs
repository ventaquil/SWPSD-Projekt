using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for SeatButton.xaml
    /// </summary>
    public partial class SeatButton : UserControl
    {
        private Action Callback;

        private Seat Seat;
        
        public SeatButton(Seat seat, Action callback)
        {
            InitializeComponent();

            Seat = seat;
            Callback = callback;

            InitializeSeatButton();
        }

        private void InitializeSeatButton()
        {
            MainButton.Content = "Rząd: " + Seat.Row + "\nMiejsce: " + Seat.No;

            MainButton.Background = Seat.Taken ? Brushes.Red : Brushes.Green;
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            PerformAction();
        }

        public void PerformAction()
        {
            if (Seat.Taken)
            {
                MessageBox.Show("To miejsce jest już zajęte!");
            }
            else
            {
                Callback();
            }
        }
    }
}
