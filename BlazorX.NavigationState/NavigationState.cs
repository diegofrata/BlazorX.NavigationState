using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Flurl;
using Microsoft.AspNetCore.Components;

namespace BlazorX.NavigationState
{
    public interface INavigationState : IDisposable
    {        
        event EventHandler<Url>? BeforeNavigate;
        event EventHandler<Url>? AfterNavigate;
        
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
            _subscription = manager.LocationStream().Subscribe(_location);
        }

        internal IObservable<Url> Location => _location;
        internal IReadOnlyList<QueryParameter> GetQueryParameters(string key) => _location.Value.QueryParams.FindAll(x => x.Name == key);
        public event EventHandler<Url>? BeforeNavigate;
        public event EventHandler<Url>? AfterNavigate;

        internal void SetQueryParameters(string key, object? value)
        {
            var newUrl = _location.Value.Clone();
            newUrl.SetQueryParam(key, value);
            
            BeforeNavigate?.Invoke(this, newUrl);
            _manager.NavigateTo(newUrl);
            AfterNavigate?.Invoke(this, newUrl);
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