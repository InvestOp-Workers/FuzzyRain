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
            myDistribution = new Distribution(myModel.MyDistribution.SimulationData, int.Parse(myModel.MyDistribution.Month));
            eventsCount = numberOfEvents;

            ucOutputData.CleanData();
            ucOutputData.CleanDataList();
            ucOutputData.SetConvergenceData(model.ConvergenceAvg, model.ConvergenceDesv, model.ConvergenceValue);
            //TODO: borrar esto. Es solo para corroborar los valores inferidos.
            checkearInferencias.Items.Clear();
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
            checkearInferencias.Items.Clear();
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

            double surface = myDistribution.SimulationData.Surface;
            ImageSuperficie.Width = surface;
            ImageSuperficie.Height = surface;
            LabelSuperficie.Content = surface.ToString("00.00") + "mm";

            double volumen = myDistribution.SimulationData.Volumen;
            ImageVolumen.Width = volumen;
            ImageVolumen.Height = volumen;
            LabelVolumen.Content = volumen.ToString("00.00") + "mm";
            
            // TODO: actualizar este dato en una grafica. Podrían imprimirse en pantalla la lista de valores tambien.
            float consumo = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 2);
            float consumo4 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 4);
            float consumo6 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 6);
            float consumo8 = FuzzyLogic.FuzzyRain.Instance.DoInference((float)surface, (float)volumen, (float)rain, 8);
            //TODO: borrar esto. Es solo para corroborar los valores inferidos.
            checkearInferencias.Items.Add(consumo);
            checkearInferencias4.Items.Add(consumo4);
            checkearInferencias6.Items.Add(consumo6);
            checkearInferencias8.Items.Add(consumo8);
        }
    }
}
