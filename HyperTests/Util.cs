using System.IO;
using Newtonsoft.Json;

namespace HyperTests
{
    public static class Util
    {
        public static string ToIndentedJson(string json)
        {
            var obj = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static string GetResource(string path)
        {
            var assem = typeof(Util).Assembly;
            using (var stream = assem.GetManifestResourceStream(path))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}