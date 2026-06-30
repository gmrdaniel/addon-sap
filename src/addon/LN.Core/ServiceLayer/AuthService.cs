using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LN.Core.ServiceLayer
{
    /// <summary>
    /// Gestiona el login y la renovación de la sesión del Service Layer.
    /// Centraliza el manejo de sesión para resolver vencimientos y ROUTEID
    /// en alta disponibilidad (riesgo identificado en la arquitectura).
    /// </summary>
    public sealed class AuthService
    {
        private readonly HttpClient _http;
        private readonly ServiceLayerOptions _options;
        private readonly SemaphoreSlim _loginLock = new SemaphoreSlim(1, 1);
        private ServiceLayerSession? _session;

        public AuthService(HttpClient http, ServiceLayerOptions options)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>Devuelve una sesión válida, haciendo login si hace falta.</summary>
        public async Task<ServiceLayerSession> GetSessionAsync(CancellationToken ct = default)
        {
            if (_session is { IsExpired: false })
            {
                return _session;
            }

            return await LoginAsync(ct).ConfigureAwait(false);
        }

        /// <summary>Fuerza un nuevo login (p. ej. tras un 401 por sesión vencida).</summary>
        public async Task<ServiceLayerSession> LoginAsync(CancellationToken ct = default)
        {
            await _loginLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_session is { IsExpired: false })
                {
                    return _session;
                }

                var payload = JsonSerializer.Serialize(new
                {
                    CompanyDB = _options.CompanyDb,
                    UserName = _options.UserName,
                    Password = _options.Password
                });

                using var content = new StringContent(payload, Encoding.UTF8, "application/json");
                using var response = await _http.PostAsync("Login", content, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                using var doc = JsonDocument.Parse(body);

                var sessionId = doc.RootElement.GetProperty("SessionId").GetString()
                    ?? throw new InvalidOperationException("El Service Layer no devolvió SessionId.");
                var timeout = doc.RootElement.TryGetProperty("SessionTimeout", out var t)
                    ? t.GetInt32()
                    : 30;

                var routeId = ExtractRouteId(response);

                _session = new ServiceLayerSession(sessionId, routeId, timeout);
                return _session;
            }
            finally
            {
                _loginLock.Release();
            }
        }

        public void Invalidate() => _session = null;

        private static string? ExtractRouteId(HttpResponseMessage response)
        {
            if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                return null;
            }

            foreach (var cookie in cookies)
            {
                const string key = "ROUTEID=";
                var idx = cookie.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                {
                    continue;
                }

                var start = idx + key.Length;
                var end = cookie.IndexOf(';', start);
                return end < 0 ? cookie.Substring(start) : cookie.Substring(start, end - start);
            }

            return null;
        }
    }
}
