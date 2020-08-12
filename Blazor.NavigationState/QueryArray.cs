using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Blazor.NavigationState
{
    public class QueryArray<T> : INavigationParameter<T[]>
    {
        public static implicit operator T[](QueryArray<T> d) => d.Value;

        readonly NavigationState _state;
        readonly string _key;
        readonly T[] _defaultValue;
        readonly string _emptyKey;

        public QueryArray(NavigationState state, string key, T[] defaultValue)
        {
            _state = state;
            _key = key;
            _emptyKey = $"{_key}:empty";
            _defaultValue = defaultValue;
        }

        public T[] Value
        {
            get
            {
                var parameters = _state.GetQueryParameters(_key);

                if (parameters.Count == 0)
                {
                    var emptyParameters = _state.GetQueryParameters(_emptyKey);
                    return emptyParameters.Any() ? Array.Empty<T>() : _defaultValue;
                }

                return parameters.Select(x => ConversionUtils<T>.Convert(x)).ToArray();
            }
            set
            {
                _state.SetQueryParameters(_key, value);
                _state.SetQueryParameters(_emptyKey, value.Length == 0 ? "" : null);
            }
        }

        public IObservable<T[]> ValueStream => _state.Location.Select(x => Value).DistinctUntilChanged(SequenceEqualityComparer.Instance);
        
        class SequenceEqualityComparer : IEqualityComparer<T[]>
        {
            public static readonly SequenceEqualityComparer Instance = new SequenceEqualityComparer();
            
            public bool Equals(T[] x, T[] y) => x.SequenceEqual(y);

            public int GetHashCode(T[] obj) => 0;
        }
    }
}