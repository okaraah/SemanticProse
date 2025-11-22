using System;
using System.Collections.Generic;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Compound.Split.Build.NodeTypes;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Rules;
using Microsoft.ProgramSynthesis.Specifications;

namespace SemMapExample
{
    public class WitnessFunctions : DomainLearningLogic
    {
        public WitnessFunctions(Grammar grammar) : base(grammar)
        {
        }

        [WitnessFunction(nameof(Semantics.SemMap), 1)]
        public ExampleSpec WitnessSemMap(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            var Q = new List<Tuple<string,string>>();
                
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;
                var tup = Tuple.Create(input,output);
                Q.Add(tup);
            }
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                result[inputState] = Q;
            }
            return new ExampleSpec(result);
        }
    }
}