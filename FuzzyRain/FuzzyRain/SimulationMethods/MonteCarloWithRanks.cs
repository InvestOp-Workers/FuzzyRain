using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyRain.Model;

namespace SimulationMethods
{
    public class MonteCarloWithRanks
    {
        public Distribution MyDistribution { get; set; }
        public List<double> ValuesToConvergence = new List<double>();
        public List<double> ValuesAfterConvergence = new List<double>();        
        private Random random = new Random();
        private Distribution distributionBase;
        public double ConvergenceError { private get; set; }
        
        // Propiedades relacionadas a la convergencia.
        public int ConvergenceValue { get; set; }
        public double ConvergenceAvg { get; set; }
        public double ConvergenceDesv { get; set; }

        public MonteCarloWithRanks(double convergenceError, Distribution distBase, int rankCount)
        {
            MyDistribution = new Distribution();            
            MyDistribution.Ranks = distBase.Ranks;
            ConvergenceError = convergenceError;
            distributionBase = distBase;

            Run();
        }        

        /// <summary>
        /// Runs the Monte Carlo Simulation
        /// </summary>
        private void Run()
        {
            Converge();
        }

        private void Converge()
        {
            double sum = 0;
            double avg = 0;
            int cantEvents = 0;
            double calculatedValue;
            double nextValue;

            double originalAvg = distributionBase.Average;

            int x = 1;

            // TODO: Se puede encontrar la convergencia con el 1° suceso pero no tendría validez, por eso se corrobora que sea mayor a 500. Consultar.
            // Esta es la formula de convergencia que se dió en la catedra.
            while (cantEvents < 500 || Math.Abs(originalAvg - avg) > ConvergenceError)
            {
                //TODO: incrementa el error de convergencia en un 0.1 cada 100 mil eventos para que no se de el caso que nunca converja. Consultar.
                if (cantEvents > x * 100000)
                {
                    ConvergenceError += 0.1;
                    x++;
                }

                cantEvents++;

                nextValue = random.NextDouble();                
                calculatedValue = MyDistribution.PutValueInRankUsingFrequency(nextValue);
                ValuesToConvergence.Add(calculatedValue);

                sum = sum + calculatedValue;
                avg = sum / cantEvents;
            }

            this.ConvergenceValue = cantEvents;
            this.ConvergenceAvg = avg;
            this.ConvergenceDesv = StatisticalMetrics.GetDesv(ValuesToConvergence);
        }

        public double NextValue()
        {
            double nextValue = random.NextDouble();

            double calculatedValue = MyDistribution.PutValueInRankUsingFrequency(nextValue);
            ValuesAfterConvergence.Add(calculatedValue);

            return calculatedValue;
        }
    }
}
