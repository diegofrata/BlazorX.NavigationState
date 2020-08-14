using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorX.NavigationState
{
    class QueryProperty<T> : QueryParameter<T>
    {
        public static implicit operator T(QueryProperty<T> d) => d.Value;

        public QueryProperty(NavigationState state, string key, T defaultValue, string? format = null) 
            : base(state, key, defaultValue, format)
        {
        }

        protected override async Task SetQueryParameters(T v, string? format)
        {
            object? boxedValue = v;

            if (boxedValue is IFormattable f)
                boxedValue = f.ToString(format, CultureInfo.InvariantCulture);
            
            await State.SetQueryParameters(Key, boxedValue);
        }

        protected override T GetQueryParameters()
        {
            var parameter = State.GetQueryParameters(Key).FirstOrDefault();
            return parameter?.Value == null ? DefaultValue : ConversionUtils<T>.Convert(parameter);
        }

        protected override IEqualityComparer<T> Comparer { get; } = EqualityComparer<T>.Default;
    }
}