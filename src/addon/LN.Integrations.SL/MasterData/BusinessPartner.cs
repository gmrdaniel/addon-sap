namespace LN.Integrations.SL.MasterData
{
    /// <summary>
    /// Socio de negocio del Service Layer (objeto "BusinessPartners").
    /// Se usa para validar existencia antes de crear documentos y para el alta de maestros.
    /// </summary>
    public sealed class BusinessPartner
    {
        public string CardCode { get; set; } = string.Empty;
        public string? CardName { get; set; }

        /// <summary>cCustomer, cSupplier o cLid.</summary>
        public string? CardType { get; set; }

        public string? FederalTaxID { get; set; }
    }
}
