using System.ComponentModel;
using Flurl;

namespace Blazor.NavigationState
{
    public static class ConversionUtils<T>
    {
        static readonly TypeConverter Converter;

        static ConversionUtils()
        {
            Converter = TypeDescriptor.GetConverter(typeof(T));
        }
        
        public static T Convert(QueryParameter parameter)
        {
            return (T) Converter.ConvertFrom(parameter.Value.ToString());
        }
        
    }
}