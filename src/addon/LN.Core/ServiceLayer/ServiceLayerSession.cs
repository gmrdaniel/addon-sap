using System;

namespace LN.Core.ServiceLayer
{
    /// <summary>
    /// Estado de una sesión activa del Service Layer.
    /// Incluye el identificador de sesión (B1SESSION) y el ROUTEID,
    /// necesario para mantener afinidad de nodo en instalaciones de alta disponibilidad.
    /// </summary>
    public sealed class ServiceLayerSession
    {
        public string SessionId { get; }

        /// <summary>ROUTEID para afinidad de nodo en alta disponibilidad (puede ser null).</summary>
        public string? RouteId { get; }

        /// <summary>Momento de expiración estimado de la sesión.</summary>
        public DateTimeOffset ExpiresAt { get; }

        public ServiceLayerSession(string sessionId, string? routeId, int sessionTimeoutMinutes)
        {
            SessionId = sessionId ?? throw new ArgumentNullException(nameof(sessionId));
            RouteId = routeId;
            // Se renueva con margen antes del timeout real informado por el Service Layer.
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(Math.Max(1, sessionTimeoutMinutes - 1));
        }

        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

        /// <summary>Cabecera Cookie a enviar en cada petición autenticada.</summary>
        public string ToCookieHeader()
        {
            return RouteId is { Length: > 0 }
                ? $"B1SESSION={SessionId}; ROUTEID={RouteId}"
                : $"B1SESSION={SessionId}";
        }
    }
}
