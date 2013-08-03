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
using FuzzyRain.Model;
using FuzzyLogic;
using SimulationMethods;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.ObjectModel;
using System.IO;

namespace FuzzyRain
{
    public class Point : DependencyObject
    {
        public static readonly DependencyProperty _rain = DependencyProperty.Register("Rain", typeof(double), typeof(Point));
        public Point(double rain, double consumed)
        {
            Rain = rain;
            Consumed = consumed;
        }
        public double Rain
        {
            get { return (double)GetValue(_rain); }
            set { SetValue(_rain, value); }
        }
        public static readonly DependencyProperty _consumed = DependencyProperty.Register("Consumed", typeof(double), typeof(Point));
        public double Consumed
        {
            get { return (double)GetValue(_consumed); }
            set { SetValue(_consumed, value); }
        }
    }

    /// <summary>
    /// Interaction logic for MonthTab.xaml
    /// </summary>
    public partial class MonthTabItemContent : UserControl
    {
        public MonteCarloWithRanks myModel;
        public Distribution myDistribution;
        int eventsCount;

        ObservableCollection<Point> _list2 = new ObservableCollection<Point>();
        public ObservableCollection<Point> List2
        {
            get { return _list2; }
        }

        ObservableCollection<Point> _list4 = new ObservableCollection<Point>();
        public ObservableCollection<Point> List4
        {
            get { return _list4; }
        }

        ObservableCollection<Point> _list6 = new ObservableCollection<Point>();
        public ObservableCollection<Point> List6
        {
            get { return _list6; }
        }

        ObservableCollection<Point> _list8 = new ObservableCollection<Point>();
        public ObservableCollection<Point> List8
        {
            get { return _list8; }
        }

        public MonthTabItemContent()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SetInitialInputData(Distribution distribution)
        {
            ucInputData.CleanData();
            ucInputData.SetInitialData(distribution);
        }

        public void SetInitialOutputData(MonteCarloWithRanks model, int numberOfEvents)
        {
            myModel = model;
            myDistribution = new Distribution(myModel.MyDistribution.SimulationData, int.Parse(myModel.MyDistribution.Month));
            eventsCount = numberOfEvents;

            ucOutputData.CleanData();
            ucOutputData.CleanDataList();
            ucOutputData.SetConvergenceData(model.ConvergenceAvg, model.ConvergenceDesv, model.ConvergenceValue);
            //TODO: borrar esto. Es solo para corroborar los valores inferidos.
            //checkearInferencias.Items.Clear();
            //checkearInferencias4.Items.Clear();
            //checkearInferencias6.Items.Clear();
            //checkearInferencias8.Items.Clear();

            List2.Clear();
            List4.Clear();
            List6.Clear();
            List8.Clear();
        }

        /// <summary>
        /// Cleans output data control
        /// </summary>
        public void CleanInitialData()
        {
            ucInputData.CleanData();
            ucOutputData.CleanData();
            ucOutputData.CleanDataList();
            //TODO: borrar esto. Es solo para corroborar los valores inferidos.
            //checkearInferencias.Items.Clear();
            //checkearInferencias4.Items.Clear();
            //checkearInferencias6.Items.Clear();
            //checkearInferencias8.Items.Clear();

            List2.Clear();
            List4.Clear();
            List6.Clear();
            List8.Clear();
        }

        public ProcessStatusEnum Tick()
        {
            if (eventsCount == 0)
            {
                ucOutputData.Finalize(myDistribution);
                eventsCount -= 1;
                return ProcessStatusEnum.finished;
            }

            if (eventsCount < 0)
                return ProcessStatusEnum.stop;

            eventsCount -= 1;

            double rain = myModel.NextValue();
            ucOutputData.AddNewSimulatedItem(myDistribution.AddValueInOrderOfAppearance(rain, eventsCount));           

            double surface = myDistribution.SimulationData.Surface;
            
            double volumen = myDistribution.SimulationData.Volumen;            

            // TODO: actualizar este dato en una grafica. Podrían imprimirse en pantalla la lista de valores tambien.
            float consumo2 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 2);
            float consumo4 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 4);
            float consumo6 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 6);
            float consumo8 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 8);

            //TODO: borrar esto. Es solo para corroborar los valores inferidos.
            //checkearInferencias.Items.Add(consumo2);
            //checkearInferencias4.Items.Add(consumo4);
            //checkearInferencias6.Items.Add(consumo6);
            //checkearInferencias8.Items.Add(consumo8);

            _list2.Add(new Point(rain, consumo2));
            _list4.Add(new Point(rain, consumo4));
            _list6.Add(new Point(rain, consumo6));
            _list8.Add(new Point(rain, consumo8));

            return ProcessStatusEnum.processing;
        }

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

                for (int i = 0; i < _list2.Count; i++)
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

                    cellRainConsumo2.SetCellValue(_list2.ElementAt<Point>(i).Rain);
                    cellRainConsumo4.SetCellValue(_list4.ElementAt(i).Rain);
                    cellRainConsumo6.SetCellValue(_list6.ElementAt(i).Rain);
                    cellRainConsumo8.SetCellValue(_list8.ElementAt(i).Rain);

                    cellConsumoConsumo2.SetCellValue(_list2.ElementAt(i).Consumed);
                    cellConsumoConsumo4.SetCellValue(_list4.ElementAt(i).Consumed);
                    cellConsumoConsumo6.SetCellValue(_list6.ElementAt(i).Consumed);
                    cellConsumoConsumo8.SetCellValue(_list8.ElementAt(i).Consumed);

                    rowNumber++;
                }

                FileStream sw = File.Create(dialog.FileName);
                workbook.Write(sw);
                sw.Close();
            }
        }
    }
}
