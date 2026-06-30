namespace LN.Integrations.SL.MasterData
{
    /// <summary>
    /// Artículo del Service Layer (objeto "Items").
    /// Se usa para validar existencia del artículo interpretado por la IA antes de crear documentos.
    /// </summary>
    public sealed class Item
    {
        public string ItemCode { get; set; } = string.Empty;
        public string? ItemName { get; set; }
        public string? ItemsGroupCode { get; set; }
        public string? SalesUnit { get; set; }
    }
}
