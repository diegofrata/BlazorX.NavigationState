using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorX.NavigationState
{
    public delegate Task AsyncEventHandler<in TEventArgs>(
        object sender,
        TEventArgs e);
    
    
    public static class AsyncEventHandlerExtensions
    {
        public static IEnumerable<AsyncEventHandler<TEventArgs>> GetHandlers<TEventArgs>(
            this AsyncEventHandler<TEventArgs> handler)
            => handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>();

        public static Task InvokeAllAsync<TEventArgs>(
            this AsyncEventHandler<TEventArgs> handler,
            object sender,
            TEventArgs e)
            => Task.WhenAll(
                handler.GetHandlers()
                    .Select(handleAsync => handleAsync(sender, e)));
    }
}