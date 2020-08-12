using Microsoft.Extensions.DependencyInjection;

namespace BlazorX.NavigationState
{
    public static class DependencyInjection
    {
        public static void AddNavigationState(this IServiceCollection collection)
        {
            collection.AddScoped<INavigationState, NavigationState>();
        }
    }
}