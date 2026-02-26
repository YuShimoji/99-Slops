using System;
using UnityEngine;

namespace GlitchWorker.Story
{
    [Serializable]
    public struct StoryId : IEquatable<StoryId>
    {
        [SerializeField] private string _value;

        public StoryId(string value)
        {
            _value = value?.Trim() ?? string.Empty;
        }

        public bool IsDefined => !string.IsNullOrEmpty(_value);

        public override string ToString()
        {
            return _value ?? string.Empty;
        }

        public bool Equals(StoryId other)
        {
            return string.Equals(ToString(), other.ToString(), StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is StoryId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(ToString());
        }

        public static bool operator ==(StoryId left, StoryId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StoryId left, StoryId right)
        {
            return !left.Equals(right);
        }
    }
}
