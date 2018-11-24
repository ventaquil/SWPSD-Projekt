using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    public class Movie
    {
        public readonly string Description;

        public readonly string Name;

        public Movie(string name)
        {
            Name = name;
        }

        public Movie(string name, string description) : this(name)
        {
            Description = description;
        }
    }
}
