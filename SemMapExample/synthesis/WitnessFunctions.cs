using System;
using System.Collections.Generic;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Rules;
using Microsoft.ProgramSynthesis.Specifications;

namespace SemMapExample
{
    /// <summary>
    /// Witness-Funktion für SemMap.
    /// Sie erzeugt Q = Liste aller (input,output)-Paare aus den Beispielen.
    /// </summary>
    public class WitnessFunctions : DomainLearningLogic
    {
        public WitnessFunctions(Grammar grammar) : base(grammar) { }

        [WitnessFunction(nameof(Semantics.SemMap), 1)]
        public ExampleSpec WitnessSemMap(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            var pairs = new List<Tuple<string, string>>();
            var inputSymbol = Grammar.InputSymbol;

            // Alle Beispiele zu Q zusammenführen
            foreach (var ex in spec.Examples)
            {
                var state = ex.Key;
                var input = state[inputSymbol] as string;
                var output = ex.Value as string;

                if (input != null && output != null)
                    pairs.Add(Tuple.Create(input, output));
            }

            var qArray = pairs.ToArray();

            // Q jedem Beispielstate zuweisen
            foreach (var ex in spec.Examples)
            {
                result[ex.Key] = qArray;
            }

            return new ExampleSpec(result);
        }
    }
}