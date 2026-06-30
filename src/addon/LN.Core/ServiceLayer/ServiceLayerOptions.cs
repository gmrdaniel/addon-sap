namespace LN.Core.ServiceLayer
{
    /// <summary>
    /// Parámetros de conexión al SAP Service Layer.
    /// </summary>
    public sealed class ServiceLayerOptions
    {
        /// <summary>Base URL del Service Layer. Ej: https://host:50000/b1s/v2/</summary>
        public string BaseUrl { get; set; } = "https://localhost:50000/b1s/v2/";

        /// <summary>Código de la base de datos de la compañía. Ej: SBODEMOMX.</summary>
        public string CompanyDb { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        /// <summary>Permite certificados auto-firmados en entornos de desarrollo.</summary>
        public bool AllowSelfSignedCertificate { get; set; } = true;

        /// <summary>Tiempo máximo de espera por petición, en segundos.</summary>
        public int TimeoutSeconds { get; set; } = 100;

        /// <summary>Número máximo de reintentos ante sesión vencida (401).</summary>
        public int MaxRetries { get; set; } = 1;
    }
}
