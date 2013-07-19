using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Hyper;

namespace HyperTests.Controllers
{
    public class TypeController : ApiController
    {
        private IDictionary<string, HyperType> _types;

        [AllowAnonymous]
        public HyperList<HyperType> Get()
        {
            return new HyperList<HyperType>(new HyperLink<HyperList<HyperType>>(GetRoute("Types")), Types);
        }

        [AllowAnonymous]
        public HyperType Get(string id)
        {
            var hyperType = Types.First(type => type.Name == id);
            return hyperType;
        }

        private IEnumerable<HyperType> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new Dictionary<string, HyperType>();
                    var assemblies = new[] { typeof(HyperType).Assembly, typeof(Application).Assembly };
                    var types = assemblies
                        .SelectMany(ass => ass.GetExportedTypes())
                        .Where(f => !f.IsAbstract && f.IsClass && f.GetCustomAttributes(typeof(HyperContractAttribute), true).Any())
                        .Select(ToType)
                        .OrderBy(type => type.Name)
                        .ToList();

                    foreach (var type in types)
                    {
                        if (!_types.ContainsKey(type.Name))
                        {
                            _types.Add(type.Name, type);
                        }
                    }
                }

                return _types.Values.OrderBy(t => t.Name).ToList();
            }
        }

        private HyperType ToType(Type type)
        {
            var typeName = GetTypeName(type);
            if (_types.ContainsKey(typeName))
            {
                return _types[typeName];
            }

            var hyperType = new HyperType
            {
                Name = typeName,
                MediaType = GetMediaType(type),
                Self = new HyperLink<HyperType>(GetRoute("Types", type.Name)),
                Members = GetMembers(type).ToList(),
                Links = GetLinks(type).ToList()
            };

            _types[hyperType.Name] = hyperType;
            return hyperType;
        }

        private string GetTypeName(Type type)
        {
            var attr = type.GetCustomAttribute<HyperContractAttribute>();
            var typeName = attr != null && !string.IsNullOrWhiteSpace(attr.Name) ? attr.Name : type.Name.ToLowerInvariant();
            return typeName;
        }

        private string GetMediaType(Type type)
        {
            var attr = type.GetCustomAttribute<HyperContractAttribute>();
            var typeName = attr != null && !string.IsNullOrWhiteSpace(attr.MediaType) ? attr.MediaType : "application/vnd.hyper." + type.Name.ToLowerInvariant();
            return typeName;
        }

        private IEnumerable<HyperMember> GetMembers(Type type)
        {
            return type
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<HyperMemberAttribute>() != null)
                .Select(prop => new HyperMember
                    {
                        Name = prop.GetCustomAttribute<HyperMemberAttribute>().Name,
                        HyperType = ToType(prop.GetGetMethod().ReturnType)
                    });
        }

        private static IEnumerable<HyperLink<HyperType>> GetLinks(Type type)
        {
            return type
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<HyperLinkAttribute>() != null)
                .Select(prop => prop.GetCustomAttribute<HyperLinkAttribute>().Rel)
                .Select(name => new HyperLink<HyperType> { Name = name });
        }

        private string GetRoute(string controller, string id = null)
        {
            return new Uri(Request.RequestUri, Url.Route("DefaultApi", new { controller, id })).ToString();
        }
    }
}