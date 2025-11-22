using System;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Features;

namespace SemMapExample
{
    public class RankingScore : Feature<double>
    {
        public RankingScore(Grammar grammar) : base(grammar, "Score")
        {
        }

        protected override double GetFeatureValueForVariable(VariableNode variable)
        {
            return 0;
        }

        [FeatureCalculator(nameof(Semantics.SemMap))]
        public static double SemMap(double v, double k)
        {
            return 1;
        }

        [FeatureCalculator("Q", Method = CalculationMethod.FromLiteral)]
        public static double K(Tuple<string,string>[] Q)
        {
            return 0;
        }
    }
}