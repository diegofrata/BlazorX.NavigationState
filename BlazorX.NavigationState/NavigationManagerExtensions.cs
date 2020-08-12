using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Flurl;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorX.NavigationState
{
    static class NavigationManagerExtensions
    {
        public static IObservable<LocationChangedEventArgs> LocationChangedStream(this NavigationManager navigationManager)
        {
            return Observable.Create<LocationChangedEventArgs>(o =>
            {
                void Handler(object sender, LocationChangedEventArgs args) => o.OnNext(args);
                navigationManager.LocationChanged += Handler;
                return Disposable.Create(navigationManager, n => n.LocationChanged -= Handler);
            });
        }

        public static IObservable<Url> LocationStream(this NavigationManager navigationManager)
        {
            return navigationManager
                .LocationChangedStream()
                .Select(x => x.Location)
                .StartWith(navigationManager.Uri)
                .Select(uri => new Url(uri));
        }
    }
}