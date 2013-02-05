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
            //*** RAIN ***//
            FuzzySet fs_rain_baja = new FuzzySet("Baja", new TrapezoidalFunction(15, 30, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fs_rain_media = new FuzzySet("Media", new TrapezoidalFunction(20, 30, 50, 60));
            FuzzySet fs_rain_alta = new FuzzySet("Alta", new TrapezoidalFunction(50, 70, 90, 100));
            FuzzySet fs_rain_muyAlta = new FuzzySet("MuyAlta", new TrapezoidalFunction(100, 120, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvRain = new LinguisticVariable("Rain", 0, 130);
            lvRain.AddLabel(fs_rain_baja);
            lvRain.AddLabel(fs_rain_media);
            lvRain.AddLabel(fs_rain_alta);
            lvRain.AddLabel(fs_rain_muyAlta);


            //*** SURFACE ***//
            FuzzySet fs_surface_chica = new FuzzySet("Chica", new TrapezoidalFunction(0, 5, 20, 30));
            FuzzySet fs_surface_mediana = new FuzzySet("Mediana", new TrapezoidalFunction(20, 30, 70, 80));
            FuzzySet fs_surface_grande = new FuzzySet("Grande", new TrapezoidalFunction(70, 80, 100, 110));
            FuzzySet fs_surface_muyGrande = new FuzzySet("MuyGrande", new TrapezoidalFunction(100, 110, 140, 150));

            LinguisticVariable lvSurface = new LinguisticVariable("Surface", 0, 150);
            lvSurface.AddLabel(fs_surface_chica);
            lvSurface.AddLabel(fs_surface_mediana);
            lvSurface.AddLabel(fs_surface_grande);
            lvSurface.AddLabel(fs_surface_muyGrande);


            //*** VOLUMEN (OUTPUT) ***//
            FuzzySet fs_vol_chico = new FuzzySet("Chico", new TrapezoidalFunction(0, 5, 20, 30));
            FuzzySet fs_vol_mediano = new FuzzySet("Mediano", new TrapezoidalFunction(20, 30, 70, 80));
            FuzzySet fs_vol_grande = new FuzzySet("Grande", new TrapezoidalFunction(70, 80, 100, 110));
            FuzzySet fs_vol_muyGrande = new FuzzySet("MuyGrande", new TrapezoidalFunction(100, 110, 140, 200));

            LinguisticVariable lvVolumen = new LinguisticVariable("Volumen", 0, 200);
            lvVolumen.AddLabel(fs_vol_chico);
            lvVolumen.AddLabel(fs_vol_mediano);
            lvVolumen.AddLabel(fs_vol_grande);
            lvVolumen.AddLabel(fs_vol_muyGrande);


            // The database
            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvRain);
            fuzzyDB.AddVariable(lvSurface);
            fuzzyDB.AddVariable(lvVolumen);

            // Creating the inference system
            CentroidDefuzzifier centroide = new CentroidDefuzzifier(1000);
            IS = new InferenceSystem(fuzzyDB, centroide);
            
            ///*** RULES ***//
            // Rain Baja
            IS.NewRule("Rule 1", IF_IS("Rain", "Baja") + AND_IS("Surface", "Chica") + THEN_IS("Volumen", "Chico"));
            IS.NewRule("Rule 2", IF_IS("Rain", "Baja") + AND_IS("Surface", "Mediana") + THEN_IS("Volumen", "Chico"));
            IS.NewRule("Rule 3", IF_IS("Rain", "Baja") + AND_IS("Surface", "Grande") + THEN_IS("Volumen", "Chico"));
            IS.NewRule("Rule 4", IF_IS("Rain", "Baja") + AND_IS("Surface", "MuyGrande") + THEN_IS("Volumen", "Chico"));
            // Rain Media
            IS.NewRule("Rule 5", IF_IS("Rain", "Media") + AND_IS("Surface", "Chica") + THEN_IS("Volumen", "Chico"));
            IS.NewRule("Rule 6", IF_IS("Rain", "Media") + AND_IS("Surface", "Mediana") + THEN_IS("Volumen", "Mediano"));
            IS.NewRule("Rule 7", IF_IS("Rain", "Media") + AND_IS("Surface", "Grande") + THEN_IS("Volumen", "Grande"));
            IS.NewRule("Rule 8", IF_IS("Rain", "Media") + AND_IS("Surface", "MuyGrande") + THEN_IS("Volumen", "Grande"));
            // Rain Alta
            IS.NewRule("Rule 9", IF_IS("Rain", "Alta") + AND_IS("Surface", "Chica") + THEN_IS("Volumen", "Chico"));
            IS.NewRule("Rule 10", IF_IS("Rain", "Alta") + AND_IS("Surface", "Mediana") + THEN_IS("Volumen", "Mediano"));
            IS.NewRule("Rule 11", IF_IS("Rain", "Alta") + AND_IS("Surface", "Grande") + THEN_IS("Volumen", "Grande"));
            IS.NewRule("Rule 12", IF_IS("Rain", "Alta") + AND_IS("Surface", "MuyGrande") + THEN_IS("Volumen", "MuyGrande"));
            // Rain MuyAlta
            IS.NewRule("Rule 13", IF_IS("Rain", "MuyAlta") + AND_IS("Surface", "Chica") + THEN_IS("Volumen", "Chico"));
            IS.NewRule("Rule 14", IF_IS("Rain", "MuyAlta") + AND_IS("Surface", "Mediana") + THEN_IS("Volumen", "Mediano"));
            IS.NewRule("Rule 15", IF_IS("Rain", "MuyAlta") + AND_IS("Surface", "Grande") + THEN_IS("Volumen", "MuyGrande"));
            IS.NewRule("Rule 16", IF_IS("Rain", "MuyAlta") + AND_IS("Surface", "MuyGrande") + THEN_IS("Volumen", "MuyGrande"));

             //float f = DoInference(5, 30);
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
            //var rain = 5;
            //var surf = 30;
            IS.SetInput("Rain", rainAmount);
            IS.SetInput("Surface", surfAmount);

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

        private string IF_IS(string variable, string value)
        {
            return string.Format("IF {0} IS {1} ", variable, value);
        }

        private string AND_IS(string variable, string value)
        {
            return string.Format(" AND {0} IS {1} ", variable, value);
        }

        private string THEN_IS(string variable, string value)
        {
            return string.Format(" THEN {0} IS {1} ", variable, value);
        }
    }
}