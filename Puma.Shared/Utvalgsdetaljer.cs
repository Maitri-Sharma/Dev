using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    [Serializable()]
    public partial class Utvalgsdetaljer : UtvalgBase
    {
        public Utvalgsdetaljer()
        {
        }

        public Utvalgsdetaljer(int id, PumaEnum.UtvalgsTypeKode type, int parentId, string navn, string logo, PumaEnum.FordelingsTypeKode fordelingsType, List<Fordeling> fordelinger)
        {
            base.UtvalgId = id;
            base.Type = type;
            base.ParentId = parentId;
            base.Navn = navn;
            this.logo = logo;
            this.fordelingsType = fordelingsType;
            this.fordelinger = fordelinger;
        }

        private string logo;
        private PumaEnum.FordelingsTypeKode fordelingsType;
        private List<Fordeling> fordelinger;

        public List<Fordeling> Fordelinger
        {
            get { return fordelinger; }
            set { fordelinger = value; }
        }

        /// <summary>
        /// Angir fordelingstype - F.eks. Normal, Kommune og bydel, Informasjon(er reserverte inkludert)
        /// </summary>
        public PumaEnum.FordelingsTypeKode FordelingsType
        {
            get { return fordelingsType; }
            set { fordelingsType = value; }
        }

        public string Logo
        {
            get { return logo; }
            set { logo = value; }
        }
    }
}
