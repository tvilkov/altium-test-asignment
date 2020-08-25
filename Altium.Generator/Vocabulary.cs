using System;
using System.IO;

namespace Altium.Generator
{
    internal class Vocabulary
    {
        public string[] Nouns { get; }
        public string[] Adjectives { get; }
        public string[] Verbs { get; }
        public string[] Propositions { get; }

        private Vocabulary(string[] nouns, string[] adjectives, string[] verbs, string[] propositions)
        {
            Nouns = nouns ?? throw new ArgumentNullException(nameof(nouns));
            Adjectives = adjectives ?? throw new ArgumentNullException(nameof(adjectives));
            Verbs = verbs ?? throw new ArgumentNullException(nameof(verbs));
            Propositions = propositions ?? throw new ArgumentNullException(nameof(propositions));
        }

        public static Vocabulary LoadFrom(string folder)
        {
            if (folder == null) throw new ArgumentNullException(nameof(folder));

            return new Vocabulary(
                File.ReadAllLines(Path.Combine(folder, "nouns.txt")),
                File.ReadAllLines(Path.Combine(folder, "adjectives.txt")),
                File.ReadAllLines(Path.Combine(folder, "verbs.txt")),
                new[] {
                    "in", "around of", "on top of", "near", "behind", "on top of", "far from",
                    "in front of", "from", "above", "along", "up", "down"
                }
            );
        }
    }
}