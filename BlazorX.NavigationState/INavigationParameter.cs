using System;

namespace BlazorX.NavigationState
{
    public interface INavigationParameter<T>
    {
        public T Value { get; set; }
        
        public IObservable<T> ValueStream { get; }
    }

    public interface IQueryParameter<T> : INavigationParameter<T>
    {
        
    }
}