using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models
{
    public readonly struct SizeOption
    {

        private const int tinySize = 200;
        private const int smallSize = 400;
        private const int mediumSize = 600;
        private const int defaultSize = 843; // Default size for images
        private const int largeSize = 1686;
        private const int fullSize = -1;

        private readonly int _value;

        public int IntValue => _value;

        public string Value
        {
            get => _value > 0 ? _value.ToString() + ",": "full";
        }

        private static readonly HashSet<int> Allowed = new() { tinySize, smallSize, mediumSize, defaultSize, largeSize, fullSize};

        public SizeOption()
        {
            _value = defaultSize;
        }
        public SizeOption(int value)
        {
            if (!Allowed.Contains(value) || value < -1)
                throw new ArgumentOutOfRangeException(nameof(value),value,null);
            _value = value > 0 ? value: -1;
        }

        public static readonly SizeOption Tiny = new(tinySize);
        public static readonly SizeOption Small = new(smallSize);
        public static readonly SizeOption Medium = new(mediumSize);
        public static readonly SizeOption Default = new(defaultSize);
        public static readonly SizeOption Large = new(largeSize);
        public static readonly SizeOption Full = new(fullSize);

        public override string ToString() => Value;

        // Equality implementation
        public bool Equals(SizeOption other) => _value == other._value;

        public override bool Equals(object? obj) => obj is SizeOption other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(SizeOption left, SizeOption right) => left.Equals(right);

        public static bool operator !=(SizeOption left, SizeOption right) => !left.Equals(right);

    }

}
