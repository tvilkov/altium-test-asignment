using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Altium.Sorter
{
    internal struct Record
    {
        public readonly long Number;
        public readonly string String;

        public Record(long number, string str)
        {
            Number = number;
            String = str ?? throw new ArgumentNullException(nameof(str));
        }

        public bool Equals(Record other)
        {
            return Number == other.Number && String == other.String;
        }

        public override bool Equals(object obj)
        {
            return obj is Record other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, String);
        }

        public override string ToString()
        {
            return Number.ToString(CultureInfo.InvariantCulture) + ". " + String;
        }

        public static Record Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new FormatException();
            var parts = input.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) throw new FormatException();
            return new Record(
                int.Parse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture),
                parts[1].Substring(1)  // Eat one space after '.'
                );
        }

        public static readonly IComparer<Record> DefaultComparer = new RecordComparer();

        private sealed class RecordComparer : IComparer<Record>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(Record x, Record y)
            {
                var strCmp = string.Compare(x.String, y.String, StringComparison.Ordinal); ;
                return strCmp != 0 ? strCmp : x.Number.CompareTo(y.Number);
            }
        }
    }
}