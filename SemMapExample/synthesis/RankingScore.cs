using System;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Features;

namespace SemMapExample
{
    /// <summary>
    /// Ranking-Score
    /// </summary>
    public class RankingScore : Feature<double>
    {
        public RankingScore(Grammar grammar) : base(grammar, "Score")
        {
        }

        protected override double GetFeatureValueForVariable(VariableNode variable)
        {
            // Keine speziellen Präferenzen für Variablen
            return 0.0;
        }

        /// <summary>
        /// Score für den Operator SemMap
        /// vScore = Score des ersten Arguments (v)
        /// qScore = Score des zweiten Arguments (Q)
        /// </summary>
        [FeatureCalculator(nameof(Semantics.SemMap))]
        public static double SemMap(double vScore, double qScore)
        {
            // Im einfachsten Fall alles gleich bewerten
            return 1.0;
        }

        /// <summary>
        /// Score für Literalwerte von Q
        /// </summary>
        [FeatureCalculator("Q", Method = CalculationMethod.FromLiteral)]
        public static double Q(Tuple<string, string>[] q)
        {
            // Ebenfalls neutral; hier könnte man z.B. kleinere Q bevorzugen
            return 0.0;
        }
    }
}