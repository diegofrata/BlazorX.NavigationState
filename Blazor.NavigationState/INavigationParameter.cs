using System;

namespace Blazor.NavigationState
{
    public interface INavigationParameter<T>
    {
        public T Value { get; set; }
        
        public IObservable<T> ValueStream { get; }
    }
}