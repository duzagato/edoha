using Edoha.Domain.Interfaces.Infraestructure.Util;
using System.Text.Json;

namespace Edoha.Infraestructure.Util
{
    public class Json : IJson
    {
        public string Serialize<T>(T obj)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(obj, options);
        }
    }
}
