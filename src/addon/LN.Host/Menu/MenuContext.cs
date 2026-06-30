using LN.Core.Contracts;
using SAPbouiCOM;

namespace LN.Host.Menu
{
    /// <summary>
    /// Implementación de IMenuContext sobre la UI API de SAP Business One.
    /// Traduce las peticiones de los módulos en altas reales de menús del cliente.
    /// </summary>
    public sealed class MenuContext : IMenuContext
    {
        private readonly Application _app;

        public MenuContext(Application app, string rootMenuUid)
        {
            _app = app;
            RootMenuUid = rootMenuUid;
        }

        public string RootMenuUid { get; }

        public void AddMenuItem(string uid, string caption, string? parentUid = null)
            => Add(uid, caption, parentUid ?? RootMenuUid, BoMenuType.mt_STRING);

        public void AddSubMenu(string uid, string caption, string? parentUid = null)
            => Add(uid, caption, parentUid ?? RootMenuUid, BoMenuType.mt_POPUP);

        private void Add(string uid, string caption, string parentUid, BoMenuType type)
        {
            var menus = _app.Menus;
            if (menus.Exists(uid))
            {
                return;
            }

            var parent = menus.Item(parentUid);
            var creationPackage = (MenuCreationParams)_app.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
            creationPackage.Type = type;
            creationPackage.UniqueID = uid;
            creationPackage.String = caption;
            creationPackage.Enabled = true;

            parent.SubMenus.AddEx(creationPackage);
        }
    }
}
