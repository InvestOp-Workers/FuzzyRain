using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLogic
{
    public class FuzzyRainResult
    {
        public FuzzyRainResult(float surface, float volume)
        {
            this.Surface = surface;
            this.Volume = volume;
        }
        public double Surface { get; set; }
        public double Volume { get; set; }
    }
}
