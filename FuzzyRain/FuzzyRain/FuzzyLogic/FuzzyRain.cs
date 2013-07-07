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
            //Superficie Captacion SC
            FuzzySet scChico = new FuzzySet("Chico", new TrapezoidalFunction(81, 120, TrapezoidalFunction.EdgeType.Right));
            FuzzySet scMediano = new FuzzySet("Mediano", new TrapezoidalFunction(80, 120, 121, 180));
            FuzzySet scGrande = new FuzzySet("Grande", new TrapezoidalFunction(150, 200, 201, 250));
            FuzzySet scMuyGrande = new FuzzySet("MuyGrande", new TrapezoidalFunction(200, 250, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvSuperficieCaptacion = new LinguisticVariable("SC", 0, 1200);
            lvSuperficieCaptacion.AddLabel(scChico);
            lvSuperficieCaptacion.AddLabel(scMediano);
            lvSuperficieCaptacion.AddLabel(scGrande);
            lvSuperficieCaptacion.AddLabel(scMuyGrande);

            //volumen del sistema de almacenamiento VA
            FuzzySet vaPequenio = new FuzzySet("Pequenio", new TrapezoidalFunction(11, 20, TrapezoidalFunction.EdgeType.Right));
            FuzzySet vaIntermedio = new FuzzySet("Intermedio", new TrapezoidalFunction(10, 20, 21, 40));
            FuzzySet vaConsiderable = new FuzzySet("Considerable", new TrapezoidalFunction(30, 40, TrapezoidalFunction.EdgeType.Left));
            
            LinguisticVariable lvVolumenAlmacenamiento = new LinguisticVariable("VA", 0, 100);
            lvVolumenAlmacenamiento.AddLabel(vaPequenio);
            lvVolumenAlmacenamiento.AddLabel(vaIntermedio);
            lvVolumenAlmacenamiento.AddLabel(vaConsiderable);

            //precipitaciones pluviales PP
            FuzzySet ppMuyBaja = new FuzzySet("MuyBaja", new TrapezoidalFunction(20, 40, TrapezoidalFunction.EdgeType.Right));
            FuzzySet ppBaja = new FuzzySet("Baja", new TrapezoidalFunction(20, 40, 41, 60));
            FuzzySet ppMedia = new FuzzySet("Media", new TrapezoidalFunction(40, 70, 71, 100));
            FuzzySet ppAlta = new FuzzySet("Alta", new TrapezoidalFunction(80, 130, 131, 180));
            FuzzySet ppMuyAlta = new FuzzySet("MuyAlta", new TrapezoidalFunction(150, 180, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvPrecipitacionesPluviales = new LinguisticVariable("PP", 0, 300);
            lvPrecipitacionesPluviales.AddLabel(ppMuyBaja);
            lvPrecipitacionesPluviales.AddLabel(ppBaja);
            lvPrecipitacionesPluviales.AddLabel(ppMedia);
            lvPrecipitacionesPluviales.AddLabel(ppAlta);
            lvPrecipitacionesPluviales.AddLabel(ppMuyAlta);
            
            //consumo C (OUTPUT)
            FuzzySet cBajo = new FuzzySet("Bajo", new TrapezoidalFunction(1000, 1200, TrapezoidalFunction.EdgeType.Right));
            FuzzySet cPromedio = new FuzzySet("Promedio", new TrapezoidalFunction(1200, 1400, 1401, 1800));
            FuzzySet dAlto = new FuzzySet("Alto", new TrapezoidalFunction(1400, 1800, TrapezoidalFunction.EdgeType.Left));
            
            LinguisticVariable lvConsumo = new LinguisticVariable("C", 0, 3000);
            lvConsumo.AddLabel(cBajo);
            lvConsumo.AddLabel(cPromedio);
            lvConsumo.AddLabel(dAlto);

            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvSuperficieCaptacion);
            fuzzyDB.AddVariable(lvVolumenAlmacenamiento);
            fuzzyDB.AddVariable(lvPrecipitacionesPluviales);
            fuzzyDB.AddVariable(lvConsumo);

            CentroidDefuzzifier centroide = new CentroidDefuzzifier(1000);
            IS = new InferenceSystem(fuzzyDB, centroide);

            // SC Chico
            IS.NewRule("Rule 1", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 2", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 3", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Media") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 4", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 5", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 6", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 7", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 8", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Media") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 9", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 10", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 11", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 12", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 13", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "Media") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 14", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 15", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));

            // SC Mediano
            IS.NewRule("Rule 16", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 17", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 18", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 19", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 20", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 21", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 22", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 23", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 24", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 25", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 26", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 27", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 28", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 29", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 30", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));

            // SC Grande
            IS.NewRule("Rule 31", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 32", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 33", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 34", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 35", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 36", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 37", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 38", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 39", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 40", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 41", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 42", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 43", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Media") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 44", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 45", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));

            // SC MuyGrande
            IS.NewRule("Rule 46", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 47", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 48", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 49", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 50", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 51", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 52", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS.NewRule("Rule 53", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Media") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 54", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 55", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 56", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS.NewRule("Rule 57", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Baja") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 58", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Media") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 59", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS.NewRule("Rule 60", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));
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

        public float DoInference(float sc, float va, float pp)
        {
            IS.SetInput("SC", sc);
            IS.SetInput("VA", va);
            IS.SetInput("PP", pp);

            float result = 0;
            try
            {
                result = IS.Evaluate("C");
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