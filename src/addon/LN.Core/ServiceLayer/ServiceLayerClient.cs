using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LN.Core.ServiceLayer
{
    /// <summary>
    /// Cliente central de acceso a datos de SAP por Service Layer (REST/OData v4).
    /// Toda la operación de datos del add-on pasa por aquí: sin DI API en runtime.
    /// Aplica la sesión vigente y reintenta automáticamente ante sesión vencida (401).
    /// </summary>
    public sealed class ServiceLayerClient
    {
        private readonly HttpClient _http;
        private readonly AuthService _auth;
        private readonly ServiceLayerOptions _options;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public ServiceLayerClient(HttpClient http, AuthService auth, ServiceLayerOptions options)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<T?> GetAsync<T>(string resourcePath, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Get, resourcePath, null, ct);

        public Task<T?> PostAsync<T>(string resourcePath, object body, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Post, resourcePath, body, ct);

        public Task<bool> PatchAsync(string resourcePath, object body, CancellationToken ct = default)
            => SendNoContentAsync(new HttpMethod("PATCH"), resourcePath, body, ct);

        public Task<bool> DeleteAsync(string resourcePath, CancellationToken ct = default)
            => SendNoContentAsync(HttpMethod.Delete, resourcePath, null, ct);

        private async Task<T?> SendAsync<T>(HttpMethod method, string resourcePath, object? body, CancellationToken ct)
        {
            using var response = await SendWithRetryAsync(method, resourcePath, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(payload))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(payload, JsonOptions);
        }

        private async Task<bool> SendNoContentAsync(HttpMethod method, string resourcePath, object? body, CancellationToken ct)
        {
            using var response = await SendWithRetryAsync(method, resourcePath, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return true;
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(
            HttpMethod method, string resourcePath, object? body, CancellationToken ct)
        {
            var attempt = 0;
            while (true)
            {
                var session = await _auth.GetSessionAsync(ct).ConfigureAwait(false);
                var request = BuildRequest(method, resourcePath, body, session);

                var response = await _http.SendAsync(request, ct).ConfigureAwait(false);

                // Sesión vencida: invalida, renueva y reintenta una vez.
                if (response.StatusCode == HttpStatusCode.Unauthorized && attempt < _options.MaxRetries)
                {
                    response.Dispose();
                    _auth.Invalidate();
                    attempt++;
                    continue;
                }

                return response;
            }
        }

        private static HttpRequestMessage BuildRequest(
            HttpMethod method, string resourcePath, object? body, ServiceLayerSession session)
        {
            var request = new HttpRequestMessage(method, resourcePath);
            request.Headers.TryAddWithoutValidation("Cookie", session.ToCookieHeader());

            if (body is not null)
            {
                var json = JsonSerializer.Serialize(body, JsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return request;
        }
    }
}
