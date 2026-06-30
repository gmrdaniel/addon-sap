using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LN.Core.Services
{
    /// <summary>
    /// Cliente del backend en la nube para licencias y medición.
    /// El add-on nunca llama al modelo de IA directamente: pasa por el backend,
    /// que valida cuota, registra consumo (pospago) y resguarda la API key.
    /// </summary>
    public sealed class LicenseClient
    {
        private readonly HttpClient _http;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public LicenseClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        /// <summary>Consulta el perfil y la cuota disponible del usuario.</summary>
        public async Task<LicenseStatus?> GetStatusAsync(string userKey, CancellationToken ct = default)
        {
            using var response = await _http.GetAsync($"api/licenses/{Uri.EscapeDataString(userKey)}", ct)
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<LicenseStatus>(body, JsonOptions);
        }
    }

    /// <summary>Estado de licencia devuelto por el backend.</summary>
    public sealed class LicenseStatus
    {
        public string Profile { get; set; } = "Basico";
        public int QuotaTotal { get; set; }
        public int QuotaUsed { get; set; }
        public bool Active { get; set; }
        public int QuotaRemaining => Math.Max(0, QuotaTotal - QuotaUsed);
    }
}
