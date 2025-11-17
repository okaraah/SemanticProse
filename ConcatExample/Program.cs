using System; // Core BCL namespaces for console I/O, collections, file loading, and reflection.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.ProgramSynthesis; // PROSE namespaces for grammar compilation, program representation, synthesis, specifications, and ranking.
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Compiler;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Learning.Strategies;
using Microsoft.ProgramSynthesis.Specifications;
using Microsoft.ProgramSynthesis.VersionSpace;

namespace ConcatExample
{
    internal class Program
    {
        private static readonly Grammar Grammar = DSLCompiler.Compile(new CompilerOptions // Compile the DSL grammar so the engine knows the vocabulary of programs to search.
        {
            InputGrammarText = File.ReadAllText("synthesis/grammar/substring.grammar"), // Load the grammar file; defines substring DSL operators.
            References = CompilerReference.FromAssemblyFiles(typeof(Program).GetTypeInfo().Assembly) // Point compiler to this assembly for references.
        }).Value;

        private static SynthesisEngine _prose;

        private static readonly Dictionary<State, object> Examples = new Dictionary<State, object>();
        private static ProgramNode _topProgram;

        private static void Main(string[] args)
        {
            _prose = ConfigureSynthesis(); // Build the synthesis engine (grammar + strategies).
            var menu = @"Select one of the options: 
1 - provide new example
2 - run top synthesized program on a new input
3 - exit";
            var option = 0; // Track the menu selection until the user chooses to exit (3).
            while (option != 3)
            {
                Console.Out.WriteLine(menu); // Draw the menu and read the user choice.
                try
                {
                    option = short.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.Out.WriteLine("Invalid option. Try again."); // Non-numeric inputs fall here.
                    continue;
                }

                try
                {
                    RunOption(option); // Execute the requested action.
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Something went wrong..."); // Catch any synthesis/run-time issues so the loop continues.
                    Console.Error.WriteLine("Exception message: {0}", e.Message);
                }
            }
        }

        private static void RunOption(int option)
        {
            switch (option)
            {
                case 1: // Add a new training example and relearn.
                    LearnFromNewExample();
                    break;
                case 2: // Run the current best program on a new input string.
                    RunOnNewInput();
                    break;
                default:
                    Console.Out.WriteLine("Invalid option. Try again.");
                    break;
            }
        }

        private static void LearnFromNewExample()
        {
            Console.Out.Write("Provide a new input-output example (e.g., \"(Sumit Gulwani)\",\"Gulwani\"): "); // Input/output must be quoted.
            try
            {
                string input = Console.ReadLine(); // Grab the entire line and parse quoted segments manually.
                if (input != null)
                {
                    int startFirstExample = input.IndexOf("\"", StringComparison.Ordinal) + 1; // Start of first quoted string.
                    int endFirstExample = input.IndexOf("\"", startFirstExample + 1, StringComparison.Ordinal) + 1; // End of first quoted string.
                    int startSecondExample = input.IndexOf("\"", endFirstExample + 1, StringComparison.Ordinal) + 1; // Start of second quoted string.
                    int endSecondExample = input.IndexOf("\"", startSecondExample + 1, StringComparison.Ordinal) + 1; // End of second quoted string.

                    if (startFirstExample >= endFirstExample || startSecondExample >= endSecondExample)
                        throw new Exception(
                            "Invalid example format. Please try again. input and out should be between quotes");

                    string inputExample = input.Substring(startFirstExample, endFirstExample - startFirstExample - 1); // Extract input without quotes.
                    string outputExample = input.Substring(startSecondExample, endSecondExample - startSecondExample - 1); // Extract output without quotes.

                    State inputState = State.CreateForExecution(Grammar.InputSymbol, inputExample); // Bind input to grammar symbol.
                    Examples.Add(inputState, outputExample); // Save the example for learning.
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid example format. Please try again. input and out should be between quotes"); // Wrap parsing/format errors.
            }

            var spec = new ExampleSpec(Examples); // Build ExampleSpec to feed examples into PROSE.
            Console.Out.WriteLine("Learning a program for examples:");
            foreach (KeyValuePair<State, object> example in Examples) Console.WriteLine("\"{0}\" -> \"{1}\"", example.Key.Bindings.First().Value, example.Value); // Echo examples.

            var scoreFeature = new RankingScore(Grammar); // RankingScore supplies heuristics to rank synthesized programs.
            ProgramSet topPrograms = _prose.LearnGrammarTopK(spec, scoreFeature, 4, null); // Learn top 4 candidate programs.
            if (topPrograms.IsEmpty) throw new Exception("No program was found for this specification."); // Bail if none fit.

            _topProgram = topPrograms.RealizedPrograms.First(); // Remember the single best program.
            Console.Out.WriteLine("Top 4 learned programs:");
            var counter = 1;
            foreach (ProgramNode program in topPrograms.RealizedPrograms)
            {
                if (counter > 4) break; // Only print up to four programs.
                Console.Out.WriteLine("==========================");
                Console.Out.WriteLine("Program {0}: ", counter);
                Console.Out.WriteLine(program.PrintAST(ASTSerializationFormat.HumanReadable)); // Human-readable AST pretty-print.
                counter++;
            }
        }

        private static void RunOnNewInput()
        {
            if (_topProgram == null)
                throw new Exception("No program was synthesized. Try to provide new examples first."); // Need a synthesized program.
            Console.Out.WriteLine("Top program: {0}", _topProgram);

            try
            {
                Console.Out.Write("Insert a new input: "); // Ask for a new input formatted the same way (quoted string).
                string newInput = Console.ReadLine();
                if (newInput != null)
                {
                    int startFirstExample = newInput.IndexOf("\"", StringComparison.Ordinal) + 1; // Extract quoted content.
                    int endFirstExample = newInput.IndexOf("\"", startFirstExample + 1, StringComparison.Ordinal) + 1;
                    newInput = newInput.Substring(startFirstExample, endFirstExample - startFirstExample - 1);
                    State newInputState = State.CreateForExecution(Grammar.InputSymbol, newInput); // Bind the input.
                    Console.Out.WriteLine("RESULT: \"{0}\" -> \"{1}\"", newInput, _topProgram.Invoke(newInputState)); // Invoke learned program.
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Execution error: " + e.Message); // Print detailed errors.
                Console.Error.WriteLine(e);
            }
        }

        public static SynthesisEngine ConfigureSynthesis()
        {
            var witnessFunctions = new WitnessFunctions(Grammar); // Collect witness functions to backpropagate specs.
            var deductiveSynthesis = new DeductiveSynthesis(witnessFunctions); // Use deductive synthesis as the learning strategy.
            var synthesisExtrategies = new ISynthesisStrategy[] {deductiveSynthesis}; // Strategies array, only one here.
            var synthesisConfig = new SynthesisEngine.Config {Strategies = synthesisExtrategies}; // Build engine config.
            var prose = new SynthesisEngine(Grammar, synthesisConfig); // Instantiate engine with grammar + config.
            return prose; // Return configured engine.
        }
    }
}
