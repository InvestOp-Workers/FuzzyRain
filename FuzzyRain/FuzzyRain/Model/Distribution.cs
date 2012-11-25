using System.IO;
using System.Collections.Generic;
using FuzzyRain.Model;

public class Distribution
{
    private const int RANK_PRECISENESS = 40;
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
                //valueToReturn = currentRank.CenterValueOfRank;

                if (i == 0)
                {
                    valueToReturn = ObtainValueInRank(currentRank.LowerLimit, currentRank.UpperLimit,
                        ObtainPlaceInRank(0, currentRank.CumFrequency, value));
                }
                else
                {
                    valueToReturn = ObtainValueInRank(currentRank.LowerLimit, currentRank.UpperLimit,
                        ObtainPlaceInRank(Ranks[i - 1].CumFrequency, currentRank.CumFrequency, value));
                }
                
                currentRank.Values.Add(valueToReturn);
                AddValueInOrderOfAppearance(valueToReturn);
                return valueToReturn;
            }
        }

        return valueToReturn;
    }

    private int ObtainPlaceInRank(double valueInf, double valueSup, double value)
    {
        double rankAmplitude = valueSup - valueInf;
        double rankPortion = rankAmplitude / RANK_PRECISENESS;

        int valueToReturn = 0;
        for (int i = 1; i <= RANK_PRECISENESS; i++)
        {
            if (value <= valueInf + rankPortion * i)
            {
                valueToReturn = i;
                break;
            }
        }

        return valueToReturn;
    }

    private double ObtainValueInRank(double valueInf, double valueSup, int placeInRank)
    {
        double rankAmplitude = valueSup - valueInf;
        double rankPortion = rankAmplitude / RANK_PRECISENESS;

        return valueInf + rankPortion * placeInRank;        
    }

    public void AddValueInOrderOfAppearance(double value)
    {
        ValuesInOrderOfAppearance.Add(value);
    }

    private double GetValueAccordingSimulationType(double value, SimulationType simulationType)
    { 
        return value / (int)simulationType;
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
    }    
}