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

    public class Point2 : DependencyObject
    {
        public static readonly DependencyProperty _period = DependencyProperty.Register("Period", typeof(int), typeof(Point2));
        public Point2(int period, double rain)
        {
            Period = period;
            Rain = rain;
        }
        public int Period
        {
            get { return (int)GetValue(_period); }
            set { SetValue(_period, value); }
        }
        public static readonly DependencyProperty _rain = DependencyProperty.Register("Rain", typeof(double), typeof(Point2));
        public double Rain
        {
            get { return (double)GetValue(_rain); }
            set { SetValue(_rain, value); }
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
        public int MonthNumber { get; set; }

        ObservableCollection<Point2> _listRainPeriods = new ObservableCollection<Point2>();
        public ObservableCollection<Point2> ListRainPeriods
        {
            get { return _listRainPeriods; }
        }

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

            ListRainPeriods.Clear();
            serieRainPeriod.Title = GetLabelText();
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

            ListRainPeriods.Clear();
            serieRainPeriod.Title = "";
            List2.Clear();
            List4.Clear();
            List6.Clear();
            List8.Clear();
        }

        public void Tick()
        {
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

            _listRainPeriods.Add(new Point2(GetPeriodAsInteger(myDistribution.ValuesInOrderOfAppearance.Last().Period), myDistribution.ValuesInOrderOfAppearance.Last().Quantity));

            eventsCount -= 1;
            if (eventsCount == 0)
            {
                ucOutputData.Finalize(myDistribution);                
            }            
        }

        private string GetLabelText()
        {
            if (myDistribution != null)
            {
                switch (myDistribution.SimulationData.SimulationType)
                {
                    case SimulationType.Daily:
                        return "precip. (mm) \n x día";
                    case SimulationType.Weekly:
                        return "precip. (mm) \n x semana";
                    case SimulationType.Monthly:
                        return "precip. (mm) \n x mes";
                    default:
                        return "precip. (mm) \n x semana";
                }
            }
            else
            {
                return "";
            }
        }

        private int GetPeriodAsInteger(Period period)
        {
            switch (myDistribution.SimulationData.SimulationType)
            {
                case SimulationType.Daily:
                    return period.DayInt + ((period.YearInt - 1) * 30);
                case SimulationType.Weekly:
                    return period.WeekInt + ((period.YearInt - 1) * 4);
                case SimulationType.Monthly:
                    return period.YearInt;
                default:
                    return period.WeekInt + ((period.YearInt - 1) * 4);
            }            
        }

        #region Export Methods

        //private void ButtonGuardarExcel_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog dialog = new SaveFileDialog()
        //    {
        //        Filter = "Excel 97-2003 WorkBook|*.xls"
        //    };

        //    dialog.FileName = "lluvia_" + this.MonthName;
        //    if (dialog.ShowDialog() == true)
        //    {
        //        IWorkbook workbook = new HSSFWorkbook();

        //        ISheet precipitacionSheet = workbook.CreateSheet("datos_simulados");

        //        int rowNumber = 0;
        //        IRow rowTitle = precipitacionSheet.CreateRow(rowNumber);

        //        ICell cellAnioTitle = rowTitle.CreateCell(0, CellType.STRING);
        //        ICell cellMesTitle = rowTitle.CreateCell(1, CellType.STRING);
        //        ICell cellSemanaTitle = rowTitle.CreateCell(2, CellType.STRING);
        //        ICell cellDiaTitle = rowTitle.CreateCell(3, CellType.STRING);
        //        ICell cellCantTitle = rowTitle.CreateCell(4, CellType.STRING);
        //        ICell cellCons2Title = rowTitle.CreateCell(5, CellType.STRING);
        //        ICell cellCons4Title = rowTitle.CreateCell(6, CellType.STRING);
        //        ICell cellCons6Title = rowTitle.CreateCell(7, CellType.STRING);
        //        ICell cellCons8Title = rowTitle.CreateCell(8, CellType.STRING);

        //        cellAnioTitle.SetCellValue("Año");
        //        cellMesTitle.SetCellValue("Mes");
        //        cellSemanaTitle.SetCellValue("Semana");
        //        cellDiaTitle.SetCellValue("Dia");
        //        cellCantTitle.SetCellValue("Cantidad Lluvia (mm)");
        //        cellCons2Title.SetCellValue("X 2 per. (lts)");
        //        cellCons4Title.SetCellValue("X 4 per. (lts)");
        //        cellCons6Title.SetCellValue("X 6 per. (lts)");
        //        cellCons8Title.SetCellValue("X 8 per. (lts)");

        //        for (int i = 0; i < myDistribution.ValuesInOrderOfAppearance.Count; i++)
        //        {
        //            Rain rain = myDistribution.ValuesInOrderOfAppearance[i];

        //            rowNumber++;
        //            IRow rowValue = precipitacionSheet.CreateRow(rowNumber);

        //            ICell cellAnioValue = rowValue.CreateCell(0, CellType.STRING);
        //            ICell cellMesValue = rowValue.CreateCell(1, CellType.STRING);
        //            ICell cellSemanaValue = rowValue.CreateCell(2, CellType.STRING);
        //            ICell cellDiaValue = rowValue.CreateCell(3, CellType.STRING);
        //            ICell cellCantValue = rowValue.CreateCell(4, CellType.NUMERIC);
        //            ICell cellCons2Value = rowValue.CreateCell(5, CellType.NUMERIC);
        //            ICell cellCons4Value = rowValue.CreateCell(6, CellType.NUMERIC);
        //            ICell cellCons6Value = rowValue.CreateCell(7, CellType.NUMERIC);
        //            ICell cellCons8Value = rowValue.CreateCell(8, CellType.NUMERIC);

        //            cellAnioValue.SetCellValue(rain.Period.Year);
        //            cellMesValue.SetCellValue(rain.Period.Month);
        //            cellSemanaValue.SetCellValue(rain.Period.Week);
        //            cellDiaValue.SetCellValue(rain.Period.Day);
        //            cellCantValue.SetCellValue(rain.Quantity);
        //            cellCons2Value.SetCellValue(_list2.ElementAt(i).Consumed);
        //            cellCons4Value.SetCellValue(_list4.ElementAt(i).Consumed);
        //            cellCons6Value.SetCellValue(_list6.ElementAt(i).Consumed);
        //            cellCons8Value.SetCellValue(_list8.ElementAt(i).Consumed);
        //        }

        //        FileStream sw = File.Create(dialog.FileName);
        //        workbook.Write(sw);
        //        sw.Close();
        //    }
        //}        

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
