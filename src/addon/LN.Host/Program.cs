using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using LN.Core.Contracts;
using LN.Core.Logging;
using LN.Modules.Config;
using LN.Modules.ImagenesIA;
using SAPbouiCOM;

namespace LN.Host
{
    /// <summary>
    /// Punto de entrada del add-on. Inicializa logging, conecta al cliente,
    /// arranca los metadatos (UDT/UDF) y registra el menú con sus módulos.
    /// </summary>
    internal static class Program
    {
        private static Application? _app;

        [STAThread]
        private static void Main(string[] args)
        {
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LN.AddOn", "logs", "addon-.log");
            AppLogger.Initialize(logPath);
            AppLogger.Info("Arrancando add-on LaNeta.");

            try
            {
                _app = SapConnection.Connect(args);
                _app.AppEvent += OnAppEvent;

                var modules = new List<IModule>
                {
                    new ConfigModule(),
                    new ImagenesIaModule()
                };

                var addon = new AddOnApplication(_app, modules);
                addon.Initialize();

                _app.StatusBar.SetText(
                    "Add-on LaNeta cargado.",
                    BoMessageTime.bmt_Short,
                    BoStatusBarMessageType.smt_Success);

                // Mantiene vivo el proceso para recibir eventos del cliente.
                System.Windows.Forms.Application.Run();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Fallo al inicializar el add-on.", ex);
                throw;
            }
        }

        private static void OnAppEvent(BoAppEventTypes eventType)
        {
            if (eventType == BoAppEventTypes.aet_ShutDown)
            {
                AppLogger.Info("Cliente SAP cerrándose; finalizando add-on.");
                AppLogger.Shutdown();
                Environment.Exit(0);
            }
        }
    }
}
