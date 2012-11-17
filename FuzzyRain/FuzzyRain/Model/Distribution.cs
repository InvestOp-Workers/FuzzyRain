using System.IO;
using System.Collections.Generic;
using FuzzyRain.Model;

public class Distribution
{
    public string Name { get; set; }
    public int RankCount 
    {
        get
        {
            if (Ranks == null)
                return 0;

            return Ranks.Length;
        }
    }
    public Rank[] Ranks { get; set; }
    public IList<double> ValuesInOrderOfAppearance = new List<double>();

    public double Average
    {
        get
        {
            return StatisticalMetrics.GetAverage(ValuesInOrderOfAppearance);
        }
    }

    public double Std_Desv
    {
        get
        {
            return StatisticalMetrics.GetDesv(ValuesInOrderOfAppearance);
        }
    }

    public Distribution()
    {
        ValuesInOrderOfAppearance = new List<double>();        
    }

    public bool PutValueInRank(double value, SimulationType simulationType)
    {
        var valueToAdd = GetValueAccordingSimulationType(value, simulationType);

        for(int i = 0; i < RankCount; i++)
        {
            if (valueToAdd <= Ranks[i].UpperLimit)
            {                
                AddValueInRank(i, valueToAdd, simulationType);                
                return true;
            }
        }

        return false;
    }

    public double PutValueInRankUsingFrequency(double value)
    {        
        double valueToReturn = 0.0;

        for (int i = 0; i < RankCount; i++)
        {
            Rank currentRank = Ranks[i];
            if (value <= currentRank.CumFrequency)
            {
                // TODO: el valor central no debiera ser tomado en cuenta, sino que debieramos obtener un valor mas preciso matematicamente 
                //calculando proporcionalmente.
                valueToReturn = currentRank.CenterValueOfRank;

                currentRank.Values.Add(valueToReturn);
                AddValueInOrderOfAppearance(valueToReturn);
                return valueToReturn;
            }
        }

        return valueToReturn;
    }    

    public void AddValueInOrderOfAppearance(double value, SimulationType simulationType)
    {
        // EXPLICACION DE ESTE CODIGO: los datos de entrada son siempre mensuales (así está definido el formato de entrada), entonces, si la simulacion 
        // será diaria, debe dividerse cada dato mensual por 30 e ingresarlo como un suceso 30 veces para luego calcular la media y el desvio standard de 
        // manera adecuada.
        // Similar sería si se pretende una simulación semanal, deberia dividirse el dato ingresado por 4 e ingresarlo como que ocurrió en 4 oportunidades.

        var valueToAdd = GetValueAccordingSimulationType(value, simulationType);

        for (int i = 1; i <= (int)simulationType; i++)
        {
            AddValueInOrderOfAppearance(valueToAdd);
        }
    }

    public void AddValueInOrderOfAppearance(double value)
    {
        ValuesInOrderOfAppearance.Add(value);
    }

    private double GetValueAccordingSimulationType(double value, SimulationType simulationType)
    { 
        return value / (int)simulationType;
    }

    private void AddValueAccordingSimulationType(IList<double> list, double value, SimulationType simulationType)
    {
        for (int i = 1; i <= (int)simulationType; i++)
        {
            list.Add(value);
        }
    }

    private void AddValueInRank(int indexRank, double value, SimulationType simulationType)
    {
        for (int i = 1; i <= (int)simulationType; i++)
        {
            Ranks[indexRank].Values.Add(value);
            AddValueInOrderOfAppearance(value);
        }

        double allValuesCount = (double)ValuesInOrderOfAppearance.Count;

        for (int k = 0; k < RankCount; k++)
        {
            if (k != 0)
            {
                Ranks[k].CumFrequency = Ranks[k - 1].CumFrequency + (double)Ranks[k].Values.Count / allValuesCount;
            }
            else
            {
                Ranks[k].CumFrequency = (double)Ranks[k].Values.Count / allValuesCount;                
            }
        }

        //double cumulative = 0;
        //for (int j = 0; j < indexRank; j++)
        //{
        //    cumulative = cumulative + Ranks[j].CumFrequency;
        //}

        //double frequencyInThisRank = (double)Ranks[indexRank].Values.Count / (double)ValuesInOrderOfAppearance.Count;

        //Ranks[indexRank].CumFrequency = cumulative + frequencyInThisRank;
    }
}