using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity
{
    public class Constants
    {
        public const string CurrentReolTableName = "CurrentReolTableName";
        public const string PreviousReolTableName = "PreviousReolTableName";
        public const string SystemUserName = "SystemUserName";
        public const string calcfield = "calcfield";
        public const string tmp_reol_adr = "tmp_reol_adr";
        public const string tmp_reolmap_adr = "tmp_reolmap_adr";
        public const string tmp_restpoints_gk = "tmp_restpoints_gk";
        //public const string kspu_gdbuser = "kspu_gdb/";
        public const string tmp_reol_adr_postbox = "tmp_reol_adr_postbox";
        public const string input_boksanlegg = "input_boksanlegg";
        public const string tmp_sumbefgk = "tmp_sumbefgk";
        public const string tmp_sumbefreol = "tmp_sumbefreol";
        public const string tmp_input_reolpunkter = "tmp_input_reolpunkter";
        public const string ReolIdFieldName = "reolidfieldname";
        public const string adressenr_pas = "adrnrgab";
        public const string KSPU_DBUser = "kspu_db";
        public const string input_reolpunkter = "input_reolpunkter";
        public const string tmp_input_boksanlegg = "tmp_input_boksanlegg";
        public const string tmp_gkreoloverforing = "tmp_gkreoloverforing";
        public const string idx_gkreoloverforing = "tmp_gkreoloverforing";
        public const string idx_reolgkreoloverforing = "idx_gkreol_reol";
        public const string tmp_reolid = "tmp_reolid";
        public const string tmp_reolantadr = "tmp_reolantadr";
        public const string tmp_sumhh_komm = "tmp_sumhh_komm";
        public const string input_reoler = "input_reoler";
        public const string tmp_reol_komm_faktor = "tmp_reol_komm_faktor";
        public const string KSPU_GDBUser = "kspu_gdb";
        public const string tmp_avisdekning = "tmp_avisdekning";
        public const string input_avisdekning = "input_avisdekning";
        public const string tmp_segment_1 = "tmp_segment_1";
        public const string tmp_segment_2 = "tmp_segment_2";
        public const string input_segmenter = "input_segmenter";
        public const string tmp_segment_3 = "tmp_segment_3";
        public const string tmp_reol_segment = "tmp_reol_segment";
        public const string tmp_input_reoler_segment = "tmp_input_reoler_segment";
        public const string tmp_AntallBef_ReolTableName = "tmp_AntallBef_Reol";
        public const string tmp_100mGrid_Reol = "tmp_reol_bef100";
        public const string SluttRapportTableName = "reol_rapport";
        public const string tmp_restpoints = "tmp_restpoints";
        public const string GetCapacityserviceCutoffLimitPercent = "GetCapacityserviceCutoffLimitPercent";
        public const string CapacitySperrefristVirkedag = "CapacitySperrefristVirkedag";



        //Error Messages
        public const string errMsgDeleteUtvalgWithOrdre = "Utvalget er knyttet til en ordre og kan ikke slettes.";
        public const string errMsgDeleteUtvalgWithTilbud = "Utvalget er knyttet til et tilbud og kan ikke slettes.";
        public const string errMsgDeleteUtvalgError = "Sletting av utvalget feilet.";
        public const string errMsgCampaingListExists = "Kan ikke slette basisutvalget, da det finnes kampanjer basert på det som det er gitt tilbud på.";

        public const string errMsgDeleteListWithOrdre = "Listen er knyttet til en ordre og kan ikke slettes.";
        public const string errMsgDeleteListWithTilbud = "Listen er knyttet til et tilbud og kan ikke slettes.";
        public const string errMsgDeleteListWithChildElementsOrdreTilbud = "Listen eller underliggende lister/utvalg er knyttet til en ordre/tilbud, og kan ikke slettes.";

        public const string errMsgSaveUtvalgWithOrdre = "Utvalget er knyttet til en ordre og kan ikke lagres.";

        public const string errMsgNoUtvalgChecked = "Ingen utvalg er krysset av.";

        public const string errMsgNotOnlyUtvalgChecked = "Du har krysset av en eller flere utvalgslister. Denne funksjonen kan kun kjøres på utvalg.";

        public const string errMsgUtvalgWithoutParentListChecked = "Du har krysset av et eller flere utvalg som ikke ligger i en utvalgsliste. Denne funksjonen kan kun kjøres på utvalg som ligger i en utvalgsliste.";

        public const string errMsgCantDisconnectUtvalgConnectedToBasis = "Kan ikke fjerne valgte utvalg fra liste fordi minst ett av utvalgene er basert på et basisutvalg eller en basisliste.";

        public const string errMsgCantDisconnectFromBasisList = "Kan ikke fjerne valgte utvalg fra basisliste fordi basislisten har kampanjer, og basislisten derfor må inneholde minst en liste eller ett utvalg.";

        public const string errMsgListeFrakoblingListInOrdreTilbud = "Utvalg {0} ligger i en liste tilknyttet ordre eller tilbud og kan ikke endres.";

        public const string MsgDisconnectUtvalgFromList = "Følgende utvalg ble koblet fra liste:";

        public const string MsgSaveUtvalgList = "Liste \"*\" er lagret uten listetilhørighet.";

        public const string errMsgListHasParentList = "Denne listen kan ikke kobles til liste, da dette gir en struktur med lister i tre nivåer.";

        public const string errMsgIsSameLists = "Dette er den samme listen som den aktive listen. Velg en annen liste.";

        public const string MsgConnectListInList = "Liste er koblet til liste \"*\".";

        public const string infoMsgDobbeldekning = "Det er dobbeltdekning på * budruter på denne utvalgslisten.";

        public const string errMsgListName = "Listenavnet må ha minst 3 tegn.";


        public const string errMsgListNameToLong = "Listenavnet inneholder mer enn {0} tegn. Prøv på nytt med et kortere navn.";

        public const string errMsgListExists = "Listen \"*\" eksisterer allerede. Velg et annet navn.";

        public const string MsgConnectListInNewList = "Liste \"*\" er opprettet og liste er koblet til.";      // bør få inn liste navnet og

        public const string errMsgUtvalgHasNoReceivers = "Utvalget har ingen mottakere og kan derfor ikke lagres. Kontroller at utvalget inneholder budruter og at minst en mottakergruppe er valgt.";

        public const string errMsgSaveUtvalgError = "Lagring av utvalget feilet.";

        public const string NewUtvalgName = "Påbegynt utvalg";

        public const string errMsgUtvalgName = "Utvalgsnavnet må ha minst 3 tegn.";

        public const string errMsgIllegalCharsUtv = "Utvalgsnavnet inneholder ulovlige tegn. Fjern tegnene '<' og '>' dersom eksisterer i navnet.";

        public const string errMsgUtvalgNameWithSpaces = "Utvalgsnavnet kan ikke ha mellomrom i begynnelsen eller slutten av navnet. Fjern mellomrom og prøv på nytt.";

    }


    public static class AppSetting
    {
        public const string KeyVaultName = "Key-Vault-Name";
        public const string ManegedIdentityName = "Managed-Identity-Name";
        public const string ManagedIdentityClientId = "Managed-Identity-Client-ID";
        public const string DatabaseName = "Puma_DB";
        public const string DBUserName = "Key-Vault-Secret-DBName";
        public const string DBPassword = "Key-Vault-Secret-DBPswrd";
        public const string DBServerName = "DBServer";
        public const string ECAPIURL = "EC_API_URL";
        public const string ECUserName = "EC_USERNAME";
        public const string ECPassword= "Key-Vault-Secret-ECPswrd";
        public const string IsAuthenticationApplicable = "IsAuthenticationApplicable";
        public const string ClientSecret = "Client_Secret";
        public const string ClientId = "Client_Id";
        public const string JWTSecret = "JWTSecret";
        public const string ArcGisAdminUserName = "Key-Vault-arcgisadminUserName";
        public const string ArcGisAdminPassword = "Key-Vault-arcgisadminUserPwd";
        public const string OEBSUrl = "OEBS_URL";
        public const string PumaLogCleanUpDays = "PumaLogCleanUpDays";

    }
}
