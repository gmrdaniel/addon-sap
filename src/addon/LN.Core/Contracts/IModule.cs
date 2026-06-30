namespace LN.Core.Contracts
{
    /// <summary>
    /// Contrato de un módulo (solución) del add-on.
    /// Cada módulo se registra en el host común y se enchufa sin modificar
    /// el host ni los demás módulos.
    /// </summary>
    public interface IModule
    {
        /// <summary>Identificador único y estable del módulo. Ej: "config", "imagenes-ia".</summary>
        string Id { get; }

        /// <summary>Texto que se muestra en el submenú del cliente nativo.</summary>
        string MenuCaption { get; }

        /// <summary>Registra las entradas de menú del módulo bajo el menú raíz del add-on.</summary>
        void RegisterMenu(IMenuContext ctx);

        /// <summary>
        /// Se invoca cuando el usuario hace clic en una entrada de menú propiedad del módulo.
        /// </summary>
        /// <param name="menuUid">UID de la entrada de menú pulsada.</param>
        void OnMenuClick(string menuUid);

        /// <summary>Indica si el módulo es dueño de la entrada de menú indicada.</summary>
        bool OwnsMenu(string menuUid);
    }
}
