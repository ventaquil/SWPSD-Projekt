using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    public class Screening
    {
        public readonly int Auditorium;

        public readonly int Id;

        public readonly Movie Movie;

        public readonly string Time;

        public Screening(int id, Movie movie, string time, int auditorium)
        {
            Id = id;

            Movie = movie;

            Time = ParseTime(time);

            Auditorium = auditorium;
        }

        public string GetHour()
        {
            return Time.Split(':').First();
        }

        public string GetMinutes()
        {
            return Time.Split(':').Skip(1).First();
        }

        private string ParseTime(string time)
        {
            return string.Join(":", time.Split(':').Take(2));
        }
    }
}
