using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BlazorX.NavigationState
{
    abstract class QueryParameter<T> : IQueryParameter<T>
    {
        readonly Subject<T> _updateSubject = new Subject<T>();
        
        protected QueryParameter(
            NavigationState state, 
            string key, 
            T defaultValue, 
            Func<IObservable<T>, IObservable<T>>? updateTransformer = null,
            string? format = null)
        {
            State = state;
            Key = key;
            DefaultValue = defaultValue;

            updateTransformer ??= x => x;
            updateTransformer(_updateSubject).Subscribe(v => SetQueryParameters(v, format));
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
            set => _updateSubject.OnNext(value);
        }

        public IObservable<T> ValueStream => State.Location.Select(x => Value).DistinctUntilChanged(Comparer);
    }
}