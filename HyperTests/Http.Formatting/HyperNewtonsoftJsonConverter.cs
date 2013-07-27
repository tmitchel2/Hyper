using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hyper;
using Hyper.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HyperTests.Http.Formatting
{
    /// <summary>
    /// HyperNewtonsoftJsonConverter class.
    /// </summary>
    public class HyperNewtonsoftJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return MediaTypeFormatterExtensions.CanReadAndWriteType(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            var obj = Serialise(value, serializer);
            obj.WriteTo(writer);
        }

        private static JObject Serialise(object value, JsonSerializer serializer)
        {
            var obj = new JObject();

            // Serialise members
            var items =
                value.GetType().GetProperties().Where(prop => IsMemberSerialised(prop, value)).ToDictionary(
                    GetMemberName, prop => GetMemberValue(prop, value));

            foreach (var item in items)
            {
                obj.Add(item.Key, new JValue(item.Value));
            }

            // Serialise links
            var links = value.GetType().GetProperties().Where(prop => IsLinkSerialised(prop, value)).ToDictionary(
                GetLinkName, f => GetLinkValue(f, value, serializer));

            foreach (var link in links)
            {
                obj.Add(link.Key, link.Value);
            }

            return obj;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            return Deserialise(objectType, serializer, jObject);
        }

        private object Deserialise(Type objectType, JsonSerializer serializer, JObject jObject)
        {
            var obj = Activator.CreateInstance(objectType);

            foreach (var item in jObject)
            {
                // Deserialise member
                var memberProp = obj.GetType().GetProperties().SingleOrDefault(p => IsMember(p) && GetMemberName(p) == item.Key);

                if (memberProp != null)
                {
                    SetMemberValue(obj, memberProp, item.Value.ToObject<object>(), serializer);
                }

                // Deserialise link
                var linkProp = obj.GetType().GetProperties().SingleOrDefault(p => IsLink(p) && GetLinkName(p) == item.Key);

                if (linkProp != null)
                {
                    SetLinkValue(obj, linkProp, (JObject) item.Value, serializer);
                }
            }

            return obj;
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
        private void SetMemberValue(object obj, PropertyInfo prop, object value, JsonSerializer serializer)
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
        /// <returns>
        /// The custom deserializer.
        /// </returns>
        private object CustomDeserialize(Type type, object data, JsonSerializer serializer)
        {
            if (type == typeof(int) && data.GetType() == typeof(long))
            {
                return (int) (long) data;
            }

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
                foreach (var item in array)
                {
                    var obj = Deserialise(itemType, serializer, (JObject)item);
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
        private void SetLinkValue(object obj, PropertyInfo prop, JObject value, JsonSerializer serializer)
        {
            var val = Deserialise(prop.PropertyType, serializer, value);
            SetMemberValue(obj, prop, val, serializer);
        }

        /// <summary>
        /// Gets the link value.
        /// </summary>
        /// <param name="prop">The prop.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        private static JObject GetLinkValue(PropertyInfo prop, object obj, JsonSerializer serializer)
        {
            var value = prop.GetGetMethod().Invoke(obj, new object[] { });
            if (value is IList<HyperLink<object>>)
            {
                return new JObject((value as IList<HyperLink<object>>).Select(f => Serialise(f, serializer)).ToList());
            }
            
            return Serialise(value, serializer);
        }
    }
}