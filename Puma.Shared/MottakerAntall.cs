using System;
using System.Collections.Generic;
using System.Text;

namespace Puma.Shared
{
    /// <summary>
    /// Denne typen holder antall mottakere og antall reserverte for hver mottakergruppe.
    /// </summary>
    public partial class MottakerAntall : MottakerAntallBase
    {
        //public MottakerAntall()
        //{}

        public MottakerAntall(PumaEnum.MottakerGruppeKode mottakerGruppe, int antall, Nullable<int> reserverte)
        {
            base.MottakerGruppe = mottakerGruppe;
            base.Antall = antall;
            this.reserverte = reserverte == null ? 0 : (int)reserverte;
        }

        private int reserverte;
        /// <summary>
        /// Antall reserverte.
        /// </summary>
        public int Reserverte
        {
            get { return reserverte; }
            set { reserverte = value; }
        }


    }
}
