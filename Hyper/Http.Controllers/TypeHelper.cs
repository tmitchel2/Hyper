using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Hyper.Http.Controllers
{
    /// <summary>
    /// TypeHelperTypeHelper class.
    /// </summary>
    internal static class TypeHelper
    {
        internal static readonly Type ApiControllerType = typeof(ApiController);

        internal static readonly Type HttpControllerType = typeof(IHttpController);

        private static readonly Type TaskGenericType = typeof(Task<>);
        
        /// <summary>
        /// Extracts the generic interface.
        /// </summary>
        /// <param name="queryType">Type of the query.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns></returns>
        internal static Type ExtractGenericInterface(Type queryType, Type interfaceType)
        {
            Func<Type, bool> predicate = t =>
                {
                    if (t.IsGenericType)
                    {
                        return t.GetGenericTypeDefinition() == interfaceType;
                    }
                    
                    return false;
                };
            
            if (!predicate(queryType))
            {
                return queryType.GetInterfaces().FirstOrDefault(predicate);
            }
            
            return queryType;
        }

        /// <summary>
        /// Gets the task inner type or null.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal static Type GetTaskInnerTypeOrNull(Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (TaskGenericType == genericTypeDefinition)
                {
                    return type.GetGenericArguments()[0];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the type arguments if match.
        /// </summary>
        /// <param name="closedType">Type of the closed.</param>
        /// <param name="matchingOpenType">Type of the matching open.</param>
        /// <returns></returns>
        internal static Type[] GetTypeArgumentsIfMatch(Type closedType, Type matchingOpenType)
        {
            if (!closedType.IsGenericType)
            {
                return null;
            }
            Type genericTypeDefinition = closedType.GetGenericTypeDefinition();
            if (!(matchingOpenType == genericTypeDefinition))
            {
                return null;
            }
            else
            {
                return closedType.GetGenericArguments();
            }
        }

        /// <summary>
        /// Determines whether [has string converter] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [has string converter] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool HasStringConverter(Type type)
        {
            return TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }

        /// <summary>
        /// Determines whether [is compatible object] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is compatible object] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsCompatibleObject(Type type, object value)
        {
            if (value != null || !TypeAllowsNullValue(type))
            {
                return type.IsInstanceOfType(value);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether [is nullable value type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is nullable value type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Determines whether [is simple type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsSimpleType(Type type)
        {
            if (!type.IsPrimitive && !(type == typeof(string))
                && (!(type == typeof(DateTime)) && !(type == typeof(decimal)))
                && (!(type == typeof(Guid)) && !(type == typeof(DateTimeOffset))))
            {
                return type == typeof(TimeSpan);
            }
            
            return true;
        }

        /// <summary>
        /// Determines whether [is simple underlying type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is simple underlying type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsSimpleUnderlyingType(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                type = underlyingType;
            }
            return IsSimpleType(type);
        }

        /// <summary>
        /// Ofs the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        internal static ReadOnlyCollection<T> OfType<T>(object[] objects) where T : class
        {
            int length = objects.Length;
            var list = new List<T>(length);
            int num = 0;
            for (int index = 0; index < length; ++index)
            {
                var obj = objects[index] as T;
                if (obj != null)
                {
                    list.Add(obj);
                    ++num;
                }
            }
            list.Capacity = num;
            return new ReadOnlyCollection<T>(list);
        }

        /// <summary>
        /// Types the allows null value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal static bool TypeAllowsNullValue(Type type)
        {
            if (type.IsValueType)
            {
                return IsNullableValueType(type);
            }
            
            return true;
        }
    }
}