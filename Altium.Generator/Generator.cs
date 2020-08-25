using System;
using System.Linq;

namespace Altium.Generator
{
    internal static class Generator
    {
        public static Vocabulary Vocabulary { get; set; }
        public static Func<string> Noun = () => Vocabulary.Nouns[peekNumber(Vocabulary.Nouns.Length)];
        public static Func<string> Verb = () => Vocabulary.Verbs[peekNumber(Vocabulary.Verbs.Length)];
        public static Func<string> Adjective = () => Vocabulary.Adjectives[peekNumber(Vocabulary.Adjectives.Length)];
        public static Func<string> Proposition = () => Vocabulary.Propositions[peekNumber(Vocabulary.Propositions.Length)];
        public static Func<string> Optional(Func<string> option) => () => flipCoin() ? option() : null;
        public static Func<string> Combine(params Func<string>[] parts) => () => string.Join(" ", parts.Select(x => x()).Where(x => x != null));
        public static Func<string> OneOf(params Func<string>[] options) => () => options[peekNumber(options.Length)]();
        public static Func<string> OneOf(params string[] options) => () => options[peekNumber(options.Length)];
        public static Func<string> Const(string str) => () => str;

        private static readonly Random m_Rnd = new Random(DateTime.UtcNow.Second);

        private static bool flipCoin() => m_Rnd.Next(1, 11) > 5;

        private static int peekNumber(int max) => m_Rnd.Next(max);

        public static Func<string> Build()
        {
            return OneOf(
                // Noun is|are|will be adjective
                Combine(Noun, OneOf("is", "are", "will be"), Adjective),
                // Noun is|are|will be [adjective] noun
                Combine(Noun, OneOf("is", "are", "will be"), Optional(Adjective), Noun),
                // Noun verb [adjective] noun [proposition [adjective] noun]
                Combine(Noun, Verb, Optional(Adjective), Noun,
                    Optional(Combine(Proposition, Optional(Adjective), Noun))),
                // Proposition the noun there is a [adjective] none
                Combine(Proposition, Const("the"), Noun, Const("there is a"), Optional(Adjective), Noun),
                // A|an|my|our|their|his|her [Adjective] noun
                Combine(OneOf("a", "an", "my", "our", "their", "his", "her"), Optional(Adjective), Noun)
            );
        }
    }
}