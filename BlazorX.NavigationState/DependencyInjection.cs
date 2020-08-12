using Microsoft.Extensions.DependencyInjection;

namespace BlazorX.NavigationState
{
    public static class DependencyInjection
    {
        public static void AddNavigationState(this ServiceCollection collection)
        {
            collection.AddScoped<INavigationState, NavigationState>();
        }
    }
}