using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Fuzzy;

namespace FuzzyLogic
{
    public class FuzzyRain
    {
        private InferenceSystem IS;
        private static FuzzyRain instance;

        private FuzzyRain()
        {
            InitFuzzyEngine();
        }

        public static FuzzyRain Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FuzzyRain();
                }
                return instance;
            }
        }

        private void InitFuzzyEngine()
        {
            // RAIN
            FuzzySet fsLow = new FuzzySet("Low", new TrapezoidalFunction(0, 5, 10, 30));
            FuzzySet fsMedium = new FuzzySet("Medium", new TrapezoidalFunction(20, 30, 50, 60));
            FuzzySet fsHigh = new FuzzySet("High", new TrapezoidalFunction(50, 70, 90, 100));
            FuzzySet fsVeryHigh = new FuzzySet("VeryHigh", new TrapezoidalFunction(100, 110, 120, 130));

            LinguisticVariable lvRain = new LinguisticVariable("Rain", 0, 130);
            lvRain.AddLabel(fsLow);
            lvRain.AddLabel(fsMedium);
            lvRain.AddLabel(fsHigh);
            lvRain.AddLabel(fsVeryHigh);

            // SURFACE
            FuzzySet fsSmall = new FuzzySet("Small", new TrapezoidalFunction(0, 5, 20, 30));
            FuzzySet fsMed = new FuzzySet("Med", new TrapezoidalFunction(20, 30, 70, 80));
            FuzzySet fsLarge = new FuzzySet("Large", new TrapezoidalFunction(70, 80, 100, 110));
            FuzzySet fsVeryLarge = new FuzzySet("VeryLarge", new TrapezoidalFunction(100, 110, 140, 150));

            LinguisticVariable lvSurface = new LinguisticVariable("Surface", 0, 150);
            lvSurface.AddLabel(fsSmall);
            lvSurface.AddLabel(fsMed);
            lvSurface.AddLabel(fsLarge);
            lvSurface.AddLabel(fsVeryLarge);


            // VOLUMEN (OUTPUT)
            FuzzySet fs_vol_Small = new FuzzySet("vol_Small", new TrapezoidalFunction(0, 5, 20, 30));
            FuzzySet fs_vol_Med = new FuzzySet("vol_Med", new TrapezoidalFunction(20, 30, 70, 80));
            FuzzySet fs_vol_Large = new FuzzySet("vol_Large", new TrapezoidalFunction(70, 80, 100, 110));
            FuzzySet fs_vol_VeryLarge = new FuzzySet("vol_VeryLarge", new TrapezoidalFunction(100, 110, 140, 200));

            LinguisticVariable lvVolumen = new LinguisticVariable("Volumen", 0, 200);
            lvVolumen.AddLabel(fs_vol_Small);
            lvVolumen.AddLabel(fs_vol_Med);
            lvVolumen.AddLabel(fs_vol_Large);
            lvVolumen.AddLabel(fs_vol_VeryLarge);

            // The database
            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvRain);
            fuzzyDB.AddVariable(lvSurface);
            fuzzyDB.AddVariable(lvVolumen);

            // Creating the inference system
            CentroidDefuzzifier centroide = new CentroidDefuzzifier(1000);
            IS = new InferenceSystem(fuzzyDB, centroide);

            // Going Straight
            IS.NewRule("Rule 1", "IF Rain IS Low AND Surface IS Med THEN Volumen IS vol_Small");
            IS.NewRule("Rule 2", "IF Rain IS High AND Surface IS Large THEN Volumen IS vol_VeryLarge");
            // TODO: define remaiming rules

            float f = DoInference(5, 30);
        }

        public FuzzyRainResult DoInference(float rainAmount)
        {
            IS.SetInput("Rain", rainAmount);
            try
            {
                return new FuzzyRainResult(IS.Evaluate("Surface"), IS.Evaluate("Surface"));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public float DoInference(float rainAmount, float surfAmount)
        {
            var rain = 5;
            var surf = 30;
            IS.SetInput("Surface", surf);
            IS.SetInput("Rain", rain);

            float result = 0;
            try
            {
                result = IS.Evaluate("Volumen");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}