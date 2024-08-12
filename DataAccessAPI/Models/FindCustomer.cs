using System;
using System.Collections.Generic;

namespace DataAccessAPI.Models
{
    public class FindCustomer
    {
        public HeaderFine Header { get; set; }

        public string Aktornummer { get; set; }

        public string Kundenummer { get; set; }

        public string Navn { get; set; }

        public string Organisasjonsnummer { get; set; }

        public int? Dunsnummer { get; set; }

        public int? MaksRader { get; set; }
    }

    public class HeaderFine
    {
        public string SystemCode { get; set; }
        public string MessageId { get; set; }
        public string SecurityToken { get; set; }
        public string UserName { get; set; }
        public string Version { get; set; }
        public string Timestamp { get; set; }
    }

    public class FindCustomerResponse
    {
        public List<Kundedata> Kundedata;
        public int? RaderTotalt;
        public int? Feilkode;
        public String Feilbeskrivelse;
    }

    public class Kundedata
    {
        public string Aktornummer;
        public string Kundenummer;
        public string Juridisknavn;
        public string Markedsnavn;
        public int? Dunsnummer;
    }
}
