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

        private FuzzyRain() {
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

            // Linguistic labels (fuzzy sets) that compose the distances
            FuzzySet fsNear = new FuzzySet("Near", new TrapezoidalFunction(15, 50, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsMedium = new FuzzySet("Medium", new TrapezoidalFunction(15, 50, 60, 100));
            FuzzySet fsFar = new FuzzySet("Far", new TrapezoidalFunction(60, 100, TrapezoidalFunction.EdgeType.Left));

            // Right Distance (Input)
            LinguisticVariable lvRight = new LinguisticVariable("RightDistance", 0, 120);
            lvRight.AddLabel(fsNear);
            lvRight.AddLabel(fsMedium);
            lvRight.AddLabel(fsFar);

            // Left Distance (Input)
            LinguisticVariable lvLeft = new LinguisticVariable("LeftDistance", 0, 120);
            lvLeft.AddLabel(fsNear);
            lvLeft.AddLabel(fsMedium);
            lvLeft.AddLabel(fsFar);

            // Front Distance (Input)
            LinguisticVariable lvFront = new LinguisticVariable("FrontalDistance", 0, 120);
            lvFront.AddLabel(fsNear);
            lvFront.AddLabel(fsMedium);
            lvFront.AddLabel(fsFar);

            // Linguistic labels (fuzzy sets) that compose the angle
            FuzzySet fsVN = new FuzzySet("VeryNegative", new TrapezoidalFunction(-40, -35, TrapezoidalFunction.EdgeType.Right));
            FuzzySet fsN = new FuzzySet("Negative", new TrapezoidalFunction(-40, -35, -25, -20));
            FuzzySet fsLN = new FuzzySet("LittleNegative", new TrapezoidalFunction(-25, -20, -10, -5));
            FuzzySet fsZero = new FuzzySet("Zero", new TrapezoidalFunction(-10, 5, 5, 10));
            FuzzySet fsLP = new FuzzySet("LittlePositive", new TrapezoidalFunction(5, 10, 20, 25));
            FuzzySet fsP = new FuzzySet("Positive", new TrapezoidalFunction(20, 25, 35, 40));
            FuzzySet fsVP = new FuzzySet("VeryPositive", new TrapezoidalFunction(35, 40, TrapezoidalFunction.EdgeType.Left));

            // Angle
            LinguisticVariable lvAngle = new LinguisticVariable("Angle", -50, 50);
            lvAngle.AddLabel(fsVN);
            lvAngle.AddLabel(fsN);
            lvAngle.AddLabel(fsLN);
            lvAngle.AddLabel(fsZero);
            lvAngle.AddLabel(fsLP);
            lvAngle.AddLabel(fsP);
            lvAngle.AddLabel(fsVP);

            // The database
            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvFront);
            fuzzyDB.AddVariable(lvLeft);
            fuzzyDB.AddVariable(lvRight);
            fuzzyDB.AddVariable(lvAngle);

            // Creating the inference system
            IS = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));

            // Going Straight
            IS.NewRule("Rule 1", "IF FrontalDistance IS Far THEN Angle IS Zero");
            // Going Straight (if can go anywhere)
            IS.NewRule("Rule 2", "IF FrontalDistance IS Far AND RightDistance IS Far AND LeftDistance IS Far THEN Angle IS Zero");
            // Near right wall
            IS.NewRule("Rule 3", "IF RightDistance IS Near AND LeftDistance IS Not Near THEN Angle IS LittleNegative");
            // Near left wall
            IS.NewRule("Rule 4", "IF RightDistance IS Not Near AND LeftDistance IS Near THEN Angle IS LittlePositive");
            // Near front wall - room at right
            IS.NewRule("Rule 5", "IF RightDistance IS Far AND FrontalDistance IS Near THEN Angle IS Positive");
            // Near front wall - room at left
            IS.NewRule("Rule 6", "IF LeftDistance IS Far AND FrontalDistance IS Near THEN Angle IS Negative");
            // Near front wall - room at both sides - go right
            IS.NewRule("Rule 7", "IF RightDistance IS Far AND LeftDistance IS Far AND FrontalDistance IS Near THEN Angle IS Positive");
        }

        public FuzzyRainResult DoInference(float rainAmount)
        {
            IS.SetInput("RightDistance", rainAmount);
            IS.SetInput("LeftDistance", rainAmount);
            IS.SetInput("FrontalDistance", rainAmount);
            try
            {
                return new FuzzyRainResult(IS.Evaluate("Angle"),IS.Evaluate("Angle"));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
