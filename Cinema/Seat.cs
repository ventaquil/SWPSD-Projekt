using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    public class Seat
    {
        public readonly int No;

        public readonly int Row;

        public readonly Screening Screening;

        public readonly bool Taken;

        public Seat(Screening screening, int no, int row, bool taken)
        {
            Screening = screening;

            No = no;

            Row = row;

            Taken = taken;
        }
    }
}
