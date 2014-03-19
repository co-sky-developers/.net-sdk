using System;
using System.Linq;
using System.Reflection;

namespace NFleet.Data
{
    public static class DataUtil
    {
        public static T ToRequest<T>(this object data)
        {
            Type t = data.GetType();
            PropertyInfo[] propertyInfos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var request = Activator.CreateInstance<T>();

            var requestProperties = typeof(T).GetProperties().ToList().Select(item => item.Name);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (requestProperties.Contains(propertyInfo.Name))
                {
                    var prop = data.GetType().GetProperty(propertyInfo.Name);
                    var value = prop.GetValue(data, null);

                    request.GetType().GetProperty(propertyInfo.Name).SetValue(request, value, null);
                }
            }

            return request;
        }
    }
}
