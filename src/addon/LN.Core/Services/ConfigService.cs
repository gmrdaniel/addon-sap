using System.Threading;
using System.Threading.Tasks;
using LN.Core.ServiceLayer;

namespace LN.Core.Services
{
    /// <summary>
    /// Lee y escribe la configuración del add-on guardada en la UDT @LN_CONFIG.
    /// El ámbito (empresa o usuario) permite parametrizar por nivel.
    /// </summary>
    public sealed class ConfigService
    {
        private const string ConfigTable = "LN_CONFIG"; // UDT @LN_CONFIG
        private readonly ServiceLayerClient _sl;

        public ConfigService(ServiceLayerClient sl)
        {
            _sl = sl;
        }

        /// <summary>Obtiene un valor de configuración por clave y ámbito.</summary>
        public async Task<string?> GetValueAsync(string key, string scope = "company", CancellationToken ct = default)
        {
            // Filtro OData sobre la UDT. Estructura final a ajustar con los UDF reales.
            var path = $"U_LN_CONFIG?$filter=U_Key eq '{key}' and U_Scope eq '{scope}'&$top=1";
            var result = await _sl.GetAsync<ODataList<ConfigRow>>(path, ct).ConfigureAwait(false);
            return result?.Value is { Count: > 0 } ? result.Value[0].U_Value : null;
        }

        private sealed class ConfigRow
        {
            public string? U_Key { get; set; }
            public string? U_Value { get; set; }
            public string? U_Scope { get; set; }
        }
    }
}
