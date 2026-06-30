using System;
using System.Collections.Generic;
using System.Linq;
using LN.Core.Contracts;
using LN.Core.Logging;
using LN.Host.Menu;
using SAPbouiCOM;

namespace LN.Host
{
    /// <summary>
    /// Aplicación del add-on. Conecta con la UI API, registra el menú raíz,
    /// descubre los módulos y enruta los eventos de menú al módulo dueño.
    /// </summary>
    public sealed class AddOnApplication
    {
        public const string RootMenuUid = "LN_ROOT";
        private const string SapMainModulesMenu = "43520"; // menú "Módulos" del cliente B1

        private readonly Application _app;
        private readonly IReadOnlyList<IModule> _modules;

        public AddOnApplication(Application app, IEnumerable<IModule> modules)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            _modules = modules.ToList();
        }

        /// <summary>Registra el menú raíz y deja que cada módulo cuelgue sus entradas.</summary>
        public void Initialize()
        {
            RegisterRootMenu();

            var ctx = new MenuContext(_app, RootMenuUid);
            foreach (var module in _modules)
            {
                module.RegisterMenu(ctx);
                AppLogger.Info($"Módulo registrado: {module.Id} ({module.MenuCaption}).");
            }

            _app.MenuEvent += OnMenuEvent;
            AppLogger.Info("Add-on inicializado y suscrito a eventos de menú.");
        }

        private void RegisterRootMenu()
        {
            var menus = _app.Menus;
            if (menus.Exists(RootMenuUid))
            {
                return;
            }

            var modulesMenu = menus.Item(SapMainModulesMenu);
            var creationPackage = (MenuCreationParams)_app.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
            creationPackage.Type = BoMenuType.mt_POPUP;
            creationPackage.UniqueID = RootMenuUid;
            creationPackage.String = "Add-on LaNeta";
            creationPackage.Enabled = true;

            modulesMenu.SubMenus.AddEx(creationPackage);
        }

        private void OnMenuEvent(ref MenuEvent pVal, out bool bubbleEvent)
        {
            bubbleEvent = true;
            if (pVal.BeforeAction)
            {
                return;
            }

            try
            {
                var module = _modules.FirstOrDefault(m => m.OwnsMenu(pVal.MenuUID));
                module?.OnMenuClick(pVal.MenuUID);
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Error al procesar el menú {pVal.MenuUID}.", ex);
                _app.MessageBox($"Error: {ex.Message}");
            }
        }
    }
}
