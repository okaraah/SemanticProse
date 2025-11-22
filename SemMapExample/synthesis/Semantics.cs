using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SemMapExample
{
    public static class Semantics
    {
        public static string SemMap(string input, Tuple<string,string>[] examples)
        {
            // 1. Prompt konstruieren
            var sb = new StringBuilder();
            
            // Prompt-Template: beschreibt, wie wir die Beispiele ins LLM geben würden
            // {0} steht für alle Beispielzeilen, {1} für den neuen Input
            string promptTemplate =
                "Map the following inputs to outputs based on the given examples.\n" +
                "Each line has the form: <input> => <output>.\n\n" +
                "{0}\n" +
                "{1} =>";
            foreach (var (source, target) in examples)
            {
                sb.AppendLine($"{source} => {target}");
            }
            string examplesBlock = sb.ToString().TrimEnd('\n', '\r');

            // 2. Endgültigen Prompt mit Template zusammensetzen
            string prompt = string.Format(promptTemplate, examplesBlock, input);

            // 2. LLM aufrufen (Pseudo-Code)
            //var answer = LlmClient.Query(prompt); // externe API, musst du selbst definieren
            var answer = "correct answer";
            Console.WriteLine(prompt);
            
            // 3. Antwort normalisieren
            return answer.Trim();
        }
    }
}