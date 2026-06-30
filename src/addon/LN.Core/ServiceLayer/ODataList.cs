using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LN.Core.ServiceLayer
{
    /// <summary>
    /// Envoltorio de una colección OData devuelta por el Service Layer:
    /// el arreglo viaja bajo la propiedad "value".
    /// </summary>
    /// <typeparam name="T">Tipo de cada elemento de la colección.</typeparam>
    public sealed class ODataList<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; } = new List<T>();

        [JsonPropertyName("odata.nextLink")]
        public string? NextLink { get; set; }
    }
}
