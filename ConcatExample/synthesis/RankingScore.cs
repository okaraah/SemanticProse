using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Features;

namespace ConcatExample
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

        [FeatureCalculator(nameof(Semantics.Concat))]
        public static double Concat(double v, double k)
        {
            return 1;
        }

        [FeatureCalculator("s", Method = CalculationMethod.FromLiteral)]
        public static double K(string s)
        {
            return 0;
        }
    }
}