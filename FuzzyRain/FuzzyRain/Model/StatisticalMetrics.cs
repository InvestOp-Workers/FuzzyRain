using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyRain.Model
{
    public static class StatisticalMetrics
    {

        #region Average

        public static double GetAverage(List<double> values, DistributionType distributionType)
        {
            double summary = 0;
            foreach (var value in values)
            {
                summary += value;
            }

            return summary / (values.Count * (int)distributionType);
        }

        public static double GetAverage(List<double> values)
        {
            // TODO: Le pasamos DistributionType = Monthly porque necesitamos que divida por 1. Talvez debiera ser revisado.
            return GetAverage(values, DistributionType.Monthly);
        }

        #endregion

        #region Std desv

        // TODO: verficar porque no lo este calculando de manera correcta.
        public static double GetDesv(List<double> values, DistributionType distributionType)
        {
            double sum = 0;
            double mean = GetAverage(values, distributionType);

            foreach (double val in values)
            {
                sum = sum + Math.Pow(val - mean, 2);
            }

            sum = sum / ((values.Count * (int)distributionType) - 1);
            return Math.Sqrt(sum);
        }

        public static double GetDesv(List<double> values)
        {
            // TODO: Le pasamos DistributionType = Monthly porque necesitamos que divida por 1. Talvez debiera ser revisado.
            return GetDesv(values, DistributionType.Monthly);
        }

        #endregion
    }
}
