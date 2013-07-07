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
            MyDistribution = new Distribution(new SimulationData(), 0);
            MyDistribution.Ranks = ranks;
            Run();
        }

        public MonteCarloModel(int rankCount, Rank[] ranks)
        {
            MyDistribution = new Distribution(new SimulationData(), 0);
            MyDistribution.Ranks = ranks;
            Run();
        }

        public MonteCarloModel(double mean, double std_dev)
        {
            _mean = mean;
            _std_dev = std_dev;
            MyDistribution = new Distribution(new SimulationData(), 0);
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
                MyDistribution.AddValueInOrderOfAppearance(nextValue, i);
            }

            // TODO: Estos valores corresponden a la media y desvio de toda la simulacion (los 20000 sucesos). Quizas debieran ser mostrados como 
            // informacion de la simulacion en la salida junto a la media y el desvio, que se estan mostrando ahora, para la cantidad de sucesos devueltos.
            var avg = MyDistribution.Average;
            var desv = MyDistribution.Std_Desv;
        }

        public List<double> GetFirstNEvents(int count)
        {
            //TODO: verificar si es posible que la librería solo genere números positivos (lo hice pero no encontré la forma).
            List<double> valuesPositive = MyDistribution.ValuesInOrderOfAppearance.Where(x => x.Quantity >= 0).Select(y => y.Quantity).ToList();

            return valuesPositive.Take<double>(count).ToList();
        }
    }    
}