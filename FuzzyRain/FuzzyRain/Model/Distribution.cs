using System.IO;
using System.Collections.Generic;
using FuzzyRain.Model;

public class Distribution
{
    public string Name { get; set; }
    public int RankCount { get; set; }
    public Rank[] Ranks { get; set; }
    public List<double> ValuesInOrderOfAppearance = new List<double>();

    public Distribution()
    {
        ValuesInOrderOfAppearance = new List<double>();        
    }
    
    public bool PutValueInRank(double value)
    {
        AddValueInOrderOfAppearance(value);

        for(int i = 0; i < RankCount; i++)
        {
            if (value <= Ranks[i].UpperLimit)
            {
                Ranks[i].Values.Add(value);
                return true;
            }
        }

        return false;
    }

    public double PutValueInRankUsingFrequency(double value)
    {
        AddValueInOrderOfAppearance(value);        

        double valueToReturn = 0.0;

        for (int i = 0; i < RankCount; i++)
        {
            Rank currentRank = Ranks[i];
            if (value <= currentRank.CumFrequency)
            {
                valueToReturn = currentRank.CenterValueOfRank;
                currentRank.Values.Add(valueToReturn);
                return valueToReturn;
            }
        }

        return valueToReturn;
    }

    public void AddValueInOrderOfAppearance(double value)
    {
        ValuesInOrderOfAppearance.Add(value);
    }
}