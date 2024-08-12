using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Puma.Shared
{

    public class Utvalg : IDisposable
    {
        private Hashtable _originalData = new Hashtable(4);

        public Utvalg()
        {
            _UtvalgId = 0;
        }

        public bool HasReservedReceivers
        {
            get
            {
                foreach (UtvalgReceiver rec in this.Receivers)
                {
                    if (rec.ReceiverId == PumaEnum.ReceiverType.FarmersReserved)
                        return true;
                    if (rec.ReceiverId == PumaEnum.ReceiverType.HouseholdsReserved)
                        return true;
                    if (rec.ReceiverId == PumaEnum.ReceiverType.HousesReserved)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        ///     ''' Returnerer dato for sist lagret av ikke-systembruker. 
        ///     ''' NB:Om ikke utvalget har info om modifications, returneres dagens dato
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>

        // Commented for now
        //public string LastSavedByNonSystemuserId()
        //{
        //    if (this.Modifications != null & this.Modifications.Count > 0)
        //    {
        //        // result is orderer by 
        //        this.Modifications.OrderByDescending(o => o.ModificationTime);
        //        foreach (UtvalgModification m in this.Modifications)
        //        {
        //            if (!m.UserId.ToUpper().Equals(KSPU.Framework.Config.SystemUserName.ToUpper))
        //                return m.UserId;
        //        }
        //    }
        //    return KSPU.Framework.Config.SystemUserName;
        //}

        /// <summary>
        ///     ''' Returnerer UserName for sist lagret av ikke-systembruker. 
        ///     ''' NB:Om ikke utvalget har info om modifications, returneres systemUser
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>

        //Commented for now
        //public DateTime LastSavedByNonSystemuser()
        //{
        //    if (this.Modifications != null & this.Modifications.Count > 0)
        //    {
        //        // result is orderer by 
        //        this.Modifications.OrderByDescending(o => o.ModificationTime);
        //        foreach (UtvalgModification m in this.Modifications)
        //        {
        //            if (!m.UserId.ToUpper().Equals(KSPU.Framework.Config.SystemUserName.ToUpper))
        //                return m.ModificationTime;
        //        }
        //    }
        //    return DateTime.Today;
        //}

        /// <summary>
        ///     ''' Sammenlign 2 utvalg for å se om de er like mht innhold. 
        ///     ''' Sammenligner kun de aktuelle parametre som bruker kan endre(TotallAntall, Mottakergrupper og antall reoler)
        ///     ''' </summary>
        ///     ''' <param name="obj"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public override bool Equals(object obj)
        {
            //Utvalg utvalg = (Utvalg)obj;
            Utvalg utvalg = new Utvalg();
            // Check TotalAntall
            if (this.TotalAntall != utvalg.TotalAntall)
                return false;
            // Check Receivers
            if (this.Receivers != null)
            {
                if (utvalg.Receivers != null)
                {
                    foreach (UtvalgReceiver r in this.Receivers)
                    {
                        bool contansReciver = false;
                        foreach (UtvalgReceiver r2 in utvalg.Receivers)
                        {
                            if (r.ReceiverId == r2.ReceiverId)
                                contansReciver = true;
                        }
                        if (!contansReciver)
                            return false;
                    }
                }
                else
                    return false;
            }
            // Check Antall reoler
            if (this.Reoler != null)
            {
                if (utvalg.Reoler != null)
                {
                    if (this.Reoler.Count != utvalg.Reoler.Count)
                        return (false);
                }
                else
                    return false;
            }

            // De to linjene under burde ha vært med for lenge siden. Fryktelig skummelt
            if (utvalg.UtvalgId != this.UtvalgId)
                return false;
            if (utvalg.Name != this.Name)
                return false;

            return true;
        }

        public static Utvalg CreateNewUtvalg(string utvalgName, UtvalgReceiverList receiverList = null/* TODO Change to default(_) if this is not a reference type */)
        {
            Utvalg utv = new Utvalg();
            utv.Name = utvalgName;
            if (receiverList == null)
                utv.Receivers.IncludeReceiver(PumaEnum.ReceiverType.Households, true);
            else
                utv.Receivers = receiverList;
            return utv;
        }

        public static Utvalg CreateNewUtvalgDriver(string utvalgName, bool isVisSelected, bool isHusSelected, UtvalgReceiverList receiverList = null/* TODO Change to default(_) if this is not a reference type */)
        {
            Utvalg utv = new Utvalg();
            utv.Name = utvalgName;
            if (receiverList == null)
            {
                utv.Receivers.IncludeReceiver(PumaEnum.ReceiverType.Households, isHusSelected);
                utv.Receivers.IncludeReceiver(PumaEnum.ReceiverType.Businesses, isVisSelected);
            }
            else
                utv.Receivers = receiverList;
            return utv;
        }

        public static Utvalg CreateUtvalgCopy(Utvalg utv, bool includeParentList = true)
        {
            Utvalg newUtvalg = new Utvalg();
            if (utv.Receivers != null)
            {
                UtvalgReceiverList urLst = new UtvalgReceiverList();
                bool bMakeSureHouseholdsIncluded = false;
                bool bMakeSureHouseholdsReservedIncluded = false;
                foreach (UtvalgReceiver ur in utv.Receivers)
                {
                    if ((ur.ReceiverId == PumaEnum.ReceiverType.Houses))
                        // If (ur.ReceiverId = ReceiverType.Farmers Or ur.ReceiverId = ReceiverType.Houses) Then
                        // Farmers eksisterer i en del gamle utvalg som skulle hatt bare V - disse skal ikke få HH lagt til
                        // These groups shall not be included any more - but if ReceiverType.Households not inclueded add it.
                        bMakeSureHouseholdsIncluded = true;
                    else if ((ur.ReceiverId == PumaEnum.ReceiverType.HousesReserved))
                        // ElseIf (ur.ReceiverId = ReceiverType.FarmersReserved Or ur.ReceiverId = ReceiverType.HousesReserved) Then
                        // Farmers eksisterer i en del gamle utvalg som skulle hatt bare V - disse skal ikke få HH lagt til
                        // These groups shall not be included any more - but if ReceiverType.HouseholdsReserved not inclueded add it.
                        bMakeSureHouseholdsReservedIncluded = true;
                    else
                        urLst.Add(ur);
                }

                if (bMakeSureHouseholdsIncluded)
                {
                    if (!utv.Receivers.ContainsReceiver(PumaEnum.ReceiverType.Households))
                    {
                        UtvalgReceiver hhr = new UtvalgReceiver(PumaEnum.ReceiverType.Households);
                        urLst.Add(hhr);
                    }
                }

                if (bMakeSureHouseholdsReservedIncluded)
                {
                    if (!utv.Receivers.ContainsReceiver(PumaEnum.ReceiverType.HouseholdsReserved))
                    {
                        UtvalgReceiver hhr = new UtvalgReceiver(PumaEnum.ReceiverType.HouseholdsReserved);
                        urLst.Add(hhr);
                    }
                }


                newUtvalg = CreateNewUtvalg("New", urLst);
            }
            else
                newUtvalg = CreateNewUtvalg("New");

            // Might get new properties based on user input in LagrePanel
            newUtvalg.Name = utv.Name;
            newUtvalg.KundeNummer = utv.KundeNummer;
            newUtvalg.Logo = utv.Logo;
            newUtvalg.KundeNavn = utv.KundeNavn;

            // Properties that will be based on original utvalg
            newUtvalg.AntallWhenLastSaved = utv.AntallWhenLastSaved;
            foreach (UtvalgDistrict d in utv.Districts)
                newUtvalg.Districts.Add(d);
            foreach (UtvalgKommune k in utv.Kommuner)
                newUtvalg.Kommuner.Add(k);

            if (includeParentList)
                newUtvalg.ListId = utv.ListId; // newUtvalg.List = utv.List;

            foreach (UtvalgPostalZone pz in utv.PostalZones)
                newUtvalg.PostalZones.Add(pz);

            newUtvalg.Reoler = utv.Reoler.CreateCopy();
            if (utv.ReolerBeforeRecreation == null)
                newUtvalg.ReolerBeforeRecreation = null/* TODO Change to default(_) if this is not a reference type */;
            else
                newUtvalg.ReolerBeforeRecreation = utv.ReolerBeforeRecreation.CreateCopy();
            newUtvalg.ReolMapName = utv.ReolMapName;

            // newUtvalg.TotalAntall = utv.TotalAntall
            foreach (UtvalgCriteria c in utv.Criterias)
            {
                UtvalgCriteria uc = new UtvalgCriteria();
                uc.CriteriaType = c.CriteriaType;
                uc.Criteria = c.Criteria;
                newUtvalg.Criterias.Add(uc);
            }

            // Not included in new utvalg:
            // newutvalg.changed
            // newutvalg.UtvalgId 
            // newutvalg.Modifications
            // newutvalg.OrdreType
            // newutvalg.OrdreReferanse
            // newutvalg.OrdreStatus
            // newutvalg.InnleveringsDato
            // '''''' Change for C-04107 - not to copy weiight while coping selection no need to copy weight & DistributionDate
            // newUtvalg.Weight = utv.Weight
            // newUtvalg.DistributionType
            // newUtvalg.DistributionDate
            // newUtvalg.IsBasis
            // newUtvalg.BasedOn
            // newUtvalg.WasBasedOn
            return newUtvalg;
        }

        public bool IsFullDistribution()
        {
            return this.Kommuner.Count > 0 | this.Districts.Count > 0;
        }

        public bool AntallHasChanged(double tolerancePercent)
        {
            if (tolerancePercent < 0)
                return true;
            if (_ReolerBeforeRecreation == null)
                return false;
            long currentAnt = CalculateTotalAntall();
            AntallInformation oldAntInfo = _ReolerBeforeRecreation.FindAntall();
            long oldAnt = oldAntInfo.GetTotalAntall(this.Receivers);
            if (currentAnt < oldAnt * (100.0 - tolerancePercent) / 100.0 | currentAnt > oldAnt * (100.0 + tolerancePercent) / 100.0)
                return true;
            else
                return false;
        }

        // Private _OldUtvalgGeometry As Object = Nothing
        // ''' <summary>
        // ''' Property containing an IGeometry with the shape of the utvalg in its old reol map.
        // ''' Declared as Object to avoid reference to ArcObjects from Entities.
        // ''' </summary>
        // ''' <value></value>
        // ''' <returns></returns>
        // ''' <remarks></remarks>
        // Public Property OldUtvalgGeometry() As Object
        // Get
        // Return _OldUtvalgGeometry
        // End Get
        // Set(ByVal value As Object)
        // _OldUtvalgGeometry = value
        // End Set
        // End Property

        // Private _OldUtvalgGeometryADF As String = ""
        // ''' <summary>
        // ''' Property containing an ADF Geometry with the shape of the utvalg in its old reol map.
        // ''' Declared as Object to avoid reference to ADF from Entities.
        // ''' </summary>
        // ''' <value></value>
        // ''' <returns></returns>
        // ''' <remarks></remarks>
        // Public Property OldUtvalgGeometryADF() As String
        // Get
        // Return _OldUtvalgGeometryADF
        // End Get
        // Set(ByVal value As String)
        // _OldUtvalgGeometryADF = value
        // End Set
        // End Property

        public bool WasRecreated()
        {
            return _ReolerBeforeRecreation != null;
        }

        private bool _IsRecreated = true;
        public bool IsRecreated
        {
            get
            {
                return _IsRecreated;
            }
            set
            {
                _IsRecreated = value;
            }
        }

        private bool _OldReolMapIsMissing = false;
        public bool OldReolMapMissing
        {
            get
            {
                return _OldReolMapIsMissing;
            }
            set
            {
                _OldReolMapIsMissing = value;
            }
        }

        private ReolCollection _ReolerBeforeRecreation = null/* TODO Change to default(_) if this is not a reference type */;

        public ReolCollection ReolerBeforeRecreation
        {
            get
            {
                return _ReolerBeforeRecreation;
            }
            set
            {
                _ReolerBeforeRecreation = value;
            }
        }

        private ReolCollection _Reoler = new ReolCollection();

        public ReolCollection Reoler
        {
            get
            {
                return _Reoler;
            }
            set
            {
                _Reoler = value;
            }
        }

        private int _UtvalgId;

        public int UtvalgId
        {
            get
            {
                return _UtvalgId;
            }
            set
            {
                _UtvalgId = value;
            }
        }


        private bool _Changed = false;
        public bool Changed
        {
            get
            {
                return _Changed;
            }
            set
            {
                _Changed = value;
            }
        }


        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public string ListName
        {
            get;set;
        }

        private string _KundeNavn;

        public string KundeNavn
        {
            get
            {
                return _KundeNavn;
            }
            set
            {
                _KundeNavn = value;
            }
        }

        private string _Logo;

        public string Logo
        {
            get
            {
                return _Logo;
            }
            set
            {
                _Logo = value;
            }
        }

        private string _ReolMapName;

        public string ReolMapName
        {
            get
            {
                return _ReolMapName;
            }
            set
            {
                _ReolMapName = value;
            }
        }

        private string _OldReolMapName;

        public string OldReolMapName
        {
            get
            {
                return _OldReolMapName;
            }
            set
            {
                _OldReolMapName = value;
            }
        }
        private bool _Skrivebeskyttet;
        public bool Skrivebeskyttet
        {
            get
            {
                return _Skrivebeskyttet;
            }
            set
            {
                _Skrivebeskyttet = value;
            }
        }

        private int _Weight;
        public int Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                _Weight = value;
            }
        }

        private PumaEnum.DistributionType _DistributionType;
        public PumaEnum.DistributionType DistributionType
        {
            get
            {
                return _DistributionType;
            }
            set
            {
                _DistributionType = value;
            }
        }

        private DateTime _DistributionDate;
        public DateTime DistributionDate
        {
            get
            {
                return _DistributionDate;
            }
            set
            {
                _DistributionDate = value;
            }
        }

        private double _ArealAvvik;
        public double ArealAvvik
        {
            get
            {
                return _ArealAvvik;
            }
            set
            {
                _ArealAvvik = value;
            }
        }

        private bool _IsBasis;
        public bool IsBasis
        {
            get
            {
                return _IsBasis;
            }
            set
            {
                _IsBasis = value;
            }
        }

        private int _BasedOn;
        public int BasedOn
        {
            get
            {
                return _BasedOn;
            }
            set
            {
                _BasedOn = value;
            }
        }

        private string _BasedOnName;
        public string BasedOnName
        {
            get
            {
                return _BasedOnName;
            }
            set
            {
                _BasedOnName = value;
            }
        }

        private int _WasBasedOn;
        public int WasBasedOn
        {
            get
            {
                return _WasBasedOn;
            }
            set
            {
                _WasBasedOn = value;
            }
        }

        private string _WasBasedOnName;
        public string WasBasedOnName
        {
            get
            {
                return _WasBasedOnName;
            }
            set
            {
                _WasBasedOnName = value;
            }
        }

        //private UtvalgList _List;

        public UtvalgList List
        {
            get; set;
        }
        //public int utvalglistid { get; set; }
        public string ListId { get; set; }
        public long AntallBeforeRecreation
        {
            get
            {
                if (ReolerBeforeRecreation == null)
                    return 0;
                AntallInformation totalAntall = ReolerBeforeRecreation.FindAntall();
                return totalAntall.GetTotalAntall(this.Receivers);
            }
        }

        private int _TotalAntall;

        public int TotalAntall
        {
            get
            {
                if (this.Reoler.Count != 0)
                {
                    return (int)CalculateTotalAntall();
                }
                else
                {
                    return _TotalAntall;

                }
            }
            set { _TotalAntall = value; }
        }

        /// <summary>
        ///     ''' Referanse til en ordre eller et tilbud. Avhengig av gitt type
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private string _OrdreReferanse;
        public string OrdreReferanse
        {
            get
            {
                return _OrdreReferanse;
            }
            set
            {
                _OrdreReferanse = value;
            }
        }

        /// <summary>
        ///     ''' Type Ordre. Tilbud eller Ordre
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private PumaEnum.OrdreType _OrdreType;
        public PumaEnum.OrdreType OrdreType
        {
            get
            {
                return _OrdreType;
            }
            set
            {
                _OrdreType = value;
            }
        }

        /// <summary>
        ///     ''' Status på en ordre eller tilbud
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private PumaEnum.OrdreStatus _OrdreStatus;
        public PumaEnum.OrdreStatus OrdreStatus
        {
            get
            {
                return _OrdreStatus;
            }
            set
            {
                _OrdreStatus = value;
            }
        }

        private string _KundeNummer;
        public string KundeNummer
        {
            get
            {
                return _KundeNummer;
            }
            set
            {
                _KundeNummer = value;
            }
        }


        /// <summary>
        ///     ''' Dato for når en ordre skal innleveres
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private DateTime _InnleveringsDato;
        public DateTime InnleveringsDato
        {
            get
            {
                return _InnleveringsDato;
            }
            set
            {
                _InnleveringsDato = value;
            }
        }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        /// <value>
        /// The modification date.
        /// </value>
        public DateTime ModificationDate
        {
            get; set;
        }

        /// <summary>
        ///     ''' Evt avtalenummer tilknyttet utvalg. Settes og benyttes av Ordre
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private int _Avtalenummer;
        public Nullable<int> Avtalenummer
        {
            get
            {
                return _Avtalenummer;
            }
            set
            {
                _Avtalenummer = (int)value;
            }
        }
        private double _thickness;
        public double Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                _thickness = value;
            }
        }

        /// <summary>
        ///     ''' Get number of HH receivers for this Utvalg. Only counts if HH are selected for this Utvalg.
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public long GetAntallHH()
        {
            long result = 0;
            AntallInformation totalAntall = this.Reoler.FindAntall();
            foreach (UtvalgReceiver rt in this.Receivers)
            {
                if (rt.ReceiverId == PumaEnum.ReceiverType.Households)
                    result += totalAntall.GetAntall(rt.ReceiverId);
            }
            return result;
        }

        /// <summary>
        ///     ''' Get number of V receivers for this Utvalg. Only counts if V are selected for this Utvalg.
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public long GetAntallV()
        {
            long result = 0;
            AntallInformation totalAntall = this.Reoler.FindAntall();
            foreach (UtvalgReceiver rt in this.Receivers)
            {
                if (rt.ReceiverId == PumaEnum.ReceiverType.Businesses)
                    result += totalAntall.GetAntall(rt.ReceiverId);
            }
            return result;
        }

        /// <summary>
        ///     ''' Get number of Reserved receivers for this Utvalg. Only counts if Res are selected for this Utvalg.
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public long GetAntallRes()
        {
            long result = 0;
            AntallInformation totalAntall = this.Reoler.FindAntall();
            foreach (UtvalgReceiver rt in this.Receivers)
            {
                if (rt.ReceiverId == PumaEnum.ReceiverType.HouseholdsReserved)
                    result += totalAntall.GetAntall(rt.ReceiverId);
            }
            return result;
        }

        /// <summary>
        ///     ''' Calculates total number of receivers for this Utvalg. Only counts the types of receivers that are selected for this Utvalg.
        ///     ''' Farmers (Gårdbrukere) and Houses (Eneboliger og rekkehus) are included in Households (husholdninger)
        ///     ''' and are not counted if Households are selected, so that these groups are not counted twice.
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public long CalculateTotalAntall()
        {
            AntallInformation totalAntall = this.Reoler.FindAntall();
            return totalAntall.GetTotalAntall(this.Receivers);
        }

        /// <summary>
        ///     ''' Calculates total number of receivers for this Utvalg. Counts all types of receivers; also receivers that are not selected for this Utvalg.
        ///     ''' Farmers (Gårdbrukere) and Houses (Eneboliger og rekkehus) are included in Households (husholdninger) and are not counted twice.
        ///     ''' This means that this method returns Businesses (Virksomheter) + Households (husholdninger) (+ Reserved households if includeReserved is true).
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>private UtvalgList _List;
        public long CalculateTotalAntallAllReceivers(bool includeReserved)
        {
            AntallInformation totalAntall = this.Reoler.FindAntall();
            long result = 0;
            foreach (PumaEnum.ReceiverType rt in new PumaEnum.ReceiverType[] { PumaEnum.ReceiverType.Businesses, PumaEnum.ReceiverType.Households })
                result += totalAntall.GetAntall(rt);
            if (includeReserved)
                result += totalAntall.GetAntall(PumaEnum.ReceiverType.HouseholdsReserved);
            return result;
        }

        private long _AntallWhenLastSaved = 0;
        public long AntallWhenLastSaved
        {
            get
            {
                return _AntallWhenLastSaved;
            }
            set
            {
                _AntallWhenLastSaved = value;
            }
        }

        private List<UtvalgModification> _Modifications = new List<UtvalgModification>();
        public List<UtvalgModification> Modifications
        {
            get
            {
                return _Modifications;
            }
            set
            {
                _Modifications = value;
            }
        }

        private List<UtvalgKommune> _Kommuner = new List<UtvalgKommune>();
        public List<UtvalgKommune> Kommuner
        {
            get
            {
                return _Kommuner;
            }
            set
            {
                _Kommuner = value;
            }
        }

        private UtvalgReceiverList _Receivers = new UtvalgReceiverList();
        public UtvalgReceiverList Receivers
        {
            get
            {
                return _Receivers;
            }
            set
            {
                _Receivers = value;
            }
        }

        private List<UtvalgDistrict> _Districts = new List<UtvalgDistrict>();
        public List<UtvalgDistrict> Districts
        {
            get
            {
                return _Districts;
            }
            set
            {
                _Districts = value;
            }
        }

        private List<UtvalgPostalZone> _PostalZones = new List<UtvalgPostalZone>();
        public List<UtvalgPostalZone> PostalZones
        {
            get
            {
                return _PostalZones;
            }
            set
            {
                _PostalZones = value;
            }
        }

        private UtvalgCriteriaList _Criterias = new UtvalgCriteriaList();
        public UtvalgCriteriaList Criterias
        {
            get
            {
                return _Criterias;
            }
            set
            {
                _Criterias = value;
            }
        }

        private List<CampaignDescription> _UtvalgsBasedOnMe;
        public List<CampaignDescription> UtvalgsBasedOnMe
        {
            get
            {
                return _UtvalgsBasedOnMe;
            }
            set
            {
                _UtvalgsBasedOnMe = value;
            }
        }

        public List<StructuralChange> GetStructuralChanges()
        {
            List<StructuralChange> result = new List<StructuralChange>();
            if (_ReolerBeforeRecreation == null)
                return result;
            foreach (Reol oldReol in _ReolerBeforeRecreation)
            {
                Reol newReol = Reoler.GetReol(oldReol.ReolId);
                result.Add(new StructuralChange(oldReol, newReol));
            }
            foreach (Reol newReol in Reoler)
            {
                if (_ReolerBeforeRecreation.GetReol(newReol.ReolId) == null)
                    // New reol without corresponding old reol
                    result.Add(new StructuralChange(null/* TODO Change to default(_) if this is not a reference type */, newReol));
            }

            result.Sort();

            return result;
        }

        /// <summary>
        ///     ''' Returns a different sorted list of structural changes from POB5095. Only used by Internweb.
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public List<StructuralChange> GetStructuralChangesSortSpecial()
        {
            List<StructuralChange> changedresult = new List<StructuralChange>();
            changedresult = this.GetStructuralChanges();
            changedresult.Sort((x, y) => x.CompareDeviation(y));

            return changedresult;
        }


        /// <summary>
        ///     ''' Returns a description containing type of receiver and number og receivers as a formatted string. Used by Internweb.
        ///     ''' </summary>
        ///     ''' <param name="showReserved"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public string GetReceiversDescription(bool showReserved)
        {
            System.Text.StringBuilder grupper = new System.Text.StringBuilder();
            AntallInformation ant = Reoler.FindAntall();

            string HH = "Hush.: ";
            string V = "Virk.: ";
            string G = "Gård.: ";
            string Eneb = "Eneb.: ";
            string HH_Res = "Res.hush.: ";
            string Eneb_Res = "Res.eneb.: ";
            string G_Res = "Res.gård.: ";
            string ending = "<br/>";

            if (showReserved)
            {
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Households))
                {
                    if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Households))
                        grupper.Append(HH + FormatUtility.IntegerToString(ant.Households) + ending);
                    if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Businesses))
                        grupper.Append(V + FormatUtility.IntegerToString(ant.Businesses) + ending);
                    if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.HouseholdsReserved))
                        grupper.Append(HH_Res + FormatUtility.IntegerToString(ant.HouseholdsReserved) + ending);
                }
                else
                {
                    if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Houses))
                        grupper.Append(Eneb + FormatUtility.IntegerToString(ant.Houses) + ending);
                    if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Farmers))
                        grupper.Append(G + FormatUtility.IntegerToString(ant.Farmers) + ending);
                    if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Businesses))
                        grupper.Append(V + FormatUtility.IntegerToString(ant.Businesses) + ending);
                    if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.HousesReserved))
                        grupper.Append(Eneb_Res + FormatUtility.IntegerToString(ant.HousesReserved) + ending);
                    if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.FarmersReserved))
                        grupper.Append(G_Res + FormatUtility.IntegerToString(ant.FarmersReserved) + ending);
                }
            }
            else if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Households))
            {
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Households) && Receivers.ContainsReceiver(PumaEnum.ReceiverType.HouseholdsReserved))
                    grupper.Append(HH + FormatUtility.IntegerToString(ant.Households + ant.HouseholdsReserved) + ending);
                else if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Households))
                    grupper.Append(HH + FormatUtility.IntegerToString(ant.Households) + ending);
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Businesses))
                    grupper.Append(V + FormatUtility.IntegerToString(ant.Businesses) + ending);
            }
            else
            {
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Houses) && Receivers.ContainsReceiver(PumaEnum.ReceiverType.HousesReserved))
                    grupper.Append(Eneb + FormatUtility.IntegerToString(ant.Houses + ant.HousesReserved) + ending);
                else if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Houses))
                    grupper.Append(Eneb + FormatUtility.IntegerToString(ant.Houses) + ending);
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Farmers) && Receivers.ContainsReceiver(PumaEnum.ReceiverType.FarmersReserved))
                    grupper.Append(G + FormatUtility.IntegerToString(ant.Farmers + ant.FarmersReserved) + ending);
                else if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Farmers))
                    grupper.Append(G + FormatUtility.IntegerToString(ant.Farmers) + ending);
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Businesses))
                    grupper.Append(V + FormatUtility.IntegerToString(ant.Businesses) + ending);
            }

            if (grupper.Length == 0)
                grupper.Append("Ingen");
            return grupper.ToString();
        }


        /// <summary>
        ///     ''' Returns a HTML table containing description of receiver and number og receivers as a formatted string for Kundeweb.
        ///     ''' vaibhav added paramter for houshold
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public string GetReceiversDescriptionKW(bool incHH
    )
        {
            System.Text.StringBuilder resultHTML = new System.Text.StringBuilder();
            AntallInformation ant = Reoler.FindAntall();
            AntallInformation antOld = null/* TODO Change to default(_) if this is not a reference type */;
            bool addBusiness = false;
            bool addHouse = false;
            resultHTML.Append("<table style='width:100%;'>");
            // values
            long oldHH = 0;
            long oldHHRes = 0;

            long oldV = 0;
            long oldEneb = 0;
            // text
            string HH = "Husholdninger";
            string V = "Virksomheter";
            string Eneb = "Eneboliger/Rekkehus";
            // style
            string cellWidthNum = " style='width:75px;padding: 0px;text-align:right;'";
            string cellWidthName = " style='width:294px;padding: 0px;'";
            string cellWidthNumTotal = " style='width:75px;border-top: solid 1px black;font-weight:bold;padding: 0px;text-align:right;'";
            string cellWidthNameTotal = " style='width:294px;border-top: solid 1px black;font-weight:bold;padding: 0px;'";
            string cssWasCreated = " class='yellow'";

            if (this.WasRecreated())
            {
                antOld = this.ReolerBeforeRecreation.FindAntall();
                oldHH = antOld.Households;
                oldHHRes = antOld.HouseholdsReserved;
                oldV = antOld.Businesses;
                oldEneb = antOld.Houses;

                cellWidthName = " style='width:219px;padding: 0px;'";
                cellWidthNameTotal = " style='width:219px;border-top: solid 1px black;font-weight:bold;padding: 0px;'";
                resultHTML.Append("<tr><th" + cellWidthName + ">&nbsp;</th><th" + cellWidthNum + cssWasCreated + ">Før</th><th " + cellWidthNum + ">Nå</th></tr>");
            }

            // receivers
            if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Households))
            {
                if (incHH)
                    resultHTML.Append(this.CreateReceiverRow(HH, cellWidthName, cellWidthNum, cssWasCreated, FormatUtility.IntegerToString(ant.Households + ant.HouseholdsReserved), FormatUtility.IntegerToString((int)(oldHH + oldHHRes))));
                else
                    resultHTML.Append(this.CreateReceiverRow(HH, cellWidthName, cellWidthNum, cssWasCreated, FormatUtility.IntegerToString(ant.Households), FormatUtility.IntegerToString((int)oldHH)));
                addHouse = true;
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Businesses))
                    addBusiness = true;
            }
            else
            {
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Houses))
                {
                    resultHTML.Append(this.CreateReceiverRow(Eneb, cellWidthName, cellWidthNum, cssWasCreated, FormatUtility.IntegerToString(ant.Houses), FormatUtility.IntegerToString((int)oldEneb)));
                    addHouse = true;
                }
                if (Receivers.ContainsReceiver(PumaEnum.ReceiverType.Businesses))
                    addBusiness = true;
            }
            if (addBusiness)
                resultHTML.Append(this.CreateReceiverRow(V, cellWidthName, cellWidthNum, cssWasCreated, FormatUtility.IntegerToString(ant.Businesses), FormatUtility.IntegerToString((int)oldV)));

            // totalsum
            resultHTML.Append("<tr style='font-weight:bold;'>");
            resultHTML.Append("<td " + cellWidthNameTotal + ">&nbsp;</td>");
            if (this.WasRecreated())
                resultHTML.Append("<td" + cellWidthNumTotal + cssWasCreated + " >" + FormatUtility.LongToString(antOld.GetTotalAntall(this.Receivers)) + "</td>");
            resultHTML.Append("<td" + cellWidthNumTotal + ">" + FormatUtility.LongToString(this.CalculateTotalAntall()) + "</td>");
            resultHTML.Append("</tr>");

            resultHTML.Append("</table>");

            if (!(addHouse | addBusiness))
                return "Ingen";
            return resultHTML.ToString();
        }
        /// <summary>
        ///     ''' Helperfunction for GetReceiversDescriptionKW
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private string CreateReceiverRow(string receiverText, string cellWidthName, string cellWidthNum, string cssWasCreated, string count, string countold)
        {
            System.Text.StringBuilder resultHTML = new System.Text.StringBuilder();

            resultHTML.Append("<tr>");
            resultHTML.Append("<td " + cellWidthName + ">" + receiverText + "</td>");
            if (this.WasRecreated())
                resultHTML.Append("<td" + cellWidthNum + cssWasCreated + " >" + countold + "</td>");
            resultHTML.Append("<td" + cellWidthNum + ">" + count + "</td>");
            resultHTML.Append("</tr>");

            return resultHTML.ToString();
        }

        /// <summary>
        ///     ''' Returnerer trur dersom utvalget har et kriterie Demografi eller Segment
        ///     ''' </summary>
        ///     ''' <returns>True dersom ett eller begge kriteriene Demografi eller Segmenet er angitt for utvalget</returns>
        ///     ''' <remarks></remarks>
        public bool IsDemOrSeg()
        {
            bool demOrSeg = false;
            if (this.Criterias != null)
            {
                foreach (UtvalgCriteria Item in this.Criterias)
                {
                    if (Item.CriteriaType == PumaEnum.CriteriaType.Demography || Item.CriteriaType == PumaEnum.CriteriaType.Segment)
                    {
                        demOrSeg = true;
                        break;
                    }
                }
            }
            return demOrSeg;
        }

        // Public Shared Sub CleanUp(ByRef comObject As Object)


        // ' Dim i As Integer = 0
        // ' While i = Marshal.ReleaseComObject(comObject) > 0
        // ' End While
        // If Not comObject Is Nothing Then
        // If Not Marshal.IsComObject(comObject) Then Return
        // Marshal.FinalReleaseComObject(comObject)
        // End If
        // comObject = Nothing
        // GC.Collect()
        // End Sub

        private bool disposedValue = false;        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                }
            }
            this.disposedValue = true;
        }

        public void SetInitialData()
        {
            this._originalData.Add("NoOfReceivers", this.AntallBeforeRecreation);
            this._originalData.Add("NoOfRoutes", this.Reoler.Count);
            this._originalData.Add("ReceiverGroups", this.Receivers.Count);
            this._originalData.Add("WriteProtected", this.Skrivebeskyttet);
        }

        public bool IsChanged()
        {
            bool isUtvalgChanged = false;

            if (this._originalData != null)
            {
                if (this._originalData.ContainsKey("WriteProtected") && this.Skrivebeskyttet != System.Convert.ToBoolean(this._originalData.ContainsKey("WriteProtected")))
                    isUtvalgChanged = true;
                else if (this._originalData.ContainsKey("NoOfRoutes") && this.Reoler.Count != System.Convert.ToInt64(this._originalData.ContainsKey("NoOfRoutes")))
                    isUtvalgChanged = true;
                else if (this._originalData.ContainsKey("ReceiverGroups") && this.Receivers.Count != System.Convert.ToInt64(this._originalData.ContainsKey("ReceiverGroups")))
                    isUtvalgChanged = true;
                else if (this._originalData.ContainsKey("NoOfReceivers") && this.TotalAntall != System.Convert.ToInt64(this._originalData.ContainsKey("NoOfReceivers")))
                    isUtvalgChanged = true;
            }

            return isUtvalgChanged;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>

    /// ''' 

    /// ''' </summary>

    /// ''' <remarks></remarks>
    public class UtvalgModification : IComparable
    {
        private int _ModificationId;

        public int ModificationId
        {
            get
            {
                return _ModificationId;
            }
            set
            {
                _ModificationId = value;
            }
        }

        private string _UserId;

        public string UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                _UserId = value;
            }
        }

        private DateTime _ModificationTime;

        public DateTime ModificationTime
        {
            get
            {
                return _ModificationTime;
            }
            set
            {
                _ModificationTime = value;
            }
        }

        public int ListId { get; set; }
        public int CompareTo(object obj)
        {
            UtvalgModification other = (UtvalgModification)obj;
            return this.ModificationTime.CompareTo(other.ModificationTime);
        }
    }

    public class StructuralChange : IComparable
    {
        private string _Fylke;
        public string Fylke
        {
            get
            {
                return _Fylke;
            }
            set
            {
                _Fylke = value;
            }
        }

        private string _Kommune;
        public string Kommune
        {
            get
            {
                return _Kommune;
            }
            set
            {
                _Kommune = value;
            }
        }

        private string _Teamname;
        public string Teamname
        {
            get
            {
                return _Teamname;
            }
            set
            {
                _Teamname = value;
            }
        }

        private string _Reolname;
        public string Reolname
        {
            get
            {
                return _Reolname;
            }
            set
            {
                _Reolname = value;
            }
        }
        private string _OldHH = "";
        public string OldHH
        {
            get
            {
                return _OldHH;
            }
            set
            {
                _OldHH = value;
            }
        }
        private string _NewHH = "";
        public string NewHH
        {
            get
            {
                return _NewHH;
            }
            set
            {
                _NewHH = value;
            }
        }
        private string _OldV = "";
        public string OldV
        {
            get
            {
                return _OldV;
            }
            set
            {
                _OldV = value;
            }
        }
        private string _NewV = "";
        public string NewV
        {
            get
            {
                return _NewV;
            }
            set
            {
                _NewV = value;
            }
        }

        private string _TeamNumber = "";
        public string TeamNumber
        {
            get
            {
                return _TeamNumber;
            }
            set
            {
                _TeamNumber = value;
            }
        }

        private string _ReolNumber = "";
        public string ReolNumber
        {
            get
            {
                return _ReolNumber;
            }
            set
            {
                _ReolNumber = value;
            }
        }

        public StructuralChange(Reol oldReol, Reol newReol)
        {
            if (oldReol != null)
            {
                _OldHH = oldReol.Antall.Households.ToString();
                _OldV = oldReol.Antall.Businesses.ToString();
                _Reolname = oldReol.DescriptiveName;
                _Teamname = oldReol.TeamName;
                _Kommune = oldReol.Kommune;
                _Fylke = oldReol.Fylke;
                _TeamNumber = oldReol.TeamNumber;
                _ReolNumber = oldReol.ReolNumber;
            }
            if (newReol != null)
            {
                _NewHH = newReol.Antall.Households.ToString();
                _NewV = newReol.Antall.Businesses.ToString();
                _Reolname = newReol.DescriptiveName;
                _Teamname = newReol.TeamName;
                _Kommune = newReol.Kommune;
                _Fylke = newReol.Fylke;
                _TeamNumber = newReol.TeamNumber;
                _ReolNumber = newReol.ReolNumber;
            }
        }

        public int CompareTo(object obj)
        {
            StructuralChange other = (StructuralChange)obj;

            int teamNoCompare = string.Compare(this.TeamNumber, other.TeamNumber, true);
            int reolNoCompare = string.Compare(this.ReolNumber, other.ReolNumber, true);

            if (teamNoCompare != 0)
                return teamNoCompare;
            else if (reolNoCompare == 1)
                return reolNoCompare;
            else
                return string.Compare(this.Reolname, other.Reolname, true) * -1;
        }

        private int getOldHH()
        {
            if (OldHH.Length < 1)
                return 0;

            return System.Convert.ToInt32(OldHH);
        }

        private int getOldV()
        {
            if (OldV.Length < 1)
                return 0;

            return System.Convert.ToInt32(OldV);
        }

        private int getNewHH()
        {
            if (NewHH.Length < 1)
                return 0;

            return System.Convert.ToInt32(NewHH);
        }

        private int getNewV()
        {
            if (NewV.Length < 1)
                return 0;

            return System.Convert.ToInt32(NewV);
        }

        private int getDeviation()
        {

            return Math.Abs(System.Convert.ToInt32(NewHH) - System.Convert.ToInt32(OldHH)) + Math.Abs(System.Convert.ToInt32(NewV) - System.Convert.ToInt32(OldV));

        }

        private bool isNew()
        {
            return ((OldHH.Length < 1) & (OldV.Length < 1));
        }

        private bool isDeleted()
        {
            return ((NewHH.Length < 1) & (NewV.Length < 1));
        }

        public int CompareDeviation(StructuralChange p2)
        {
            if (isNew())
            {
                if (p2.isNew())
                    return 0;
                else
                    return -1;
            }
            else if (p2.isNew())
                return 1;


            if (isDeleted())
            {
                if (p2.isDeleted())
                    return 0;
                else
                    return -1;
            }
            else if (p2.isDeleted())
                return 1;


            if (getDeviation() > p2.getDeviation())
                return -1;

            return 1;
        }
    }

    [Serializable()]
    internal class FormatUtility
    {
        public static string IntegerToString(int value)
        {
            return value.ToString("N", FormatUtility.IntNumberFormatInfo);
        }

        public static string LongToString(long value)
        {
            return value.ToString("N", FormatUtility.IntNumberFormatInfo);
        }

        private static NumberFormatInfo _intNumberFormatInfo = null;

        private static NumberFormatInfo IntNumberFormatInfo
        {
            get
            {
                if (_intNumberFormatInfo == null)
                {
                    FormatUtility._intNumberFormatInfo = new CultureInfo("nb-NO").NumberFormat;
                    FormatUtility._intNumberFormatInfo.NumberGroupSeparator = " ";
                    int[] groupSizeArray = new[] { 3 };
                    FormatUtility._intNumberFormatInfo.NumberGroupSizes = groupSizeArray;
                    FormatUtility._intNumberFormatInfo.NumberDecimalDigits = 0;
                }

                return FormatUtility._intNumberFormatInfo;
            }
        }
    }

}
