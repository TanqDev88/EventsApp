using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Ticketera.Extensions
{
    public static class ExtraPropertiesExt
    {
        public static T GetPropertyList<T>(this IHasExtraProperties source, string name)
        {
            var value = source.GetProperty<string>(name);
            if (value == null)
            {
                return default;
            }
            var obj = JsonConvert.DeserializeObject<T>(value);

            return obj;
        }

        public static TSource SetPropertyList<TSource,T>(this TSource source, string name, IList<T> value, bool validate = true) 
            where TSource : IHasExtraProperties 
            where T: class            
        {
            if (validate)
            {
                ExtensibleObjectValidator.CheckValue(source, name, value);
            }

            source.ExtraProperties[name] = JsonConvert.SerializeObject(value);

            return source;
        }
    }
}
