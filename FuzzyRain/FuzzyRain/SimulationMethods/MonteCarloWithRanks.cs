using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationMethods
{
    public class MonteCarloWithRanks
    {
        public Distribution MyDistribution { get; set; }
        public List<double> ValuesInOrderOfAppearance = new List<double>();

        public MonteCarloWithRanks(int rankCount, Rank[] ranks)
        {
            MyDistribution = new Distribution();            
            MyDistribution.Ranks = ranks;
            Run();
        }        

        /// <summary>
        /// Runs the Monte Carlo Simulation
        /// </summary>
        private void Run()
        {            
            int i = 0;
            Random random = new Random();
            double calculatedValue;
            
            // TODO: verificar la convergencia
            while (i < 40000)
            {                
                double nextValue = random.NextDouble();
                
                i++;
                
                calculatedValue = MyDistribution.PutValueInRankUsingFrequency(nextValue);
                ValuesInOrderOfAppearance.Add(calculatedValue);
            }

            var avg = MyDistribution.Average;
        }

        public List<double> GetFirstNEvents(int count)
        {            
            return MyDistribution.ValuesInOrderOfAppearance.Take<double>(count).ToList();            
        }
    }
}
