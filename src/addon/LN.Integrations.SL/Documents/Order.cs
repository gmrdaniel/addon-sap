using System.Collections.Generic;

namespace LN.Integrations.SL.Documents
{
    /// <summary>
    /// Orden de venta del Service Layer (objeto "Orders").
    /// Modelo mínimo; ampliar con los campos que use el mapeo de documentos (@LN_DOCMAP).
    /// </summary>
    public sealed class Order
    {
        public int? DocEntry { get; set; }
        public int? DocNum { get; set; }

        /// <summary>Código del socio de negocio (CardCode).</summary>
        public string CardCode { get; set; } = string.Empty;

        public string? DocDate { get; set; }
        public string? Comments { get; set; }

        public List<DocumentLine> DocumentLines { get; set; } = new List<DocumentLine>();
    }

    /// <summary>Línea de documento (artículo, cantidad, precio).</summary>
    public sealed class DocumentLine
    {
        public string ItemCode { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double? UnitPrice { get; set; }
    }
}
