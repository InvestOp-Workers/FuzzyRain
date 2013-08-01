﻿using System;
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
using System.Globalization;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Microsoft.Win32;
using System.ComponentModel;

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

        private const double SURFACE = 1200.00;
        private const double VOLUMEN = 100.00;

        List<KeyValuePair<double, float>> listConsumo2 = new List<KeyValuePair<double, float>>();
        List<KeyValuePair<double, float>> listConsumo4 = new List<KeyValuePair<double, float>>();
        List<KeyValuePair<double, float>> listConsumo6 = new List<KeyValuePair<double, float>>();
        List<KeyValuePair<double, float>> listConsumo8 = new List<KeyValuePair<double, float>>();

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            UpdateAnimationTimer = new DispatcherTimer();
            UpdateAnimationTimer.Tick += new EventHandler(UpdateAnimationTimer_Tick);
            UpdateAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void OpenFile()
        {
            ButtonComenzar.Content = "Comenzar";
            CleanAllInitialData();

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
            InitInputValues();

            BeginSimulation();

            // Parseo
            Distribution[] valuesParsed = ParseFile(TextBoxArchivoEntrada.Text);

            // Simulación de Monte Carlo
            Simulation(valuesParsed);
            

            ButtonComenzar.Content = "Re-intentar";


        }

        private void InitInputValues()
        {
            var rankCount = 0;
            int.TryParse(txtRankCount.Text, out rankCount);
            txtRankCount.Text = rankCount != 0 ? rankCount.ToString() : RANK_COUNT.ToString();

            var rankAmplitude = 0;
            int.TryParse(txtRankAmplitude.Text, out rankAmplitude);
            txtRankAmplitude.Text = rankAmplitude != 0 ? rankAmplitude.ToString() : RANK_AMPLITUDE.ToString();

            double ErrorOfConvergence = 0;
            double.TryParse(txtConvError.Text, out ErrorOfConvergence);
            txtConvError.Text = ErrorOfConvergence != 0 ? ErrorOfConvergence.ToString() : CONVERGENCE_ERROR.ToString();

            var numberOfEvents = 0;
            int.TryParse(txtCountEvents.Text, out numberOfEvents);
            txtCountEvents.Text = numberOfEvents != 0 ? numberOfEvents.ToString() : NUMBER_OF_EVENTS_DEFAULT.ToString();

            double surface = 0;
            double.TryParse(txtSup.Text, out surface);
            txtSup.Text = surface != 0 ? surface.ToString() : SURFACE.ToString();

            double volumen = 0;
            double.TryParse(txtVol.Text, out volumen);
            txtVol.Text = volumen != 0 ? volumen.ToString() : VOLUMEN.ToString();
        }

        private void UpdateAnimationTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 10; i <= 12; i++)
            {
                TabItem tab = (TabItem)tabMonths.Items[i - 1];

                double rain = ((MonthTabItemContent)tab.Content).myModel.NextValue();
                double surface = ((MonthTabItemContent)tab.Content).myDistribution.SimulationData.Surface;
                double volumen = ((MonthTabItemContent)tab.Content).myDistribution.SimulationData.Volumen;
            
                float consumo2 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 2);
                float consumo4 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 4);
                float consumo6 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 6);
                float consumo8 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 8);
            
                ((MonthTabItemContent)tab.Content).Tick(rain, surface, volumen);

                listConsumo2.Add(new KeyValuePair<double, float>(rain, consumo2));
                listConsumo4.Add(new KeyValuePair<double, float>(rain, consumo4));
                listConsumo6.Add(new KeyValuePair<double, float>(rain, consumo6));
                listConsumo8.Add(new KeyValuePair<double, float>(rain, consumo8));
               
                //Setting data for column chart
                chartConsumo2.DataContext = listConsumo2;
                chartConsumo4.DataContext = listConsumo4;
                chartConsumo6.DataContext = listConsumo6;
                chartConsumo8.DataContext = listConsumo8;
            }
        }

        private void BeginSimulation()
        {
            UpdateAnimationTimer.Start();
        }

        #region Simulation Methods

        private void Simulation(Distribution[] distributions)
        {
            double ErrorOfConvergence = double.Parse(txtConvError.Text);
            int numberOfEvents = string.IsNullOrEmpty(txtCountEvents.Text) ? NUMBER_OF_EVENTS_DEFAULT : int.Parse(txtCountEvents.Text);

            // Set Parsed Data
            SetInitialInputData(distributions);
            
            for (int i = 10; i <= 12; i++)
            {
                // Iniciar la simulacion
                models[i] = new MonteCarloWithRanks(ErrorOfConvergence, distributions[i]);
            }

            // Set Simulated Data
            SetInitialOutputData(models, numberOfEvents);
        }

        public Distribution[] ParseFile(string fileName)
        {
            var rankCount = int.Parse(txtRankCount.Text);
            var rankAmplitude = int.Parse(txtRankAmplitude.Text);
            
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);            

            Distribution[] monthsPrecipitations = new Distribution[13];

            for (int i = 1; i <= 12; i++)
            {
                monthsPrecipitations[i] = new Distribution(GetSimulationData(), i);                
                monthsPrecipitations[i].CreateRanks(rankCount, rankAmplitude);
            }

            try
            {
                int month;
                double precipitation;
                int year;
                SimulationType simulationType = GetSimulationType();

                foreach (XmlNode item in xDoc.SelectNodes("/rainfall/yearfall"))
                {
                    year = int.Parse(item.Attributes["year"].Value);
                    
                    //foreach (XmlNode item2 in xDoc.SelectNodes("/rainfall/yearfall/fall"))
                    foreach (XmlNode item2 in item.SelectNodes("fall"))
                    {
                        month = int.Parse(item2.SelectSingleNode("month").Attributes["value"].InnerText);
                        precipitation = double.Parse(item2.SelectSingleNode("precipitation").Attributes["value"].InnerText, CultureInfo.InvariantCulture);

                        monthsPrecipitations[month].PutValueInRank(year, precipitation);
                    }
                }

            }
            catch (Exception)
            {
                monthsPrecipitations = null;
            }

            return monthsPrecipitations;
        }

        private SimulationData GetSimulationData()
        {
            SimulationData result = new SimulationData();
            result.SimulationType = GetSimulationType();            
            result.Surface = int.Parse(txtSup.Text);
            result.Volumen = int.Parse(txtVol.Text);

            return result;
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

        private void CleanAllInitialData()
        {
            for (int i = 10; i <= 12; i++)
            {
                TabItem tab = (TabItem)tabMonths.Items[i - 1];
                ((MonthTabItemContent)tab.Content).CleanInitialData();
            }
        }

        private void SetInitialInputData(Distribution[] distributions)
        {
            for (int i = 10; i <= 12; i++)
            {
                TabItem tab = (TabItem)tabMonths.Items[i - 1];
                ((MonthTabItemContent)tab.Content).SetInitialInputData(distributions[i]);
            }
        }

        private void SetInitialOutputData(MonteCarloWithRanks[] models, int numberOfEvents)
        {
            for (int i = 10; i <= 12; i++)
            {
                TabItem tab = (TabItem)tabMonths.Items[i - 1];
                ((MonthTabItemContent)tab.Content).SetInitialOutputData(models[i], numberOfEvents);
            }
        }

        #endregion

        private void ButtonGuardarExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Excel 97-2003 WorkBook|*.xls"
            };

            if (dialog.ShowDialog() == true)
            {
                IWorkbook workbook = new HSSFWorkbook();
                int rowNumber = 0;
                ISheet sheetConsumo2 = workbook.CreateSheet("Consumo2");
                ISheet sheetConsumo4 = workbook.CreateSheet("Consumo4");
                ISheet sheetConsumo6 = workbook.CreateSheet("Consumo6");
                ISheet sheetConsumo8 = workbook.CreateSheet("Consumo8");

                for (int i = 0; i < listConsumo2.Count; i++)
                {
                    IRow rowConsumo2 = sheetConsumo2.CreateRow(rowNumber);
                    IRow rowConsumo4 = sheetConsumo4.CreateRow(rowNumber);
                    IRow rowConsumo6 = sheetConsumo6.CreateRow(rowNumber);
                    IRow rowConsumo8 = sheetConsumo8.CreateRow(rowNumber);

                    ICell cellRainConsumo2 = rowConsumo2.CreateCell(0, CellType.NUMERIC);
                    ICell cellConsumoConsumo2 = rowConsumo2.CreateCell(1, CellType.NUMERIC);

                    ICell cellRainConsumo4 = rowConsumo4.CreateCell(0, CellType.NUMERIC);
                    ICell cellConsumoConsumo4 = rowConsumo4.CreateCell(1, CellType.NUMERIC);

                    ICell cellRainConsumo6 = rowConsumo6.CreateCell(0, CellType.NUMERIC);
                    ICell cellConsumoConsumo6 = rowConsumo6.CreateCell(1, CellType.NUMERIC);

                    ICell cellRainConsumo8 = rowConsumo8.CreateCell(0, CellType.NUMERIC);
                    ICell cellConsumoConsumo8 = rowConsumo8.CreateCell(1, CellType.NUMERIC);

                    cellRainConsumo2.SetCellValue(listConsumo2.ElementAt(i).Key);
                    cellRainConsumo4.SetCellValue(listConsumo2.ElementAt(i).Key);
                    cellRainConsumo6.SetCellValue(listConsumo2.ElementAt(i).Key);
                    cellRainConsumo8.SetCellValue(listConsumo2.ElementAt(i).Key);

                    cellConsumoConsumo2.SetCellValue(listConsumo2.ElementAt(i).Value);
                    cellConsumoConsumo4.SetCellValue(listConsumo4.ElementAt(i).Value);
                    cellConsumoConsumo6.SetCellValue(listConsumo6.ElementAt(i).Value);
                    cellConsumoConsumo8.SetCellValue(listConsumo8.ElementAt(i).Value);

                    rowNumber++;
                }

                FileStream sw = File.Create(dialog.FileName);
                workbook.Write(sw);
                sw.Close();
            }
        }

    }    
}
