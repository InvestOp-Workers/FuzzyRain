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

namespace FuzzyRain
{
    /// <summary>
    /// Interaction logic for MonthTab.xaml
    /// </summary>
    public partial class MonthTabItemContent : UserControl
    {
        private MonteCarloWithRanks myModel;
        private Distribution myDistribution;
        int eventsCount;

        public MonthTabItemContent()
        {
            InitializeComponent();            
        }

        public void SetInitialInputData(Distribution distribution)
        {
            ucInputData.CleanData();
            ucInputData.SetInitialData(distribution);
        }

        public void SetInitialOutputData(MonteCarloWithRanks model, int numberOfEvents)
        {
            myModel = model;
            myDistribution = new Distribution(myModel.MyDistribution.SimulationType, int.Parse(myModel.MyDistribution.Month));
            eventsCount = numberOfEvents;

            ucOutputData.CleanData();
            ucOutputData.CleanDataList();
            ucOutputData.SetConvergenceData(model.ConvergenceAvg, model.ConvergenceDesv, model.ConvergenceValue);
        }

        /// <summary>
        /// Cleans output data control
        /// </summary>
        public void CleanInitialData()
        {
            ucInputData.CleanData();
            ucOutputData.CleanData();
            ucOutputData.CleanDataList();
        }

        public void Tick()
        {
            if (eventsCount == 0)
            {
                ucOutputData.Finalize(myDistribution);
                eventsCount -= 1;
                return;
            }

            if (eventsCount < 0)
                return;
                
            eventsCount -= 1;

            double rain = myModel.NextValue();            
            ucOutputData.AddNewSimulatedItem(myDistribution.AddValueInOrderOfAppearance(rain, eventsCount));

            ImageLLuvia.Width = rain;
            ImageLLuvia.Height = rain;
            LabelLLuvia.Content = rain.ToString("00.00") + "mm";

            double surface = 100;
            ImageSuperficie.Width = surface;
            ImageSuperficie.Height = surface;
            LabelSuperficie.Content = surface.ToString("00.00") + "mm";

            double volumen = FuzzyLogic.FuzzyRain.Instance.DoInference((float)rain, (float)surface);            
            ImageVolumen.Width = volumen;
            ImageVolumen.Height = volumen;
            LabelVolumen.Content = volumen.ToString("00.00") + "mm";
        }
    }
}
