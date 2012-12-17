using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SimulationMethods;
using System.IO;
using System.Xml;
using FuzzyRain.Model;
using FuzzyLogic;

namespace FuzzyRain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer UpdateAnimationTimer;

        private MonteCarloWithRanks[] models = new MonteCarloWithRanks[13];

        // TODO: los valores por defecto son los que fueron usados para el caso de estudio. Verificar.
        private const int RANK_AMPLITUDE = 10;
        private const int RANK_COUNT = 12;

        private const double CONVERGENCE_ERROR = 0.25;

        private const int NUMBER_OF_EVENTS_DEFAULT = 40;

        public MainWindow()
        {
            InitializeComponent();
            UpdateAnimationTimer = new DispatcherTimer();
            UpdateAnimationTimer.Tick += new EventHandler(UpdateAnimationTimer_Tick);
            UpdateAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            FuzzyLogic.FuzzyRain.Instance.DoInference(9);
        }
        private void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".txt";
            dlg.Filter = "XML documents (.xml)|*.xml";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                TextBoxArchivoEntrada.Text = filename;
                this.ButtonComenzar.IsEnabled = true;                
            }
        }

        private void MenuItemSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItemCargarLLuvia_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void ButtonExaminar_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void ButtonComenzar_Click(object sender, RoutedEventArgs e)
        {
            BeginSimulation();

            // Parseo
            Distribution[] valuesParsed = ParseFile(TextBoxArchivoEntrada.Text);

            // Simulación de Monte Carlo
            Simulation(valuesParsed);

            ButtonComenzar.Content = "Re-intentar";
        }

        private void UpdateAnimationTimer_Tick(object sender, EventArgs e)
        {
            //Random random = new Random();
            //double newSize = random.NextDouble() * 200;
            //ImageLLuvia.Width = newSize;
            //ImageLLuvia.Height = newSize;
            //LabelLLuvia.Content = newSize.ToString("00.00") + "mm";

            //FuzzyRainResult result = FuzzyLogic.FuzzyRain.Instance.DoInference((float)newSize);
            //if (result != null)
            //{
            //    double newSize1 = random.NextDouble() * 200;
            //    ImageSuperficie.Width = newSize1;
            //    ImageSuperficie.Height = newSize1;
            //    LabelSuperficie.Content = result.Surface.ToString("00.00") + "mm";

            //    double newSize2 = random.NextDouble() * 200;
            //    ImageVolumen.Width = newSize2;
            //    ImageVolumen.Height = newSize2;
            //    LabelVolumen.Content = result.Volume.ToString("00.00") + "mm";
            //}

            // TODO: deberian creearse threads que pidan el next value al models 
            // (arreglo con las 12 instancias de la simulacion de montecarlo para cada mes)
            for (int i = 10; i <= 12; i++)
            {
                TabItem tab = (TabItem)tabMonths.Items[i - 1];
                ((MonthTabItemContent)tab.Content).Tick();
            }
        }

        private void BeginSimulation()
        {
            UpdateAnimationTimer.Start();
        }

        #region Simulation Methods

        private void Simulation(Distribution[] distributions)
        {
            double ErrorOfConvergence = 0;
            double.TryParse(txtConvError.Text, out ErrorOfConvergence);
            ErrorOfConvergence = ErrorOfConvergence != 0 ? ErrorOfConvergence : CONVERGENCE_ERROR;

            int numberOfEvents = string.IsNullOrEmpty(txtCountEvents.Text) ? NUMBER_OF_EVENTS_DEFAULT : int.Parse(txtCountEvents.Text);

            // Set Parsed Data
            SetDataMonths(distributions, true);

            Distribution[] simulations = new Distribution[13];
            for (int i = 10; i <= 12; i++)
            {
                simulations[i] = new Distribution();
                
                // Iniciar la simulacion
                models[i] = new MonteCarloWithRanks(ErrorOfConvergence, distributions[i]);
                
                // TODO: esto se tendría que hacer en la parte de lógica difusa
                // Obtener el siguiente elemento luego de que la simulacon converge
                for (int e = 1; e <= numberOfEvents; e++)
                {
                    simulations[i].ValuesInOrderOfAppearance.Add(models[i].NextValue());
                }

                SetConvergenceData(models[i].ConvergenceAvg, models[i].ConvergenceDesv, models[i].ConvergenceValue, i);                
            }

            // Set Simulated Data
            SetDataMonths(simulations, false);                        
        }

        public Distribution[] ParseFile(string fileName)
        {
            var rankCount = 0;
            int.TryParse(txtRankCount.Text, out rankCount);
            rankCount = rankCount != 0 ? rankCount : RANK_COUNT;            
           
            var rankAmplitude = 0;
            int.TryParse(txtRankAmplitude.Text, out rankAmplitude);
            rankAmplitude = rankAmplitude != 0 ? rankAmplitude : RANK_AMPLITUDE;
            
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);            

            Distribution[] monthsPrecipitations = new Distribution[13];

            for (int i = 1; i <= 12; i++)
            {
                monthsPrecipitations[i] = new Distribution();                
                monthsPrecipitations[i].Ranks = CreateRanks(rankCount, rankAmplitude);                
            }

            try
            {
                int month;
                double precipitation;
                SimulationType simulationType = GetSimulationType();

                foreach (XmlNode item in xDoc.SelectNodes("/rainfall/yearfall/fall"))
                {                    
                    month = int.Parse(item.SelectSingleNode("month").Attributes["value"].InnerText);
                    precipitation = double.Parse(item.SelectSingleNode("precipitation").Attributes["value"].InnerText);
                                        
                    monthsPrecipitations[month].PutValueInRank(precipitation, simulationType);                    
                }

            }
            catch (Exception)
            {
                monthsPrecipitations = null;
            }

            return monthsPrecipitations;
        }
        
        private Rank[] CreateRanks(int rankCount, int rankAmplitude)
        {
            Rank[] ranks = new Rank[rankCount];

            for (int i = 0; i < rankCount; i++)
            {
                ranks[i] = new Rank(i * rankAmplitude, i * rankAmplitude + rankAmplitude);
            }

            return ranks;
        }

        private SimulationType GetSimulationType()
        {
            switch (cmbSimulationType.SelectedIndex)
            {
                case 0:
                    return SimulationType.Daily;
                case 1:
                    return SimulationType.Weekly;
                default:
                    return SimulationType.Monthly;
            }
        }

        #endregion

        #region User Control Handlers

        private void SetDataMonths(Distribution[] distributions, bool isInput)
        {
            for (int i = 10; i <= 12; i++)
            {
                TabItem tab = (TabItem)tabMonths.Items[i - 1];
                ((MonthTabItemContent)tab.Content).SetDataMonths(distributions[i], isInput);
            }
        }

        private void SetConvergenceData(double avg, double desv, int eventNumberOfConvergence, int month)
        {
            TabItem tab = (TabItem)tabMonths.Items[month - 1];
            ((MonthTabItemContent)tab.Content).SetConvergenceData(avg, desv, eventNumberOfConvergence);
        }

        #endregion        

    }    
}
