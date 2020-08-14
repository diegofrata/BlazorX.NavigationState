using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.Components;

namespace BlazorX.NavigationState
{
    public interface INavigationState : IDisposable
    {
        IQueryParameter<T> QueryProperty<T>(
            string key,
            T defaultValue = default,
            string? format = null);

        IQueryParameter<T[]> QueryArray<T>(
            string key,
            T[] defaultValue,
            string? format = null);
    }

    public class NavigationState : INavigationState
    {
        readonly NavigationManager _manager;
        readonly IDisposable _subscription;
        readonly BehaviorSubject<Url> _location;
        
        public NavigationState(NavigationManager manager)
        {
            _manager = manager;
            _location = new BehaviorSubject<Url>(new Url(manager.Uri));
            _subscription = manager.LocationStream().DistinctUntilChanged().Subscribe(_location);
        }

        internal IObservable<Url> Location => _location;
        internal IReadOnlyList<QueryParameter> GetQueryParameters(string key) => _location.Value.QueryParams.FindAll(x => x.Name == key);

        internal void SetQueryParameters(string key, object? value)
        {
            var currentUrl = _location.Value;
            var newUrl = currentUrl.Clone();
            newUrl.SetQueryParam(key, value);
            
            if (currentUrl.Equals(newUrl))
                return;

            _manager.NavigateTo(newUrl);
        }

        public IQueryParameter<T> QueryProperty<T>(
            string key,
            T defaultValue = default,
            string? format = null)
        {
            return new QueryProperty<T>(this, key, defaultValue, format);
        }

        public IQueryParameter<T[]> QueryArray<T>(
            string key,
            T[] defaultValue,
            string? format = null)
        {
            return new QueryArray<T>(this, key, defaultValue, format);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}