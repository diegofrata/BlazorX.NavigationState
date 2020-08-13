using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace BlazorX.NavigationState
{
    abstract class QueryParameter<T> : IQueryParameter<T>
    {
        readonly string? _format;

        protected QueryParameter(
            NavigationState state, 
            string key, 
            T defaultValue,
            string? format = null)
        {
            _format = format;
            State = state;
            Key = key;
            DefaultValue = defaultValue;
        }

        protected NavigationState State { get; }
        protected string Key { get; }
        protected T DefaultValue { get; }
        
        protected abstract void SetQueryParameters(T v, string? format);
        protected abstract T GetQueryParameters();
        protected abstract IEqualityComparer<T> Comparer { get; }

        public T Value
        {
            get => GetQueryParameters();
            set => SetQueryParameters(value, _format);
        }

        public IObservable<T> ValueStream => State.Location.Select(x => Value).DistinctUntilChanged(Comparer);
    }
}