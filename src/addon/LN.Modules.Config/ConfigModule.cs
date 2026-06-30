using LN.Core.Contracts;

namespace LN.Modules.Config
{
    /// <summary>
    /// Módulo 1 — Administrador de Configuraciones.
    /// Permisos y licencias, selección de documento, alta de maestros,
    /// uso por usuario y personalidad del modelo.
    /// (Esqueleto: los formularios se implementan en la Fase 2.)
    /// </summary>
    public sealed class ConfigModule : IModule
    {
        public const string MenuUid = "LN_CFG";

        public string Id => "config";

        public string MenuCaption => "Administrador de Configuraciones";

        public void RegisterMenu(IMenuContext ctx)
        {
            ctx.AddMenuItem(MenuUid, MenuCaption);
        }

        public bool OwnsMenu(string menuUid) => menuUid == MenuUid;

        public void OnMenuClick(string menuUid)
        {
            // TODO (Fase 2): abrir el formulario del Administrador de Configuraciones.
        }
    }
}
