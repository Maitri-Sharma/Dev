using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.Models
{
    /// <summary>
    /// Data Contract Class - Utvalgsoppslag
    /// </summary>
    [DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05", Name = "FinnUtvalg")]
    public partial class FinnUtvalgData : UtvalgsId
    {
        private System.String KundenrField;

        [DataMember(IsRequired = false, Name = "Kundenr", Order = 0)]
        public System.String Kundenr
        {
            get { return KundenrField; }
            set { KundenrField = value; }
        }

        private System.Boolean InkluderDetaljerField;

        [DataMember(IsRequired = true, Name = "InkluderDetaljer", Order = 4)]
        public System.Boolean InkluderDetaljer
        {
            get { return InkluderDetaljerField; }
            set { InkluderDetaljerField = value; }
        }
    }
}
