using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    public class Price
    {
        public readonly int Id;

        public readonly string Description;

        public readonly float Value;

        public Price(int id, float value, string description)
        {
            Id = id;

            Value = value;

            Description = description;
        }
    }
}
