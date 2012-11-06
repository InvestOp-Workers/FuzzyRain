using System.Collections.Generic;

public class Rank
{
    public double LowerLimit { get; set; }
    public double UpperLimit { get; set; }
    public double CumFrequency { get; set; }
    public double CenterValueOfRank { get; set; }
    
    public IList<double> Values  { get; set; }

    public Rank(double lowerLimit, double upperLimit)
    {
        Values = new List<double>();

        LowerLimit = lowerLimit;
        UpperLimit = upperLimit;
        CenterValueOfRank = (upperLimit + lowerLimit) / 2;
    }
}