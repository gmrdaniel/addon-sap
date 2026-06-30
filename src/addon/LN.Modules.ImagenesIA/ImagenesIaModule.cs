using LN.Core.Contracts;

namespace LN.Modules.ImagenesIA
{
    /// <summary>
    /// Módulo 2 — Procesamiento de Imágenes con IA.
    /// Lee una imagen o repositorio, la interpreta con el modelo de visión (vía backend),
    /// valida requisitos y crea el documento en SAP por Service Layer.
    /// (Esqueleto: el flujo completo se implementa en las Fases 3 y 4.)
    /// </summary>
    public sealed class ImagenesIaModule : IModule
    {
        public const string MenuUid = "LN_IMG";

        public string Id => "imagenes-ia";

        public string MenuCaption => "Procesamiento de Imágenes con IA";

        public void RegisterMenu(IMenuContext ctx)
        {
            ctx.AddMenuItem(MenuUid, MenuCaption);
        }

        public bool OwnsMenu(string menuUid) => menuUid == MenuUid;

        public void OnMenuClick(string menuUid)
        {
            // TODO (Fase 3): abrir el formulario de selección e interpretación de imágenes.
        }
    }
}
