using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Fuzzy;

namespace FuzzyLogic
{
    public class FuzzyRain
    {
        private InferenceSystem[] IS;
        private static FuzzyRain instance;

        private FuzzyRain()
        {
            IS = new InferenceSystem[4];
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
            InitFuzzyEngineFor2Person();
            InitFuzzyEngineFor4Person();
            InitFuzzyEngineFor6Person();
            InitFuzzyEngineFor8Person();
        }

        private void InitFuzzyEngineFor2Person()
        {
            //Superficie Captacion SC
            FuzzySet scChico = new FuzzySet("Chico", new TrapezoidalFunction(80, 120, TrapezoidalFunction.EdgeType.Right));
            FuzzySet scMediano = new FuzzySet("Mediano", new TrapezoidalFunction(80, 120, 180));
            FuzzySet scGrande = new FuzzySet("Grande", new TrapezoidalFunction(150, 200, 250));
            FuzzySet scMuyGrande = new FuzzySet("MuyGrande", new TrapezoidalFunction(200, 250, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvSuperficieCaptacion = new LinguisticVariable("SC", 0, 1200);
            lvSuperficieCaptacion.AddLabel(scChico);
            lvSuperficieCaptacion.AddLabel(scMediano);
            lvSuperficieCaptacion.AddLabel(scGrande);
            lvSuperficieCaptacion.AddLabel(scMuyGrande);

            //volumen del sistema de almacenamiento VA
            FuzzySet vaPequenio = new FuzzySet("Pequenio", new TrapezoidalFunction(10, 20, TrapezoidalFunction.EdgeType.Right));
            FuzzySet vaIntermedio = new FuzzySet("Intermedio", new TrapezoidalFunction(10, 20, 40));
            FuzzySet vaConsiderable = new FuzzySet("Considerable", new TrapezoidalFunction(30, 40, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvVolumenAlmacenamiento = new LinguisticVariable("VA", 0, 100);
            lvVolumenAlmacenamiento.AddLabel(vaPequenio);
            lvVolumenAlmacenamiento.AddLabel(vaIntermedio);
            lvVolumenAlmacenamiento.AddLabel(vaConsiderable);

            //precipitaciones pluviales PP
            FuzzySet ppMuyBaja = new FuzzySet("MuyBaja", new TrapezoidalFunction(20, 40, TrapezoidalFunction.EdgeType.Right));
            FuzzySet ppBaja = new FuzzySet("Baja", new TrapezoidalFunction(20, 40, 60));
            FuzzySet ppMedia = new FuzzySet("Media", new TrapezoidalFunction(40, 70, 100));
            FuzzySet ppAlta = new FuzzySet("Alta", new TrapezoidalFunction(80, 130, 180));
            FuzzySet ppMuyAlta = new FuzzySet("MuyAlta", new TrapezoidalFunction(150, 180, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvPrecipitacionesPluviales = new LinguisticVariable("PP", 0, 300);
            lvPrecipitacionesPluviales.AddLabel(ppMuyBaja);
            lvPrecipitacionesPluviales.AddLabel(ppBaja);
            lvPrecipitacionesPluviales.AddLabel(ppMedia);
            lvPrecipitacionesPluviales.AddLabel(ppAlta);
            lvPrecipitacionesPluviales.AddLabel(ppMuyAlta);

            //consumo C (OUTPUT)
            FuzzySet cBajo = new FuzzySet("Bajo", new TrapezoidalFunction(1000, 1200, TrapezoidalFunction.EdgeType.Right));
            FuzzySet cPromedio = new FuzzySet("Promedio", new TrapezoidalFunction(1200, 1400, 1800));
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
            SetInferenceSystemAndRules(GetInferenceSystemIndex(2), fuzzyDB, centroide);
        }

        private void InitFuzzyEngineFor4Person()
        {
            //Superficie Captacion SC
            FuzzySet scChico = new FuzzySet("Chico", new TrapezoidalFunction(100, 200, TrapezoidalFunction.EdgeType.Right));
            FuzzySet scMediano = new FuzzySet("Mediano", new TrapezoidalFunction(100, 200, 300));
            FuzzySet scGrande = new FuzzySet("Grande", new TrapezoidalFunction(200, 400, 600));
            FuzzySet scMuyGrande = new FuzzySet("MuyGrande", new TrapezoidalFunction(400, 600, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvSuperficieCaptacion = new LinguisticVariable("SC", 0, 1200);
            lvSuperficieCaptacion.AddLabel(scChico);
            lvSuperficieCaptacion.AddLabel(scMediano);
            lvSuperficieCaptacion.AddLabel(scGrande);
            lvSuperficieCaptacion.AddLabel(scMuyGrande);

            //volumen del sistema de almacenamiento VA
            FuzzySet vaPequenio = new FuzzySet("Pequenio", new TrapezoidalFunction(10, 20, TrapezoidalFunction.EdgeType.Right));
            FuzzySet vaIntermedio = new FuzzySet("Intermedio", new TrapezoidalFunction(10, 30, 50));
            FuzzySet vaConsiderable = new FuzzySet("Considerable", new TrapezoidalFunction(30, 50, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvVolumenAlmacenamiento = new LinguisticVariable("VA", 0, 100);
            lvVolumenAlmacenamiento.AddLabel(vaPequenio);
            lvVolumenAlmacenamiento.AddLabel(vaIntermedio);
            lvVolumenAlmacenamiento.AddLabel(vaConsiderable);

            //precipitaciones pluviales PP
            FuzzySet ppMuyBaja = new FuzzySet("MuyBaja", new TrapezoidalFunction(20, 40, TrapezoidalFunction.EdgeType.Right));
            FuzzySet ppBaja = new FuzzySet("Baja", new TrapezoidalFunction(20, 40, 60));
            FuzzySet ppMedia = new FuzzySet("Media", new TrapezoidalFunction(40, 70, 100));
            FuzzySet ppAlta = new FuzzySet("Alta", new TrapezoidalFunction(80, 130, 180));
            FuzzySet ppMuyAlta = new FuzzySet("MuyAlta", new TrapezoidalFunction(150, 180, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvPrecipitacionesPluviales = new LinguisticVariable("PP", 0, 300);
            lvPrecipitacionesPluviales.AddLabel(ppMuyBaja);
            lvPrecipitacionesPluviales.AddLabel(ppBaja);
            lvPrecipitacionesPluviales.AddLabel(ppMedia);
            lvPrecipitacionesPluviales.AddLabel(ppAlta);
            lvPrecipitacionesPluviales.AddLabel(ppMuyAlta);

            //consumo C (OUTPUT)
            FuzzySet cBajo = new FuzzySet("Bajo", new TrapezoidalFunction(1800, 2200, TrapezoidalFunction.EdgeType.Right));
            FuzzySet cPromedio = new FuzzySet("Promedio", new TrapezoidalFunction(2000, 2400, 2800));
            FuzzySet dAlto = new FuzzySet("Alto", new TrapezoidalFunction(2600, 3000, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvConsumo = new LinguisticVariable("C", 0, 4000);
            lvConsumo.AddLabel(cBajo);
            lvConsumo.AddLabel(cPromedio);
            lvConsumo.AddLabel(dAlto);

            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvSuperficieCaptacion);
            fuzzyDB.AddVariable(lvVolumenAlmacenamiento);
            fuzzyDB.AddVariable(lvPrecipitacionesPluviales);
            fuzzyDB.AddVariable(lvConsumo);

            CentroidDefuzzifier centroide = new CentroidDefuzzifier(1000);
            SetInferenceSystemAndRules(GetInferenceSystemIndex(4), fuzzyDB, centroide);
        }

        private void InitFuzzyEngineFor6Person()
        {
            //Superficie Captacion SC
            FuzzySet scChico = new FuzzySet("Chico", new TrapezoidalFunction(200, 300, TrapezoidalFunction.EdgeType.Right));
            FuzzySet scMediano = new FuzzySet("Mediano", new TrapezoidalFunction(200, 400, 600));
            FuzzySet scGrande = new FuzzySet("Grande", new TrapezoidalFunction(500, 700, 900));
            FuzzySet scMuyGrande = new FuzzySet("MuyGrande", new TrapezoidalFunction(800, 900, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvSuperficieCaptacion = new LinguisticVariable("SC", 0, 1200);
            lvSuperficieCaptacion.AddLabel(scChico);
            lvSuperficieCaptacion.AddLabel(scMediano);
            lvSuperficieCaptacion.AddLabel(scGrande);
            lvSuperficieCaptacion.AddLabel(scMuyGrande);

            //volumen del sistema de almacenamiento VA
            FuzzySet vaPequenio = new FuzzySet("Pequenio", new TrapezoidalFunction(20, 30, TrapezoidalFunction.EdgeType.Right));
            FuzzySet vaIntermedio = new FuzzySet("Intermedio", new TrapezoidalFunction(20, 40, 60));
            FuzzySet vaConsiderable = new FuzzySet("Considerable", new TrapezoidalFunction(50, 60, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvVolumenAlmacenamiento = new LinguisticVariable("VA", 0, 100);
            lvVolumenAlmacenamiento.AddLabel(vaPequenio);
            lvVolumenAlmacenamiento.AddLabel(vaIntermedio);
            lvVolumenAlmacenamiento.AddLabel(vaConsiderable);

            //precipitaciones pluviales PP
            FuzzySet ppMuyBaja = new FuzzySet("MuyBaja", new TrapezoidalFunction(20, 40, TrapezoidalFunction.EdgeType.Right));
            FuzzySet ppBaja = new FuzzySet("Baja", new TrapezoidalFunction(20, 40, 60));
            FuzzySet ppMedia = new FuzzySet("Media", new TrapezoidalFunction(40, 70, 100));
            FuzzySet ppAlta = new FuzzySet("Alta", new TrapezoidalFunction(80, 130, 180));
            FuzzySet ppMuyAlta = new FuzzySet("MuyAlta", new TrapezoidalFunction(150, 180, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvPrecipitacionesPluviales = new LinguisticVariable("PP", 0, 300);
            lvPrecipitacionesPluviales.AddLabel(ppMuyBaja);
            lvPrecipitacionesPluviales.AddLabel(ppBaja);
            lvPrecipitacionesPluviales.AddLabel(ppMedia);
            lvPrecipitacionesPluviales.AddLabel(ppAlta);
            lvPrecipitacionesPluviales.AddLabel(ppMuyAlta);

            //consumo C (OUTPUT)
            FuzzySet cBajo = new FuzzySet("Bajo", new TrapezoidalFunction(3000, 3200, TrapezoidalFunction.EdgeType.Right));
            FuzzySet cPromedio = new FuzzySet("Promedio", new TrapezoidalFunction(3000, 3400, 3800));
            FuzzySet dAlto = new FuzzySet("Alto", new TrapezoidalFunction(3600, 4000, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvConsumo = new LinguisticVariable("C", 0, 5000);
            lvConsumo.AddLabel(cBajo);
            lvConsumo.AddLabel(cPromedio);
            lvConsumo.AddLabel(dAlto);

            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvSuperficieCaptacion);
            fuzzyDB.AddVariable(lvVolumenAlmacenamiento);
            fuzzyDB.AddVariable(lvPrecipitacionesPluviales);
            fuzzyDB.AddVariable(lvConsumo);

            CentroidDefuzzifier centroide = new CentroidDefuzzifier(1000);
            SetInferenceSystemAndRules(GetInferenceSystemIndex(6), fuzzyDB, centroide);
        }

        private void InitFuzzyEngineFor8Person()
        {
            //Superficie Captacion SC
            FuzzySet scChico = new FuzzySet("Chico", new TrapezoidalFunction(300, 400, TrapezoidalFunction.EdgeType.Right));
            FuzzySet scMediano = new FuzzySet("Mediano", new TrapezoidalFunction(300, 500, 700));
            FuzzySet scGrande = new FuzzySet("Grande", new TrapezoidalFunction(500, 700, 900));
            FuzzySet scMuyGrande = new FuzzySet("MuyGrande", new TrapezoidalFunction(800, 1000, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvSuperficieCaptacion = new LinguisticVariable("SC", 0, 1200);
            lvSuperficieCaptacion.AddLabel(scChico);
            lvSuperficieCaptacion.AddLabel(scMediano);
            lvSuperficieCaptacion.AddLabel(scGrande);
            lvSuperficieCaptacion.AddLabel(scMuyGrande);

            //volumen del sistema de almacenamiento VA
            FuzzySet vaPequenio = new FuzzySet("Pequenio", new TrapezoidalFunction(30, 40, TrapezoidalFunction.EdgeType.Right));
            FuzzySet vaIntermedio = new FuzzySet("Intermedio", new TrapezoidalFunction(30, 50, 70));
            FuzzySet vaConsiderable = new FuzzySet("Considerable", new TrapezoidalFunction(60, 70, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvVolumenAlmacenamiento = new LinguisticVariable("VA", 0, 100);
            lvVolumenAlmacenamiento.AddLabel(vaPequenio);
            lvVolumenAlmacenamiento.AddLabel(vaIntermedio);
            lvVolumenAlmacenamiento.AddLabel(vaConsiderable);

            //precipitaciones pluviales PP
            FuzzySet ppMuyBaja = new FuzzySet("MuyBaja", new TrapezoidalFunction(20, 40, TrapezoidalFunction.EdgeType.Right));
            FuzzySet ppBaja = new FuzzySet("Baja", new TrapezoidalFunction(20, 40, 60));
            FuzzySet ppMedia = new FuzzySet("Media", new TrapezoidalFunction(40, 70, 100));
            FuzzySet ppAlta = new FuzzySet("Alta", new TrapezoidalFunction(80, 130, 180));
            FuzzySet ppMuyAlta = new FuzzySet("MuyAlta", new TrapezoidalFunction(150, 180, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvPrecipitacionesPluviales = new LinguisticVariable("PP", 0, 300);
            lvPrecipitacionesPluviales.AddLabel(ppMuyBaja);
            lvPrecipitacionesPluviales.AddLabel(ppBaja);
            lvPrecipitacionesPluviales.AddLabel(ppMedia);
            lvPrecipitacionesPluviales.AddLabel(ppAlta);
            lvPrecipitacionesPluviales.AddLabel(ppMuyAlta);

            //consumo C (OUTPUT)
            FuzzySet cBajo = new FuzzySet("Bajo", new TrapezoidalFunction(4000, 4400, TrapezoidalFunction.EdgeType.Right));
            FuzzySet cPromedio = new FuzzySet("Promedio", new TrapezoidalFunction(4000, 4400, 5000));
            FuzzySet dAlto = new FuzzySet("Alto", new TrapezoidalFunction(4600, 5000, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvConsumo = new LinguisticVariable("C", 0, 6000);
            lvConsumo.AddLabel(cBajo);
            lvConsumo.AddLabel(cPromedio);
            lvConsumo.AddLabel(dAlto);

            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvSuperficieCaptacion);
            fuzzyDB.AddVariable(lvVolumenAlmacenamiento);
            fuzzyDB.AddVariable(lvPrecipitacionesPluviales);
            fuzzyDB.AddVariable(lvConsumo);

            CentroidDefuzzifier centroide = new CentroidDefuzzifier(1000);
            SetInferenceSystemAndRules(GetInferenceSystemIndex(8), fuzzyDB, centroide);
        }

        // Segun la investigación de Ricardo las reglas definidas son siempre las mismas para la distinta cantidad de personas.
        // Por eso este método es común a las 4 inicializaciones (2, 4, 6 y 8 personas).
        private void SetInferenceSystemAndRules(int is_index, Database fuzzyDB, CentroidDefuzzifier centroide)
        {            
            IS[is_index] = new InferenceSystem(fuzzyDB, centroide);

            // SC Chico
            IS[is_index].NewRule("Rule 1", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 2", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 3", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Media") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 4", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 5", IF_IS("SC", "Chico") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 6", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 7", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 8", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Media") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 9", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 10", IF_IS("SC", "Chico") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 11", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 12", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 13", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "Media") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 14", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 15", IF_IS("SC", "Chico") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));

            // SC Mediano
            IS[is_index].NewRule("Rule 16", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 17", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 18", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 19", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 20", IF_IS("SC", "Mediano") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 21", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 22", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 23", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 24", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 25", IF_IS("SC", "Mediano") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 26", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 27", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "Baja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 28", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 29", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 30", IF_IS("SC", "Mediano") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));

            // SC Grande
            IS[is_index].NewRule("Rule 31", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 32", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 33", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 34", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 35", IF_IS("SC", "Grande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 36", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 37", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 38", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 39", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 40", IF_IS("SC", "Grande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 41", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 42", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 43", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Media") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 44", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 45", IF_IS("SC", "Grande") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));

            // SC MuyGrande
            IS[is_index].NewRule("Rule 46", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 47", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 48", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Media") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 49", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "Alta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 50", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Pequenio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 51", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 52", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Baja") + THEN_IS("C", "Promedio"));
            IS[is_index].NewRule("Rule 53", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Media") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 54", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 55", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Intermedio") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 56", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyBaja") + THEN_IS("C", "Bajo"));
            IS[is_index].NewRule("Rule 57", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Baja") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 58", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Media") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 59", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "Alta") + THEN_IS("C", "Alto"));
            IS[is_index].NewRule("Rule 60", IF_IS("SC", "MuyGrande") + AND_IS("VA", "Considerable") + AND_IS("PP", "MuyAlta") + THEN_IS("C", "Alto"));
        }

        private int GetInferenceSystemIndex(int qtyPeople)
        {           
            switch (qtyPeople)
            {
                case 2:
                    return 0;
                case 4:
                    return 1;
                case 6:
                    return 2;
                case 8:
                    return 3;
                default:
                    return 0;
            }
        }        

        public float DoInference(float sc, float va, float pp, int qtyPeople)
        {
            InferenceSystem infSys = IS[GetInferenceSystemIndex(qtyPeople)];

            infSys.SetInput("SC", sc);
            infSys.SetInput("VA", va);
            infSys.SetInput("PP", pp);

            float result = 0;
            try
            {
                result = infSys.Evaluate("C");
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