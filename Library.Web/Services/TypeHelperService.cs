using System.Reflection;

namespace Library.Web.Services
{
    public class TypeHelperService : ITypeHelperService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach (string field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                var prepertyInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (prepertyInfo == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
