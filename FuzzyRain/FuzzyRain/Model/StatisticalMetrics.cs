using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyRain.Model
{
    public static class StatisticalMetrics
    {
        public static double GetAverage(IList<double> values)
        {
            double summary = 0;
            foreach (var value in values)
            {
                summary += value;
            }

            return summary / values.Count;
        }
        
        public static double GetDesv(IList<double> values)
        {
            double sum = 0;
            double mean = GetAverage(values);

            foreach (double val in values)
            {
                sum = sum + Math.Pow(val - mean, 2);
            }

            sum = sum / (values.Count - 1);
            return Math.Sqrt(sum);
        }        
    }
}
