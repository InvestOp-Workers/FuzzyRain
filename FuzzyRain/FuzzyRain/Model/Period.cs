using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyRain.Model
{
    public class Period
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Week { get; set; }
        public string Day { get; set; }

        public int YearInt
        {
            get { return Year.Equals("-") ? 0 : int.Parse(Year); }
        }
        public int MonthInt
        {
            get { return Month.Equals("-") ? 0 : int.Parse(Month); }
        }
        public int WeekInt
        {
            get { return Week.Equals("-") ? 0 : int.Parse(Week); }
        }
        public int DayInt
        {
            get { return Day.Equals("-") ? 0 : int.Parse(Day); }
        }

        public Period(string year = "-", string month = "-", string week = "-", string day = "-")
        {
            Year = year;
            Month = month;
            Week = week;
            Day = day;
        }
    }
}
