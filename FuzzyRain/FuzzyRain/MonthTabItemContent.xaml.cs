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

        public string MonthName { get; set; }

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
            
            float consumo2 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 2);
            float consumo4 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 4);
            float consumo6 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 6);
            float consumo8 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 8);

            _list2.Add(new Point(rain, consumo2));
            _list4.Add(new Point(rain, consumo4));
            _list6.Add(new Point(rain, consumo6));
            _list8.Add(new Point(rain, consumo8));

            return ProcessStatusEnum.processing;
        }

        #region Export Methods

        private void ButtonGuardarExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Excel 97-2003 WorkBook|*.xls"
            };

            dialog.FileName = "lluvia_" + this.MonthName;
            if (dialog.ShowDialog() == true)
            {
                IWorkbook workbook = new HSSFWorkbook();

                ISheet precipitacionSheet = workbook.CreateSheet("datos_simulados");

                int rowNumber = 0;
                IRow rowTitle = precipitacionSheet.CreateRow(rowNumber);

                ICell cellAnioTitle = rowTitle.CreateCell(0, CellType.STRING);
                ICell cellMesTitle = rowTitle.CreateCell(1, CellType.STRING);
                ICell cellSemanaTitle = rowTitle.CreateCell(2, CellType.STRING);
                ICell cellDiaTitle = rowTitle.CreateCell(3, CellType.STRING);
                ICell cellCantTitle = rowTitle.CreateCell(4, CellType.STRING);
                ICell cellCons2Title = rowTitle.CreateCell(5, CellType.STRING);
                ICell cellCons4Title = rowTitle.CreateCell(6, CellType.STRING);
                ICell cellCons6Title = rowTitle.CreateCell(7, CellType.STRING);
                ICell cellCons8Title = rowTitle.CreateCell(8, CellType.STRING);

                cellAnioTitle.SetCellValue("Año");
                cellMesTitle.SetCellValue("Mes");
                cellSemanaTitle.SetCellValue("Semana");
                cellDiaTitle.SetCellValue("Dia");
                cellCantTitle.SetCellValue("Cantidad Lluvia (mm)");
                cellCons2Title.SetCellValue("X 2 per. (lts)");
                cellCons4Title.SetCellValue("X 4 per. (lts)");
                cellCons6Title.SetCellValue("X 6 per. (lts)");
                cellCons8Title.SetCellValue("X 8 per. (lts)");

                for (int i = 0; i < myDistribution.ValuesInOrderOfAppearance.Count; i++)
                {
                    Rain rain = myDistribution.ValuesInOrderOfAppearance[i];

                    rowNumber++;
                    IRow rowValue = precipitacionSheet.CreateRow(rowNumber);

                    ICell cellAnioValue = rowValue.CreateCell(0, CellType.STRING);
                    ICell cellMesValue = rowValue.CreateCell(1, CellType.STRING);
                    ICell cellSemanaValue = rowValue.CreateCell(2, CellType.STRING);
                    ICell cellDiaValue = rowValue.CreateCell(3, CellType.STRING);
                    ICell cellCantValue = rowValue.CreateCell(4, CellType.NUMERIC);
                    ICell cellCons2Value = rowValue.CreateCell(5, CellType.NUMERIC);
                    ICell cellCons4Value = rowValue.CreateCell(6, CellType.NUMERIC);
                    ICell cellCons6Value = rowValue.CreateCell(7, CellType.NUMERIC);
                    ICell cellCons8Value = rowValue.CreateCell(8, CellType.NUMERIC);

                    cellAnioValue.SetCellValue(rain.Period.Year);
                    cellMesValue.SetCellValue(rain.Period.Month);
                    cellSemanaValue.SetCellValue(rain.Period.Week);
                    cellDiaValue.SetCellValue(rain.Period.Day);
                    cellCantValue.SetCellValue(rain.Quantity);
                    cellCons2Value.SetCellValue(_list2.ElementAt(i).Consumed);
                    cellCons4Value.SetCellValue(_list4.ElementAt(i).Consumed);
                    cellCons6Value.SetCellValue(_list6.ElementAt(i).Consumed);
                    cellCons8Value.SetCellValue(_list8.ElementAt(i).Consumed);
                }

                FileStream sw = File.Create(dialog.FileName);
                workbook.Write(sw);
                sw.Close();
            }
        }

        private void ButtonGuardarExcel_Click2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Excel 97-2003 WorkBook|*.xls"
            };

            dialog.FileName = "lluvia_" + this.MonthName;
            if (dialog.ShowDialog() == true)
            {
                IWorkbook workbook = new HSSFWorkbook();

                CreatePrecipitacionSheet(workbook);
                ISheet sheetConsumo2 = workbook.CreateSheet("Consumo2");
                ISheet sheetConsumo4 = workbook.CreateSheet("Consumo4");
                ISheet sheetConsumo6 = workbook.CreateSheet("Consumo6");
                ISheet sheetConsumo8 = workbook.CreateSheet("Consumo8");

                int rowNumber = 0;
                CreateRowExtraInfo(sheetConsumo2, "Cant. Personas", 2, rowNumber);
                CreateRowExtraInfo(sheetConsumo4, "Cant. Personas", 4, rowNumber);
                CreateRowExtraInfo(sheetConsumo6, "Cant. Personas", 6, rowNumber);
                CreateRowExtraInfo(sheetConsumo8, "Cant. Personas", 8, rowNumber);

                rowNumber++;
                CreateRowExtraInfo(sheetConsumo2, "Superficie (m2)", myDistribution.SimulationData.Surface, rowNumber);
                CreateRowExtraInfo(sheetConsumo4, "Superficie (m2)", myDistribution.SimulationData.Surface, rowNumber);
                CreateRowExtraInfo(sheetConsumo6, "Superficie (m2)", myDistribution.SimulationData.Surface, rowNumber);
                CreateRowExtraInfo(sheetConsumo8, "Superficie (m2)", myDistribution.SimulationData.Surface, rowNumber);

                rowNumber++;
                CreateRowExtraInfo(sheetConsumo2, "Volumen (m3)", myDistribution.SimulationData.Volumen, rowNumber);
                CreateRowExtraInfo(sheetConsumo4, "Volumen (m3)", myDistribution.SimulationData.Volumen, rowNumber);
                CreateRowExtraInfo(sheetConsumo6, "Volumen (m3)", myDistribution.SimulationData.Volumen, rowNumber);
                CreateRowExtraInfo(sheetConsumo8, "Volumen (m3)", myDistribution.SimulationData.Volumen, rowNumber);

                // para dejar una fila de espacio.
                rowNumber++;

                rowNumber++;
                CreateRowTitles(sheetConsumo2, rowNumber);
                CreateRowTitles(sheetConsumo4, rowNumber);
                CreateRowTitles(sheetConsumo6, rowNumber);
                CreateRowTitles(sheetConsumo8, rowNumber);

                for (int i = 0; i < _list2.Count; i++)
                {
                    rowNumber++;
                    CreateRowValues(sheetConsumo2, rowNumber, _list2.ElementAt(i).Rain, _list2.ElementAt(i).Consumed);
                    CreateRowValues(sheetConsumo4, rowNumber, _list4.ElementAt(i).Rain, _list4.ElementAt(i).Consumed);
                    CreateRowValues(sheetConsumo6, rowNumber, _list6.ElementAt(i).Rain, _list6.ElementAt(i).Consumed);
                    CreateRowValues(sheetConsumo8, rowNumber, _list8.ElementAt(i).Rain, _list8.ElementAt(i).Consumed);
                }

                FileStream sw = File.Create(dialog.FileName);
                workbook.Write(sw);
                sw.Close();
            }            
        }

        private void CreatePrecipitacionSheet(IWorkbook workbook)
        {
            ISheet precipitacionSheet = workbook.CreateSheet("datos_simulados");

            int rowNumber = 0;
            IRow rowTitle = precipitacionSheet.CreateRow(rowNumber);

            ICell cellAnioTitle = rowTitle.CreateCell(0, CellType.STRING);
            ICell cellMesTitle = rowTitle.CreateCell(1, CellType.STRING);
            ICell cellSemanaTitle = rowTitle.CreateCell(2, CellType.STRING);
            ICell cellDiaTitle = rowTitle.CreateCell(3, CellType.STRING);
            ICell cellCantTitle = rowTitle.CreateCell(4, CellType.STRING);

            cellAnioTitle.SetCellValue("Año");
            cellMesTitle.SetCellValue("Mes");
            cellSemanaTitle.SetCellValue("Semana");
            cellDiaTitle.SetCellValue("Dia");
            cellCantTitle.SetCellValue("Cantidad Lluvia (mm)");

            foreach (Rain rain in myDistribution.ValuesInOrderOfAppearance)
            {
                rowNumber++;
                IRow rowValue = precipitacionSheet.CreateRow(rowNumber);

                ICell cellAnioValue = rowValue.CreateCell(0, CellType.STRING);
                ICell cellMesValue = rowValue.CreateCell(1, CellType.STRING);
                ICell cellSemanaValue = rowValue.CreateCell(2, CellType.STRING);
                ICell cellDiaValue = rowValue.CreateCell(3, CellType.STRING);
                ICell cellCantValue = rowValue.CreateCell(4, CellType.NUMERIC);
                
                cellAnioValue.SetCellValue(rain.Period.Year);
                cellMesValue.SetCellValue(rain.Period.Month);
                cellSemanaValue.SetCellValue(rain.Period.Week);
                cellDiaValue.SetCellValue(rain.Period.Day);
                cellCantValue.SetCellValue(rain.Quantity);
            }
        }

        private void CreateRowExtraInfo(ISheet sheet, string title, int value, int rowNumber)
        {
            IRow row = sheet.CreateRow(rowNumber);
            ICell cellTitle = row.CreateCell(0, CellType.STRING);
            ICell cellValue = row.CreateCell(1, CellType.NUMERIC);

            cellTitle.SetCellValue(title);
            cellValue.SetCellValue(value);
        }

        private void CreateRowTitles(ISheet sheet, int rowNumber)
        {
            IRow rowTitle = sheet.CreateRow(rowNumber);
            
            ICell cellRainTitle = rowTitle.CreateCell(0, CellType.STRING);
            ICell cellConsumedTitle = rowTitle.CreateCell(1, CellType.STRING);

            cellRainTitle.SetCellValue("Cant. Lluvia (mm)");
            cellConsumedTitle.SetCellValue("Consumo (ltrs)");
        }

        private void CreateRowValues(ISheet sheet, int rowNumber, double rain, double consumed)
        {
            IRow row = sheet.CreateRow(rowNumber);

            ICell cellRainValue = row.CreateCell(0, CellType.NUMERIC);
            ICell cellConsumedValue = row.CreateCell(1, CellType.NUMERIC);

            cellRainValue.SetCellValue(rain);
            cellConsumedValue.SetCellValue(consumed);
        }

        #endregion
    }
}
