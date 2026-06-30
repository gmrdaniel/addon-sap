using System.Collections.Generic;

namespace LN.Setup
{
    /// <summary>
    /// Definiciones de las tablas (UDT) y campos (UDF) del add-on.
    /// Fuente única de verdad del esquema; se materializa vía metadatos del Service Layer.
    /// Ver modelo de datos en README.md / documento de arquitectura.
    /// </summary>
    public static class MetadataDefinitions
    {
        // BoUTBTableType: 0 = bott_NoObject (tabla de datos de usuario).
        private const int NoObject = 0;

        public static IReadOnlyList<UserTableDef> Tables { get; } = new List<UserTableDef>
        {
            new UserTableDef("LN_CONFIG", "Configuracion general del add-on", NoObject, new[]
            {
                new UserFieldDef("Key",   "Clave",  "db_Alpha", 100),
                new UserFieldDef("Value", "Valor",  "db_Memo",  0),
                new UserFieldDef("Scope", "Ambito", "db_Alpha", 20),
            }),
            new UserTableDef("LN_PERFILES", "Asignacion de perfil por usuario", NoObject, new[]
            {
                new UserFieldDef("UserKey",  "Usuario",  "db_Alpha", 50),
                new UserFieldDef("Profile",  "Perfil",   "db_Alpha", 20),
                new UserFieldDef("Quota",    "Cuota",    "db_Numeric", 11),
                new UserFieldDef("ValidTo",  "Vigencia", "db_Date", 0),
            }),
            new UserTableDef("LN_DOCMAP", "Mapeo del documento a generar", NoObject, new[]
            {
                new UserFieldDef("SourceField", "Campo origen",  "db_Alpha", 100),
                new UserFieldDef("TargetField", "Campo destino", "db_Alpha", 100),
                new UserFieldDef("DocType",     "Tipo doc",      "db_Alpha", 20),
            }),
            new UserTableDef("LN_PERSONA", "Instrucciones del modelo por ambito", NoObject, new[]
            {
                new UserFieldDef("Scope",      "Ambito",            "db_Alpha", 20),
                new UserFieldDef("SystemMsg",  "Mensaje de sistema", "db_Memo", 0),
                new UserFieldDef("Params",     "Parametros",        "db_Memo", 0),
            }),
            new UserTableDef("LN_USOLOG", "Bitacora de uso y medicion local", NoObject, new[]
            {
                new UserFieldDef("UserKey",   "Usuario",   "db_Alpha", 50),
                new UserFieldDef("Operation", "Operacion", "db_Alpha", 50),
                new UserFieldDef("Date",      "Fecha",     "db_Date", 0),
                new UserFieldDef("Cost",      "Costo est", "db_Numeric", 11),
            }),
            new UserTableDef("LN_TRAZA", "Trazabilidad imagen a documento", NoObject, new[]
            {
                new UserFieldDef("ImageRef", "Ruta o ID imagen", "db_Alpha", 254),
                new UserFieldDef("DocType",  "Tipo doc",         "db_Alpha", 20),
                new UserFieldDef("DocNum",   "Numero doc",       "db_Numeric", 11),
                new UserFieldDef("Status",   "Estado",           "db_Alpha", 20),
            }),
        };
    }

    /// <summary>Definición de una tabla de usuario (UDT).</summary>
    public sealed class UserTableDef
    {
        public string Name { get; }
        public string Description { get; }
        public int TableType { get; }
        public IReadOnlyList<UserFieldDef> Fields { get; }

        public UserTableDef(string name, string description, int tableType, IReadOnlyList<UserFieldDef> fields)
        {
            Name = name;
            Description = description;
            TableType = tableType;
            Fields = fields;
        }
    }

    /// <summary>Definición de un campo de usuario (UDF) dentro de una UDT.</summary>
    public sealed class UserFieldDef
    {
        public string Name { get; }
        public string Description { get; }
        public string FieldType { get; }
        public int Size { get; }

        public UserFieldDef(string name, string description, string fieldType, int size)
        {
            Name = name;
            Description = description;
            FieldType = fieldType;
            Size = size;
        }
    }
}
