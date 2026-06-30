using System;
using SAPbouiCOM;

namespace LN.Host
{
    /// <summary>
    /// Establece la conexión con el cliente nativo de SAP Business One vía UI API.
    /// El connection string lo entrega el cliente como argumento al lanzar el add-on;
    /// en depuración se usa la cadena por defecto del SDK.
    /// </summary>
    public static class SapConnection
    {
        // Cadena por defecto para conectar el add-on a un cliente B1 abierto durante el desarrollo.
        private const string DebugConnectionString =
            "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";

        public static Application Connect(string[] args)
        {
            var connectionString = args is { Length: > 0 } ? args[0] : DebugConnectionString;

            var guiApi = new SboGuiApi();
            guiApi.Connect(connectionString);

            var app = guiApi.GetApplication(-1);
            if (app is null)
            {
                throw new InvalidOperationException("No se pudo obtener la aplicación de SAP Business One.");
            }

            return app;
        }
    }
}
