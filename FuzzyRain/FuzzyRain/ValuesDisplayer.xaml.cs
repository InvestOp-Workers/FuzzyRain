using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using SimulationMethods;

namespace FuzzyRain
{
    /// <summary>
    /// Interaction logic for MonthTab.xaml
    /// </summary>
    public partial class ValuesDisplayer : UserControl
    {
        public string Title
        {
            set { contentTitle.Text = value; }
        }

        public ValuesDisplayer()
        {
            InitializeComponent();
        }

        public void SetInitialData(Distribution distribution)
        {
            double avg = distribution.Average;
            double desv = distribution.Std_Desv;
            
            lwValues.ItemsSource = distribution.ValuesInOrderOfAppearance;            
            txtAvg.Text = avg.ToString("#0.00");
            txtDesv.Text = desv.ToString("#0.00");
        }

        public void SetConvergenceData(double avg, double desv, int eventNumberOfConvergence)
        {
            //txtValueConv.Text = eventNumberOfConvergence.ToString();
            txtAvgConv.Text = avg.ToString("#0.00");
            txtDesvConv.Text = desv.ToString("#0.00");

            //valueConvPanel.Visibility = System.Windows.Visibility.Visible;
            avgConvPanel.Visibility = System.Windows.Visibility.Visible;
            desvConvPanel.Visibility = System.Windows.Visibility.Visible;
        }

        public void AddNewSimulatedItem(Rain rain)
        {            
            lwValues.Items.Add(rain);
        }

        public void Finalize(Distribution distribution)
        {
            txtAvg.Text = distribution.Average.ToString("#0.00");
            txtDesv.Text = distribution.Std_Desv.ToString("#0.00");
        }

        public void CleanDataList()
        {            
            lwValues.Items.Clear();
        }

        public void CleanData()
        {     
            lwValues.ItemsSource = null;

            //txtValueConv.Text = string.Empty;
            txtAvgConv.Text = string.Empty;
            txtDesvConv.Text = string.Empty;

            txtAvg.Text = string.Empty;
            txtDesv.Text = string.Empty;
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (lwValues.Items.Count > 0)
            {                
                var sb = new StringBuilder();
                sb.AppendLine("Año\tSemana\tDia\tCantidad");
                foreach (Rain rain in lwValues.Items)
                {
                    sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}", rain.Period.Year, rain.Period.Week, rain.Period.Day, rain.Quantity));
                }
                try
                {
                    System.Windows.Clipboard.SetData(DataFormats.Text, sb.ToString());
                }
                catch (COMException)
                {
                    MessageBox.Show("No se pudo copiar la información");
                }
            }
        }
    }
}
