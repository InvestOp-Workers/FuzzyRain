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

            Simulation(valuesParsed);

            stkDataInput.Visibility = System.Windows.Visibility.Visible;
            stkDataOutput.Visibility = System.Windows.Visibility.Visible;

            ButtonComenzar.Content = "Re-intentar";
        }

        private void UpdateAnimationTimer_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            double newSize = random.NextDouble() * 200;
            ImageLLuvia.Width = newSize;
            ImageLLuvia.Height = newSize;
            LabelLLuvia.Content = newSize.ToString("00.00") + "mm";

            FuzzyRainResult result = FuzzyLogic.FuzzyRain.Instance.DoInference((float)newSize);
            if (result != null)
            {
                double newSize1 = random.NextDouble() * 200;
                ImageSuperficie.Width = newSize1;
                ImageSuperficie.Height = newSize1;
                LabelSuperficie.Content = result.Surface.ToString("00.00") + "mm";

                double newSize2 = random.NextDouble() * 200;
                ImageVolumen.Width = newSize2;
                ImageVolumen.Height = newSize2;
                LabelVolumen.Content = result.Volume.ToString("00.00") + "mm";
            }
        }

        private void BeginSimulation()
        {
            UpdateAnimationTimer.Start();
        }

        #region Simulation

        private void Simulation(Distribution[] distributions)
        {
            // TODO: los rangos deberian ser creados de manera matematica.
            var rankCount = 12;

            // TODO: debería ser importado desde el archivo o ingresado desde la ui.
            var ErrorOfConvergence = 0.3;

            int numberOfEvents = string.IsNullOrEmpty(txtCountEvents.Text) ? 0 : int.Parse(txtCountEvents.Text);

            // Set Parsed Data
            SetDataMonths(distributions, true);

            Distribution[] simulations = new Distribution[13];
            for (int i = 10; i <= 12; i++)
            {
                simulations[i] = new Distribution();
                
                // Iniciar la simulacion
                models[i] = new MonteCarloWithRanks(ErrorOfConvergence, distributions[i], rankCount);

                // Obtener el siguiente elemento luego de que la simulacon converge
                // TODO: esto se tendría que hacer en la parte de lógica difusa
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
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);            

            Distribution[] monthsPrecipitations = new Distribution[13];

            for (int i = 1; i <= 12; i++)
            {
                monthsPrecipitations[i] = new Distribution();

                // TODO: los rangos deberian ser creados de manera matematica.
                monthsPrecipitations[i].Ranks = CreateRanks(12);                
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

        // TODO: los rangos deberian ser creados de manera matematica.
        private Rank[] CreateRanks(int rankCount)
        {
            Rank[] ranks = new Rank[rankCount];

            for (int i = 0; i < rankCount; i++)
            {
                ranks[i] = new Rank(i * 10, i * 10 + 10);
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

        private void SetDataMonths(Distribution[] distributions, bool isInput)
        {
            if (isInput)
            {                                
                inputTab.SetDataMonths(distributions);
            }
            else
            {                
                outputTab.SetDataMonths(distributions);
            }            
        }

        private void SetConvergenceData(double avg, double desv, int eventNumberOfConvergence, int month)
        {
            outputTab.SetConvergenceData(avg, desv, eventNumberOfConvergence, month);
        }

        #endregion

    }    
}
