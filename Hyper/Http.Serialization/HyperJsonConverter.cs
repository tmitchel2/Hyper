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

            // Deserialise members
            foreach (var member in dictionary)
            {
                var prop = obj
                    .GetType()
                    .GetProperties()
                    .SingleOrDefault(p => IsMember(p) && GetMemberName(p) == member.Key);

                if (prop != null)
                {
                    SetMemberValue(obj, prop, member.Value);
                }
            }

            // Deserialise links
            object linkObjs;
            if (dictionary.TryGetValue("_links", out linkObjs))
            {
                var links = (IDictionary<string, object>)linkObjs;
                foreach (var link in links)
                {
                    var prop = obj
                        .GetType()
                        .GetProperties()
                        .SingleOrDefault(p => IsLink(p) && GetLinkName(p) == link.Key);

                    if (prop != null)
                    {
                        SetLinkValue(obj, prop, link.Value, serializer);
                    }
                }
            }

            object embeddedObjs;
            if (dictionary.TryGetValue("_embedded", out embeddedObjs))
            {
                var embeddeds = (IDictionary<string, object>)embeddedObjs;
                foreach (var embedded in embeddeds)
                {
                    var prop = obj
                        .GetType()
                        .GetProperties()
                        .SingleOrDefault(p => IsEmbedded(p) && GetEmbeddedName(p) == embedded.Key);

                    if (prop != null)
                    {
                        SetEmbeddedValue(obj, prop, embedded.Value, serializer);
                    }
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
                .Where(prop => IsLinksSerialised(prop, obj))
                .ToDictionary(GetLinkName, f => GetLinkValue(f, obj, serializer));

            if (links.Any())
            {
                items.Add("_links", links);
            }

            // Serialise embedded
            var embedded = obj
                .GetType()
                .GetProperties()
                .Where(prop => IsEmbeddedSerialised(prop, obj))
                .ToDictionary(GetEmbeddedName, f => GetEmbeddedValue(f, obj, serializer));

            if (embedded.Any())
            {
                items.Add("_embedded", embedded);
            }

            return items;
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
        /// Determines whether the specified prop is embedded.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns>
        ///   <c>true</c> if the specified prop is embedded; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEmbedded(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<HyperEmbeddedAttribute>() != null;
        }

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="prop">The prop.</param>
        /// <param name="value">The value.</param>
        private static void SetMemberValue(object obj, PropertyInfo prop, object value)
        {
            value = CustomDeserialize(prop.PropertyType, value);
            prop.GetSetMethod().Invoke(obj, new[] { value });
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
        private static bool IsLinksSerialised(PropertyInfo prop, object obj)
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
        /// Determines whether [is embedded serialised] [the specified prop].
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///   <c>true</c> if [is embedded serialised] [the specified prop]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEmbeddedSerialised(PropertyInfo prop, object obj)
        {
            var attr = prop.GetCustomAttribute<HyperEmbeddedAttribute>();
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
        /// Gets the name of the embedded.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <returns></returns>
        private static string GetEmbeddedName(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<HyperEmbeddedAttribute>();
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
        /// Customs the deserialize.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static object CustomDeserialize(Type type, object data)
        {
            if (type == typeof(Version))
            {
                return Version.Parse((string)data);
            }

            if (type == typeof(Guid))
            {
                return Guid.Parse((string)data);
            }

            return data;
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
                    return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Revision, version.Build);

                default:
                    return obj;
            }
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

            SetMemberValue(obj, prop, value);
        }

        /// <summary>
        /// Sets the embedded value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="prop">The prop.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        private void SetEmbeddedValue(object obj, PropertyInfo prop, object value, JavaScriptSerializer serializer)
        {
            if (value is IDictionary<string, object>)
            {
                value = Deserialize(value as IDictionary<string, object>, prop.PropertyType, serializer);
            }

            SetMemberValue(obj, prop, value);
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
            if (value is IList<HyperLink>)
            {
                return (value as IList<HyperLink>).Select(f => Serialize(f, serializer)).ToList();
            }
            
            return Serialize(value, serializer);
        }

        /// <summary>
        /// Gets the embedded value.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        private object GetEmbeddedValue(PropertyInfo prop, object obj, JavaScriptSerializer serializer)
        {
            var value = prop.GetGetMethod().Invoke(obj, new object[] { });
            if (value is IList<HyperMember>)
            {
                return (value as IList<HyperMember>).Select(f => Serialize(f, serializer)).ToList();
            }
            
            if (value is IList<HyperLink>)
            {
                return (value as IList<HyperLink>).Select(f => Serialize(f, serializer)).ToList();
            }

            return Serialize(value, serializer);
        }
    }
}