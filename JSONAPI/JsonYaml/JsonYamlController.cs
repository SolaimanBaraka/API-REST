using Microsoft.AspNetCore.Mvc;
using System.Text.Json; // Usar System.Text.Json
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections.Generic;

namespace JsonToYamlApi.JsonYaml
{
    [ApiController]
    [Route("api/[controller]")]
    public class JsonYamlController : ControllerBase
    {
        [HttpPost]
        public IActionResult ConvertJsonToYaml([FromBody] JsonElement jsonData)
        {
            var nativeObject = ConvertJsonElementToObject(jsonData);

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yamlContent = serializer.Serialize(nativeObject);

            return Ok(yamlContent);
        }

        private static object ConvertJsonElementToObject(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                var obj = new Dictionary<string, object>();
                foreach (var prop in element.EnumerateObject())
                {
                    obj[prop.Name] = ConvertJsonElementToObject(prop.Value);
                }
                return obj;
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                var list = new List<object>();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(ConvertJsonElementToObject(item));
                }
                return list;
            }
            else if (element.ValueKind == JsonValueKind.String)
            {
                return element.GetString();
            }
            else if (element.ValueKind == JsonValueKind.Number)
            {
                if (element.TryGetInt32(out int intValue))
                    return intValue;
                if (element.TryGetDouble(out double doubleValue))
                    return doubleValue;
                return element.GetRawText();
            }
            else if (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False)
            {
                return element.GetBoolean();
            }
            else if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}