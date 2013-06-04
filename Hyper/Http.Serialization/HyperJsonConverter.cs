using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Hyper.Http.Serialization
{
    /// <summary>
    /// HyperJsonConverter class.
    /// </summary>
    public class HyperJsonConverter : JavaScriptConverter
    {
        private readonly IList<Type> _supportedTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperJsonConverter" /> class.
        /// </summary>
        /// <param name="supportedTypes">The supported types.</param>
        public HyperJsonConverter(IEnumerable<Type> supportedTypes)
        {
            _supportedTypes = supportedTypes.ToList();
        }

        /// <summary>
        /// When overridden in a derived class, gets a collection of the supported types.
        /// </summary>
        /// <returns>An object that implements <see cref="T:System.Collections.Generic.IEnumerable`1" /> that represents the types supported by the converter.</returns>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _supportedTypes;
            }
        }

        /// <summary>
        /// Deserializes the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="type">The type.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var obj = Activator.CreateInstance(type);
            foreach (var item in dictionary)
            {
                // Deserialise member
                var memberProp = obj
                    .GetType()
                    .GetProperties()
                    .SingleOrDefault(p => IsMember(p) && GetMemberName(p) == item.Key);

                if (memberProp != null)
                {
                    SetMemberValue(obj, memberProp, item.Value, serializer);
                }

                // Deserialise link
                var linkProp = obj
                        .GetType()
                        .GetProperties()
                        .SingleOrDefault(p => IsLink(p) && GetLinkName(p) == item.Key);

                if (linkProp != null)
                {
                    SetLinkValue(obj, linkProp, item.Value, serializer);
                }
            }

            return obj;
        }

        /// <summary>
        /// When overridden in a derived class, builds a dictionary of name/value pairs.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The object that is responsible for the serialization.</param>
        /// <returns>
        /// An object that contains key/value pairs that represent the object’s data.
        /// </returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            if (obj == null)
            {
                return new Dictionary<string, object>();
            }

            // Serialise members
            var items = obj
                .GetType()
                .GetProperties()
                .Where(prop => IsMemberSerialised(prop, obj))
                .ToDictionary(GetMemberName, prop => GetMemberValue(prop, obj));

            // Serialise links
            var links = obj
                .GetType()
                .GetProperties()
                .Where(prop => IsLinkSerialised(prop, obj))
                .ToDictionary(GetLinkName, f => GetLinkValue(f, obj, serializer));

            // Merge dictionaries
            return items
                .Concat(links)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }
        
        /// <summary>
        /// Determines whether the specified prop is link.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns>
        ///   <c>true</c> if the specified prop is link; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsLink(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<HyperLinkAttribute>() != null;
        }

        /// <summary>
        /// Determines whether the specified prop is member.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns>
        ///   <c>true</c> if the specified prop is member; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMember(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<HyperMemberAttribute>() != null;
        }

        /// <summary>
        /// Determines whether [is member serialised] [the specified prop].
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///   <c>true</c> if [is member serialised] [the specified prop]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMemberSerialised(PropertyInfo prop, object obj)
        {
            var attr = prop.GetCustomAttribute<HyperMemberAttribute>();
            if (attr == null)
            {
                return false;
            }

            if (!attr.IsOptional)
            {
                return true;
            }

            var defaultValue = GetDefaultValue(prop.GetGetMethod().ReturnType);
            var memberValue = GetMemberValue(prop, obj);
            if ((defaultValue == null && memberValue == null) || (defaultValue != null && defaultValue.Equals(memberValue)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether [is links serialised] [the specified prop].
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///   <c>true</c> if [is links serialised] [the specified prop]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsLinkSerialised(PropertyInfo prop, object obj)
        {
            var attr = prop.GetCustomAttribute<HyperLinkAttribute>();
            if (attr == null)
            {
                return false;
            }

            var value = prop.GetGetMethod().Invoke(obj, new object[] { });
            if (value is IList)
            {
                return (value as IList).Count > 0;
            }

            return true;
        }
        
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Gets the name of the link.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns></returns>
        private static string GetLinkName(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<HyperLinkAttribute>();
            return attr.Rel ?? prop.Name;
        }
        
        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private static object GetMemberValue(PropertyInfo prop, object obj)
        {
            var value = prop.GetGetMethod().Invoke(obj, new object[] { });
            return CustomSerialize(value);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns></returns>
        private static string GetMemberName(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<HyperMemberAttribute>();
            return attr.Name ?? prop.Name;
        }

        /// <summary>
        /// Customs the serialize.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private static object CustomSerialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            switch (obj.GetType().FullName)
            {
                case "System.Version":
                    var version = (Version)obj;
                    return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

                default:
                    return obj;
            }
        }

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="prop">The prop.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        private void SetMemberValue(object obj, PropertyInfo prop, object value, JavaScriptSerializer serializer)
        {
            value = CustomDeserialize(prop.PropertyType, value, serializer);
            prop.GetSetMethod().Invoke(obj, new[] { value });
        }

        /// <summary>
        /// Customs the deserialize.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>The custom deserializer.</returns>
        private object CustomDeserialize(Type type, object data, JavaScriptSerializer serializer)
        {
            if (type == typeof(Version))
            {
                return Version.Parse((string)data);
            }

            if (type == typeof(Guid))
            {
                return Guid.Parse((string)data);
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                var itemType = type.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(itemType);
                var list = (IList)Activator.CreateInstance(listType);

                var array = (ArrayList)data;
                foreach (IDictionary<string, object> item in array)
                {
                    var obj = Deserialize(item, itemType, serializer);
                    list.Add(obj);
                }

                return list;
            }

            return data;
        }

        /// <summary>
        /// Sets the link value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="prop">The prop.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        private void SetLinkValue(object obj, PropertyInfo prop, object value, JavaScriptSerializer serializer)
        {
            if (value is IDictionary<string, object>)
            {
                value = Deserialize(value as IDictionary<string, object>, prop.PropertyType, serializer);
            }

            SetMemberValue(obj, prop, value, serializer);
        }

        /// <summary>
        /// Gets the link value.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        private object GetLinkValue(PropertyInfo prop, object obj, JavaScriptSerializer serializer)
        {
            var value = prop.GetGetMethod().Invoke(obj, new object[] { });
            if (value is IList<HyperLink<object>>)
            {
                return (value as IList<HyperLink<object>>).Select(f => Serialize(f, serializer)).ToList();
            }
            
            return Serialize(value, serializer);
        }
    }
}