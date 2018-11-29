using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    public class Screening
    {
        public readonly int Auditorium;

        public readonly string Date;

        public readonly int Id;

        public readonly Movie Movie;

        public readonly string Time;

        public Screening(int id, Movie movie, string date, string time, int auditorium)
        {
            Id = id;

            Movie = movie;

            Date = ParseDate(date);

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

        private string ParseDate(string date)
        {
            return DateTime.ParseExact(date, "dd.MM.yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy");
        }

        private string ParseTime(string time)
        {
            return string.Join(":", time.Split(':').Take(2));
        }
    }
}
