namespace LN.Core.Contracts
{
    /// <summary>
    /// Abstracción del registro de menús que el host expone a los módulos.
    /// Evita que cada módulo dependa directamente de la UI API de SAP.
    /// </summary>
    public interface IMenuContext
    {
        /// <summary>UID del menú raíz del add-on bajo el que cuelgan los módulos.</summary>
        string RootMenuUid { get; }

        /// <summary>
        /// Agrega una entrada de submenú bajo el menú raíz del add-on.
        /// </summary>
        /// <param name="uid">UID único de la entrada.</param>
        /// <param name="caption">Texto visible.</param>
        /// <param name="parentUid">UID del menú padre. Si es null, usa el menú raíz del add-on.</param>
        void AddMenuItem(string uid, string caption, string? parentUid = null);

        /// <summary>
        /// Agrega un submenú contenedor (folder) que puede tener hijos.
        /// </summary>
        void AddSubMenu(string uid, string caption, string? parentUid = null);
    }
}
