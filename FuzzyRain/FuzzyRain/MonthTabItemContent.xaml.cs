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

namespace FuzzyRain
{
    /// <summary>
    /// Interaction logic for MonthTab.xaml
    /// </summary>
    public partial class MonthTabItemContent : UserControl
    {
        public MonthTabItemContent()
        {
            InitializeComponent();
        }

        public void SetDataMonths(Distribution distribution, bool IsInputData)
        {
            if (IsInputData)
            {
                ucInputData.SetDataMonths(distribution);
            }
            else
            {
                ucOutputData.SetDataMonths(distribution);
            }
        }

        public void SetConvergenceData(double avg, double desv, int eventNumberOfConvergence)
        {
            ucOutputData.SetConvergenceData(avg, desv, eventNumberOfConvergence);
        }

        public void Tick()
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
    }
}
