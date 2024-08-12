using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.DataLayer.BusinessEntity.EC_Data
{
    public partial class UtvalgsIdEntity
    {
        public System.Int32 Id
        {
            get; set;
        }

        /// <summary>
        /// Angir om id tilhører utvalg eller utvalgsliste.
        /// </summary>
        public string SourceType { get; set; }

        public UtvalgsTypeKode Type
        {
            get;set;
        }
    }
}
