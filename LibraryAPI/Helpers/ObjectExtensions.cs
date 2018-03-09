using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace LibraryAPI.Helpers
{
    internal static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dataShapeedObject = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    // get the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(source);

                    // add the filed to the ExpandoObject
                    ((IDictionary<string, object>)dataShapeedObject).Add(propertyInfo.Name, propertyValue);
                }

                return dataShapeedObject;
            }

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                var propertyInfo = typeof(TSource)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                }

                var propertyValue = propertyInfo.GetValue(source);

                ((IDictionary<string, object>)dataShapeedObject).Add(propertyInfo.Name, propertyValue);
            }

            return dataShapeedObject;
        }
    }
}
