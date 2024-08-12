using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.Models
{
    public class Helper
    {
        //DataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]

        public enum UtvalgsTypeKode
        {
            Liste,
            Utvalg
        }

        // EnumMemberAttribute attribute if the type has DataContractAttribute attribute."

        public enum Distribusjonstype
        {
            Null,
            S,
            B
        }

        [CollectionDataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]
        public class Fordelinger : List<Fordeling> { }

        [CollectionDataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]
        public class SoneAntall : List<MottakerAntallSone> { }

        [CollectionDataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]
        public class Antall : List<MottakerAntall> { }

        [CollectionDataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]
        public class UtvalgListe : List<UtvalgSvc> { }

        [CollectionDataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]
        public class Utvalgsdetaljer : List<UtvalgDetaljer> { }

        [CollectionDataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2007/05")]
        public class UtvalgantallListe : List<Utvalgantall> { }

        [CollectionDataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2009/10")]
        public class UtvalgsIdListe : List<UtvalgsId> { }

        [CollectionDataContract(Namespace = "http://Posten.KSPU.DataAccessAPI.DataContracts/2009/10")]
        public class UtvalgPRSListe : List<PRS> { }


    }
}
