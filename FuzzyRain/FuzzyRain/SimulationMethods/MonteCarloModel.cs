using System.Collections.Generic;
using React.Distribution;
using System.Linq;
using FuzzyRain.Model;

namespace SimulationMethods
{
    /// <summary>
    /// Monte Carlo Simulation
    /// </summary>
    public class MonteCarloModel
    {                           
        public Distribution MyDistribution { get; set; }

        private double _mean = 0.0;
        private double _std_dev = 0.0;

        public MonteCarloModel(int rankCount, Rank[] ranks, double mean, double std_dev)
        {
            _mean = mean;
            _std_dev = std_dev;
            MyDistribution = new Distribution();
            MyDistribution.RankCount = rankCount;
            MyDistribution.Ranks = ranks;
            Run();
        }

        public MonteCarloModel(int rankCount, Rank[] ranks)
        {
            MyDistribution = new Distribution();
            MyDistribution.RankCount = rankCount;
            MyDistribution.Ranks = ranks;
            Run();
        }

        public MonteCarloModel(double mean, double std_dev)
        {
            _mean = mean;
            _std_dev = std_dev;
            MyDistribution = new Distribution();
            Run();
        }

        /// <summary>
        /// Runs the Monte Carlo Simulation
        /// </summary>
        private void Run()
        {
            var distributionType = new Normal(_mean, _std_dev);

            int i = 0;            
            while(i < 20000)
            {                
                double nextValue = distributionType.NextDouble();                
                
                i++;                
                //MyDistribution.PutValueInRank(nextValue);
                MyDistribution.AddValueInOrderOfAppearance(nextValue);
            }            
        }

        public List<double> GetFirstNEvents(int count)
        {
            //TODO: verificar si es posible que la librería solo genere números positivos (lo hice pero no encontré la forma).
            List<double> valuesPositive = MyDistribution.ValuesInOrderOfAppearance.Where(x => x >= 0).ToList();

            return valuesPositive.Take<double>(count).ToList();
        }
    }    
}