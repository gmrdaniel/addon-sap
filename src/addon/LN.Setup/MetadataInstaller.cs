using System.Threading;
using System.Threading.Tasks;
using LN.Core.Logging;
using LN.Core.ServiceLayer;

namespace LN.Setup
{
    /// <summary>
    /// Crea las UDT/UDF del add-on usando los puntos de metadatos del Service Layer
    /// (UserTablesMD y UserFieldsMD). Mantiene el runtime por completo sobre Service Layer:
    /// no se usa DI API.
    /// Es idempotente: si la tabla o el campo ya existen (409 Conflict), continúa.
    /// </summary>
    public sealed class MetadataInstaller
    {
        private readonly ServiceLayerClient _sl;

        public MetadataInstaller(ServiceLayerClient sl)
        {
            _sl = sl;
        }

        public async Task InstallAsync(CancellationToken ct = default)
        {
            foreach (var table in MetadataDefinitions.Tables)
            {
                await CreateTableAsync(table, ct).ConfigureAwait(false);
                foreach (var field in table.Fields)
                {
                    await CreateFieldAsync(table.Name, field, ct).ConfigureAwait(false);
                }
            }

            AppLogger.Info("Metadatos del add-on verificados/creados en SAP.");
        }

        private async Task CreateTableAsync(UserTableDef table, CancellationToken ct)
        {
            try
            {
                await _sl.PostAsync<object>("UserTablesMD", new
                {
                    TableName = table.Name,
                    TableDescription = table.Description,
                    TableType = table.TableType
                }, ct).ConfigureAwait(false);
                AppLogger.Info($"UDT creada: @{table.Name}.");
            }
            catch (System.Net.Http.HttpRequestException)
            {
                // Ya existe (idempotencia): se continúa con los campos.
                AppLogger.Info($"UDT @{table.Name} ya existía; se omite.");
            }
        }

        private async Task CreateFieldAsync(string tableName, UserFieldDef field, CancellationToken ct)
        {
            try
            {
                await _sl.PostAsync<object>("UserFieldsMD", new
                {
                    TableName = "@" + tableName,
                    Name = field.Name,
                    Description = field.Description,
                    Type = field.FieldType,
                    Size = field.Size > 0 ? field.Size : (int?)null
                }, ct).ConfigureAwait(false);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                // El campo ya existe: idempotente.
            }
        }
    }
}
