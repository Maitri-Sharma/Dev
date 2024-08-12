using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    public class UtvalgsId
    {
        private int? utvalgid;
        private PumaEnum.UtvalgsTypeKode type;

        public UtvalgsId() { }
        public UtvalgsId(int utvalgid, PumaEnum.UtvalgsTypeKode type)
        {
            this.utvalgid = utvalgid;
            this.type = type;
        }

        /// <summary>
        /// Id til utvalg eller utvalgsliste
        /// </summary>
        public int? UtvalgId
        {
            get { return this.utvalgid; }
            set { this.utvalgid = value; }
        }

        /// <summary>
        /// Angir om id tilhører utvalg eller utvalgsliste.
        /// </summary>
        public PumaEnum.UtvalgsTypeKode Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

    }
}
