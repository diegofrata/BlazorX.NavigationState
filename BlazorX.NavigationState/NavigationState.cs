using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Flurl;
using Microsoft.AspNetCore.Components;

namespace BlazorX.NavigationState
{
    public interface INavigationState : IDisposable
    {
        QueryProperty<T> QueryProperty<T>(string key, T defaultValue);
        QueryArray<T> QueryArray<T>(string key, T[] defaultValue);
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
            _subscription = manager.LocationStream().Subscribe(_location);
        }
        
        internal IObservable<Url> Location => _location;
        internal IReadOnlyList<QueryParameter> GetQueryParameters(string key) => _location.Value.QueryParams.FindAll(x => x.Name == key);
        internal void SetQueryParameters(string key, object? value)
        {
            var newUrl = _location.Value.Clone();
            newUrl.SetQueryParam(key, value);
            _manager.NavigateTo(newUrl);
        }

        public QueryProperty<T> QueryProperty<T>(string key, T defaultValue = default) =>
            new QueryProperty<T>(this, key, defaultValue);
        
        public QueryArray<T> QueryArray<T>(string key, T[] defaultValue) => new QueryArray<T>(this, key, defaultValue);

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}