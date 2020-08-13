using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BlazorX.NavigationState
{
    class QueryArray<T> : QueryParameter<T[]>
    {
        readonly string _emptyKey;

        public QueryArray(NavigationState state, string key, T[] defaultValue, string? format = null)
            : base(state, key, defaultValue, format)
        {
            _emptyKey = $"{Key}:empty";
        }

        protected override void SetQueryParameters(T[] v, string? format)
        {
            State.SetQueryParameters(Key, v.Select(x => x is IFormattable f ? (object) f.ToString(format, CultureInfo.InvariantCulture) : x));
            State.SetQueryParameters(_emptyKey, v.Length == 0 ? "" : null);
        }

        protected override T[] GetQueryParameters()
        {
            var parameters = State.GetQueryParameters(Key);

            if (parameters.Count != 0)
                return parameters.Select(x => ConversionUtils<T>.Convert(x)).ToArray();

            var emptyParameters = State.GetQueryParameters(_emptyKey);
            return emptyParameters.Any() ? Array.Empty<T>() : DefaultValue;
        }

        protected override IEqualityComparer<T[]> Comparer { get; } = SequenceEqualityComparer.Instance;

        class SequenceEqualityComparer : IEqualityComparer<T[]>
        {
            public static readonly SequenceEqualityComparer Instance = new SequenceEqualityComparer();

            public bool Equals(T[] x, T[] y) => x.SequenceEqual(y);

            public int GetHashCode(T[] obj) => 0;
        }
    }
}