using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyRain.Model
{
    public class Rain
    {
        public double Quantity { get; set; }
        public string Period { get; set; }

        public string GetFormatedValue()
        {
            return Period + " --> " + Quantity;
        }
    }
}
