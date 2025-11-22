using System;
using System.Text;

namespace SemMapExample
{
    public static class Semantics
    {
        /// <summary>
        /// SemMap konstruiert einen Mapping-Prompt für input = v und examples = Q.
        /// </summary>
        public static string SemMap(string input, Tuple<string, string>[] examples)
        {
            if (input == null) input = string.Empty;
            if (examples == null) examples = Array.Empty<Tuple<string, string>>();

            var sb = new StringBuilder();

            // Prompt-Template
            string promptTemplate =
                "Map the following inputs to outputs based on the given examples.\n" +
                "Each line has the form: <input> => <output>.\n\n" +
                "{0}\n" +
                "{1} =>";

            // Beispiele in Textform serialisieren
            foreach (var (source, target) in examples)
            {
                sb.AppendLine($"{source} => {target}");
            }

            string examplesBlock = sb.ToString().TrimEnd('\n', '\r');

            // Finalen Prompt zusammenstellen
            string prompt = string.Format(promptTemplate, examplesBlock, input);

            // -----------------------------------------------
            // LLM-Aufruf (bewusst deaktiviert)
            //
            // var answer = LlmClient.Query(prompt);
            //
            // -----------------------------------------------

            // Prompt in Konsole ausgeben
            Console.Write("\n=== Generated Prompt =========================");
            Console.Write(prompt);
            Console.Write("===============================================\n");

            // Da der LLM deaktiviert bleibt, geben wir den Prompt zurück
            return prompt;
        }
    }
}