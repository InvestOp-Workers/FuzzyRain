using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyRain.Model
{
    public class Period
    {
        public string Year { get; set; }
        public string Week { get; set; }
        public string Day { get; set; }

        public Period(string year = "-", string week = "-", string day = "-")
        {
            Year = year;
            Week = week;
            Day = day;
        }
    }
}
