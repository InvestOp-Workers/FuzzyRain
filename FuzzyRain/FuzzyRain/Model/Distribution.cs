using System.Collections.Generic;
using System.IO;

public class Distribution
{
    public string Name { get; set; }
    public int RankCount { get; set; }
    public Rank[] Ranks { get; set; }
    
    public double Average 
    {
        get 
        {            
            int elementsCount = 0;
            double summary = 0;
            for (int i = 0; i < RankCount; i++)
            {
                foreach (double value in Ranks[i].Values)
                {
                    summary = summary + value;
                    elementsCount++;
                }
            }

            return summary / elementsCount;
        }
    }

    public int GetRank(double value)
    {
        for(int i = 0; i < RankCount; i++)
        {
            if (value <= Ranks[i].UpperLimit)
                return i;
        }

        return RankCount - 1;
    }

    public bool PutValueInRank(double value)
    {
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
}