using System.Text.Json;
using System.Text.Json.Nodes;

namespace DatabaseManagement.Extensions;

public static class JsonNodeExtension
{

    public static JsonNode GetOrThrow(this JsonNode? node)
    {
        if (node == null)
        {
            throw new JsonException();
        }
        return node;
    }

    public static TData GetOrThrow<TData>(this JsonObject obj, string propertyName)
    {
        return GetOrThrow(obj[propertyName]).GetValue<TData>();
    }
    
}