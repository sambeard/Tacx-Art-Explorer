using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models
{
    public readonly struct SizeOption
    {

        private const int defaultSize = 843; // Default size for images
        private readonly int _value;

        public string Value
        {
            get => _value > 0 ? _value.ToString() + ",": "full";
        }

        private static readonly HashSet<int> Allowed = new() { 200, 400, 843, 1686};

        public SizeOption()
        {
            _value = defaultSize;
        }
        public SizeOption(int value)
        {
            if (!Allowed.Contains(value) || value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            _value = value > 0 ? value: -1;
        }

        public static readonly SizeOption Tiny = new(200);
        public static readonly SizeOption Small = new(400);
        public static readonly SizeOption Medium = new(600);
        public static readonly SizeOption Large = new(1686);
        public static readonly SizeOption Full = new(-1);
        public static readonly SizeOption Default = new(defaultSize);

        public override string ToString() => Value;

        // Equality implementation
        public bool Equals(SizeOption other) => _value == other._value;

        public override bool Equals(object? obj) => obj is SizeOption other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(SizeOption left, SizeOption right) => left.Equals(right);

        public static bool operator !=(SizeOption left, SizeOption right) => !left.Equals(right);

    }

}
