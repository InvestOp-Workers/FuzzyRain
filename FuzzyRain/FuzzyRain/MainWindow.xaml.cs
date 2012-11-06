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

namespace FuzzyRain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer UpdateAnimationTimer;

        public MainWindow()
        {
            InitializeComponent();
            UpdateAnimationTimer = new DispatcherTimer();
            UpdateAnimationTimer.Tick += new EventHandler(UpdateAnimationTimer_Tick);
            UpdateAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
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
            
            // Simulacion
            List<double>[] valuesParsed = ParseFile(TextBoxArchivoEntrada.Text);
            MonteCarloSimulation(valuesParsed);
        }

        private void UpdateAnimationTimer_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            double newSize = random.NextDouble() * 200;
            ImageLLuvia.Width = newSize;
            ImageLLuvia.Height = newSize;
            LabelLLuvia.Content = newSize.ToString("00.00") + "mm";

            double newSize1 = random.NextDouble() * 200;
            ImageSuperficie.Width = newSize1;
            ImageSuperficie.Height = newSize1;
            LabelSuperficie.Content = newSize1.ToString("00.00") + "mm";

            double newSize2 = random.NextDouble() * 200;
            ImageVolumen.Width = newSize2;
            ImageVolumen.Height = newSize2;
            LabelVolumen.Content = newSize2.ToString("00.00") + "mm";
        }

        private void BeginSimulation()
        {
            UpdateAnimationTimer.Start();
        }

        private void MonteCarloSimulation(List<double>[] values)
        {
            // TODO: Replace harcoded values by import values of excel.
            // We need a file with the ranks and cumulative frequency 
            var rankCount = 12;
            //double[] limits = new double[rankCount];
            Rank[] ranks = new Rank[rankCount];

            for (int i = 0; i < rankCount; i++)
            {
                //limits[i] = i * 10;
                ranks[i] = new Rank(i * 10, i * 10 + 10);
            }
            
            double mean = GetMean(values[10]);
            double std_dev = 15.6465375035832;

            // TODO: verficar porque no lo este calculando de manera correcta.
            //double std_dev = GetDesv(values[10], mean);

            int numberOfEvents = 40;

            // Obtain Model                        
            var myModel = new MonteCarloModel(rankCount, ranks, mean, std_dev);

            myModel.ValuesInOrderOfAppearance.Take(numberOfEvents).ToList();
        }

        public List<double>[] ParseFile(string fileName)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);            

            // TODO: Es un array de 12 listas, 1 x mes, las cuales tienen los valores de lluvias de ese mes para la cantidad de años muestreados.
            // no parece una buena estructura pero fue la primera que se me ocurrió para probar el parseo, hay que cambiarla.
            List<double>[] monthsPrecipitations = new List<double>[13];
            
            monthsPrecipitations[10] = new List<double>();
            monthsPrecipitations[11] = new List<double>();

            try
            {
                int month;
                double precipitation;
                foreach (XmlNode item in xDoc.SelectNodes("/rainfall/yearfall/fall"))
                {                    
                    month = int.Parse(item.SelectSingleNode("month").Attributes["value"].InnerText);
                    precipitation = double.Parse(item.SelectSingleNode("precipitation").Attributes["value"].InnerText);

                    if (month == 10)
                        monthsPrecipitations[10].Add(precipitation);
                    else if (month == 11)
                        monthsPrecipitations[11].Add(precipitation);
                }

            }
            catch (Exception ex)
            {
                monthsPrecipitations = null;
            }

            return monthsPrecipitations;
        }

        private double GetMean(List<double> values)
        {
            double sum = 0;

            foreach (double val in values)
            {
                sum = sum + val;
            }

            // calcula la media por mes
            //return sum / values.Count;

            // TODO: Esto devolveria la media de la semana, porque el valor del mes sería 4 valores iguales por cada semana
            // de todos modos hay que ver como es mejor implementar esto, es solo para ir armando un esqueleto.
            return sum / (values.Count * 4);
        }

        // TODO: Verfiicar la formula ya que la aplico y no me da el valor del desvio que esta en el archivo.
        private double GetDesv(List<double> values, double mean)
        {
            double sum = 0;

            foreach (double val in values)
            {
                sum = sum + Math.Pow(val - mean, 2);
            }

            sum = sum / ( (values.Count * 4) - 1 );
            return Math.Sqrt( sum );
        }
    }
}
