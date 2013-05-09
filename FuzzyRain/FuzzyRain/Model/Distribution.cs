using System.IO;
using System.Collections.Generic;
using FuzzyRain.Model;
using System.Linq;

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
    public IList<Rain> ValuesInOrderOfAppearance = new List<Rain>();
    public SimulationType SimulationType { get; set; }

    public double Average
    {
        get
        {
            return StatisticalMetrics.GetAverage(ValuesInOrderOfAppearance.Select(x => x.Quantity).ToList());
        }
    }

    public double Std_Desv
    {
        get
        {
            return StatisticalMetrics.GetDesv(ValuesInOrderOfAppearance.Select(x => x.Quantity).ToList());
        }
    }

    public Distribution(SimulationType simulationType)
    {
        ValuesInOrderOfAppearance = new List<Rain>();
        this.SimulationType = simulationType;
    }

    public void CreateRanks(int rankCount, int rankAmplitude)
    {
        Ranks = new Rank[rankCount];
        for (int i = 0; i < rankCount; i++)
        {
            Ranks[i] = new Rank(i * rankAmplitude, i * rankAmplitude + rankAmplitude);
        }        
    }

    public bool PutValueInRank(double value)
    {
        var valueToAdd = GetValueAccordingSimulationType(value);

        for(int i = 0; i < RankCount; i++)
        {
            if (valueToAdd <= Ranks[i].UpperLimit)
            {                
                AddValueInRank(i, valueToAdd);                
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

                currentRank.Values.Add(new Rain() { Quantity = valueToReturn });
                AddValueInOrderOfAppearance(valueToReturn, 0);
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

    private string GetPeriod(int indexPeriod)
    {
        var count = ValuesInOrderOfAppearance.Count;
        string period = "";
        
        if (SimulationType == FuzzyRain.Model.SimulationType.Daily)
        {
            int dia = count % (int)FuzzyRain.Model.SimulationType.Daily;
            int anio = count / (int)FuzzyRain.Model.SimulationType.Daily;

            dia++;            
            anio++;
            period = anio + "° año - " + dia + "° dia";
        }

        if (SimulationType == FuzzyRain.Model.SimulationType.Weekly)
        {
            int semana = count % (int)FuzzyRain.Model.SimulationType.Weekly;
            int anio = count / (int)FuzzyRain.Model.SimulationType.Weekly;

            semana++;           
            anio++;
            period = anio + "° año - " + semana + "° semana";
        }

        if (SimulationType == FuzzyRain.Model.SimulationType.Monthly)
        {            
            int anio = count / (int)FuzzyRain.Model.SimulationType.Monthly;
            
            anio++;
            period = anio + "° año";
        }        

        return period;
    }

    public Rain AddValueInOrderOfAppearance(double value, int indexPeriod)
    {
        Rain rain = new Rain() { Period = GetPeriod(indexPeriod), Quantity = value };
        ValuesInOrderOfAppearance.Add(rain);
        return rain;
    }

    private double GetValueAccordingSimulationType(double value)
    { 
        return value / (int)SimulationType;
    }

    private void AddValueInRank(int indexRank, double value)
    {
        for (int i = 1; i <= (int)SimulationType; i++)
        {            
            Ranks[indexRank].Values.Add(new Rain() { Period = GetPeriod(i), Quantity = value });
            AddValueInOrderOfAppearance(value, i);
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