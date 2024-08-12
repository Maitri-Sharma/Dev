using System;
using System.Collections.Generic;

namespace Puma.Shared
{
    public interface IUtvalg
    {
        List<Utvalg> FinnUtvalg(string kundenr, int id, PumaEnum.UtvalgsTypeKode type, bool inkluderDetaljer, out Nullable<PumaEnum.FeilKode> feilKode);
        List<Antallsopplysninger> HentAntallsopplysninger(UtvalgsId id, out Nullable<PumaEnum.FeilKode> feilKode);
        Utvalgsfordeling OppdaterUtvalg(OrdreStatusService ordreStatus);
        Utvalgsfordeling HentUtvalgsfordeling(int id, PumaEnum.UtvalgsTypeKode type);
    }
}
