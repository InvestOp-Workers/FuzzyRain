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

namespace FuzzyRain
{
    /// <summary>
    /// Interaction logic for MonthTab.xaml
    /// </summary>
    public partial class MonthTab : UserControl
    {
        public string Title 
        {
            set { txtTitle.Text = value; }
        }

        public MonthTab()
        {
            InitializeComponent();
        }

        public void SetDataMonths(Distribution[] distributions)
        {
            for (int month = 10; month <= 12; month++)
            {
                double avg = distributions[month].Average;
                double desv = distributions[month].Std_Desv;
                
                ((ListView)tabMonths.FindName("values_" + month)).ItemsSource = distributions[month].ValuesInOrderOfAppearance;
                ((TextBlock)tabMonths.FindName("avg_" + month)).Text = avg.ToString("#0.00");
                ((TextBlock)tabMonths.FindName("desv_" + month)).Text = desv.ToString("#0.00");                    
            }
        }

        public void SetConvergenceData(double avg, double desv, int eventNumberOfConvergence, int month)
        {            
            ((TextBlock)tabMonths.FindName("valueConv_" + month)).Text = eventNumberOfConvergence.ToString();
            ((TextBlock)tabMonths.FindName("avgConv_" + month)).Text = avg.ToString("#0.00");
            ((TextBlock)tabMonths.FindName("desvConv_" + month)).Text = desv.ToString("#0.00");

            ((StackPanel)tabMonths.FindName("valueConvPanel_" + month)).Visibility = System.Windows.Visibility.Visible;
            ((StackPanel)tabMonths.FindName("avgConvPanel_" + month)).Visibility = System.Windows.Visibility.Visible;
            ((StackPanel)tabMonths.FindName("desvConvPanel_" + month)).Visibility = System.Windows.Visibility.Visible;            
        }
    }
}
