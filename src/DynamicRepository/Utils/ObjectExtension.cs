using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicRepository.Utils
{
    public static class ObjectExtension
    {
        public static void InheritedPropertyMap<T, TU>(this T source, TU destination)
            where T : class, new()
            where TU : class, new()
        {
            List<PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList();
            List<PropertyInfo> destinationProperties = destination.GetType().GetProperties().ToList();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                if (sourceProperty.Name != "Id")
                {
                    PropertyInfo destinationProperty = destinationProperties.Find(item => item.Name == sourceProperty.Name);

                    if (destinationProperty != null)
                    {
                        try
                        {
                            destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                }
            }
        }

        public static T DeleteNestedProperties<T>(this T entity)
            where T : class, new()
        {
            var entityType = typeof(T);
            foreach (var property in entityType.GetProperties().Where(p =>
                                     p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                entityType.GetProperty(property.Name).SetValue(entity, null);
            }

            return entity;
        }
    }
}
