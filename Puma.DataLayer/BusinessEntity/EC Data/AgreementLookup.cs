
namespace Puma.DataLayer.BusinessEntity.EC_Data
{
    public class AgreementLookup
    {
        public Header Header { get; set; }
        public string BrukerNavn { get; set; }
        public string Key { get; set; }
        public string UtvalgsID { get; set; }
    }

    public class AvtaleOppslagResponse
    {
        public string KundeNr { get; set; }
        public decimal? statuskode { get; set; }
        public string AvtaleData { get; set; }
    }

    public class AgreementLookupResponse
    {
        public AvtaleOppslagResponse AvtaleOppslagResponse { get; set; }
    }

    public class Header
    {
        public string SystemCode { get; set; }
        public string MessageId { get; set; }
        public string SecurityToken { get; set; }
        public string UserName { get; set; }
        public string Version { get; set; }
        public string Timestamp { get; set; }
    }
}
