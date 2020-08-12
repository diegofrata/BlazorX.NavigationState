using System;
using System.Linq;
using System.Reactive.Linq;

namespace Blazor.NavigationState
{
    public class QueryProperty<T> : INavigationParameter<T>
    {
        public static implicit operator T(QueryProperty<T> d) => d.Value;

        readonly NavigationState _state;
        readonly string _key;
        readonly T _defaultValue;

        public QueryProperty(NavigationState state, string key, T defaultValue)
        {
            _state = state;
            _key = key;
            _defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                var parameter = _state.GetQueryParameters(_key).FirstOrDefault();

                if (parameter?.Value == null)
                    return _defaultValue;

                return ConversionUtils<T>.Convert(parameter);
            }
            set => _state.SetQueryParameters(_key, value);
        }

        public IObservable<T> ValueStream => _state.Location.Select(x => Value).DistinctUntilChanged();
    }
}