using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgList
    {
        private int _ListId;

        public int ListId
        {
            get
            {
                return _ListId;
            }
            set
            {
                _ListId = value;
            }
        }

        private bool _IsRecreated;
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

        private bool _hasMemberList;
        public bool hasMemberList
        {
            get
            {
                return _hasMemberList;
            }
            set
            {
                _hasMemberList = value;
            }
        }

        public bool HasListDemSegUtvalgDescendant()
        {
            foreach (Utvalg utv in this.MemberUtvalgs)
            {
                if (utv.IsDemOrSeg())
                    return true;
            }
            foreach (UtvalgList childList in this.MemberLists)
            {
                if (childList.HasListDemSegUtvalgDescendant())
                    return true;
            }

            return false;
        }

        public void ReplaceUtvalgIfPresentWithFreshUtvalg(Utvalg utv)
        {
            foreach (Utvalg memberUtv in this.MemberUtvalgs.ToArray())
            {
                if (memberUtv.UtvalgId == utv.UtvalgId)
                {
                    this.MemberUtvalgs.Remove(memberUtv);
                    break;
                }
            }

            this.MemberUtvalgs.Add(utv);
        }

        public Utvalg GetUtvalgDescendant(int utvalgId)
        {
            foreach (Utvalg utv in this.MemberUtvalgs)
            {
                if (utv.UtvalgId == utvalgId)
                    return utv;
            }

            foreach (UtvalgList list in this.MemberLists)
            {
                if (list.HasUtvalgAsDescendant(utvalgId))
                    return list.GetUtvalgDescendant(utvalgId);
            }

            return null/* TODO Change to default(_) if this is not a reference type */;
        }

        public UtvalgList GetUtvalgListDescendant(int utvalgListId)
        {
            foreach (UtvalgList list in this.MemberLists)
            {
                if (list.ListId == utvalgListId)
                    return list;
            }

            return null;
        }

        public bool IsUtvalgConnectedToListOrItsListReferences(int utvalgId)
        {
            if (this.ParentList != null)
                return this.ParentList.HasUtvalgAsDescendant(utvalgId);
            else
                return HasUtvalgAsDescendant(utvalgId);
        }

        public bool HasUtvalgAsDescendant(int utvalgId)
        {
            foreach (Utvalg utv in this.MemberUtvalgs)
            {
                if (utv.UtvalgId == utvalgId)
                    return true;
            }

            foreach (UtvalgList utvList in this.MemberLists)
            {
                if (utvList.HasUtvalgAsDescendant(utvalgId))
                    return true;
            }

            return false;
        }

        public bool HasUtvalgListAsDescendant(int utvalgListId)
        {
            foreach (UtvalgList list in this.MemberLists)
            {
                if (list.ListId == utvalgListId)
                    return true;
            }

            return false;
        }

        private UtvalgList _ParentList;

        public UtvalgList ParentList
        {
            get
            {
                return _ParentList;
            }
            set
            {
                if (value != null)
                {
                    if (this._ParentList != null)
                        this._ParentList.MemberLists.Remove(this);
                    if (!value.MemberLists.Contains(this))
                        value.MemberLists.Add(this);
                }
                else if (_ParentList != null)
                {
                    if (this._ParentList.MemberLists.Contains(this))
                        this._ParentList.MemberLists.Remove(this);
                }

                _ParentList = value;
            }
        }

        private List<UtvalgModification> _Modifications;
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

        private string _OrdreReferanse;
        public string OrdreReferanse
        {
            get
            {
                return _OrdreReferanse; // Referanse til en ordre eller et tilbud. Avhengig av gitt type
            }
            set
            {
                _OrdreReferanse = value;
            }
        }

        private PumaEnum.OrdreType _OrdreType;
        public PumaEnum.OrdreType OrdreType
        {
            get
            {
                return _OrdreType; // Type Ordre
            }
            set
            {
                _OrdreType = value;
            }
        }

        private PumaEnum.OrdreStatus _OrdreStatus;
        public PumaEnum.OrdreStatus OrdreStatus
        {
            get
            {
                return _OrdreStatus; // Status på en ordre eller tilbud
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

        private DateTime _InnleveringsDato;
        public DateTime InnleveringsDato
        {
            get
            {
                return _InnleveringsDato; // Dato for når en ordre skal innleveres
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

        /// <summary>
        ///     ''' NB!!: Benyttes kun av og er kun implementert for integrasjon mot Ordre
        ///     ''' Har ellers ingen verdi satt.
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private DateTime _SistOppdatert;
        public DateTime SistOppdatert
        {
            get
            {
                return _SistOppdatert; // Dato for når innholdet i lista sist ble oppdatert
            }
            set
            {
                _SistOppdatert = value;
            }
        }

        /// <summary>
        ///     ''' NB!!: Benyttes kun av og er kun implementert for integrasjon mot Ordre.
        ///     ''' Har ellers ingen verdi satt.
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        private string _SistEndretAv;
        public string SistEndretAv
        {
            get
            {
                return _SistEndretAv; // Hvem endret innholdet i lista sist
            }
            set
            {
                _SistEndretAv = value;
            }
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

        public long ParentListId { get; set; }

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

        //private string _BasedOnName;
        //public string BasedOnName
        //{
        //    get
        //    {
        //        return _BasedOnName;
        //    }
        //    set
        //    {
        //        _BasedOnName = value;
        //    }
        //}

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

        //private string _WasBasedOnName;
        //public string WasBasedOnName
        //{
        //    get
        //    {
        //        return _WasBasedOnName;
        //    }
        //    set
        //    {
        //        _WasBasedOnName = value;
        //    }
        //}

        private bool _AllowDouble;
        public bool AllowDouble
        {
            get
            {
                return _AllowDouble;
            }
            set
            {
                _AllowDouble = value;
            }
        }



        private List<CampaignDescription> _ListsBasedOnMe;
        public List<CampaignDescription> ListsBasedOnMe
        {
            get
            {
                return _ListsBasedOnMe;
            }
            set
            {
                _ListsBasedOnMe = value;
            }
        }

        private List<Utvalg> _MemberUtvalgs = new List<Utvalg>();
        public List<Utvalg> MemberUtvalgs
        {
            get
            {
                return _MemberUtvalgs;
            }
            set
            {
                _MemberUtvalgs = value;
            }
        }

        private List<UtvalgList> _MemberLists = new List<UtvalgList>();
        public List<UtvalgList> MemberLists
        {
            get
            {
                return _MemberLists;
            }
            set
            {
                _MemberLists = value;
            }
        }

        private long _Antall;
        public long Antall
        {
            get
            {
                _Antall = CalculateAntall();
                if (_Antall == 0)
                {
                    _Antall = _AntallWhenLastSaved;
                }
                return _Antall;
            }
            set
            {


                _Antall = CalculateAntall();
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

        public bool IsMemberUtvalgSkrivebeskyttet()
        {
            if (MemberUtvalgs != null)
            {
                foreach (Utvalg u in MemberUtvalgs)
                {
                    if (u.Skrivebeskyttet)
                        return true;
                }
            }
            if (MemberLists != null)
            {
                foreach (UtvalgList ul in MemberLists)
                {
                    if (ul.MemberUtvalgs != null)
                    {
                        foreach (Utvalg u in ul.MemberUtvalgs)
                        {
                            if (u.Skrivebeskyttet)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///     ''' Returnerer dato for sist lagret av ikke-systembruker. 
        ///     ''' NB: Om ikke lista har info om modifications, returneres dagens dato
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        ///     
        //commented for now
        //public DateTime LastSavedByNonSystemuser()
        //{
        //    if (this.Modifications != null && this.Modifications.Count > 0)
        //    {
        //        // result is orderer by 
        //        this.Modifications.OrderByDescending(o => o.ModificationTime);
        //        foreach (UtvalgModification m in this.Modifications)
        //        {
        //            if (!m.UserId.ToUpper.Equals(KSPU.Framework.Config.SystemUserName.ToUpper))
        //                return m.ModificationTime;
        //        }
        //    }
        //    return new DateTime();
        //}

        public bool ContainsRecreatedUtvalgs()
        {
            foreach (Utvalg u in MemberUtvalgs)
            {
                if (u.WasRecreated())
                    return true;
            }
            if (MemberLists != null)
            {
                foreach (UtvalgList l in MemberLists)
                {
                    if (l.ContainsRecreatedUtvalgs())
                        return true;
                }
            }
            return false;
        }

        private long _AntallBeforeRecreation;
        public long AntallBeforeRecreation
        {
            get
            {
                _AntallBeforeRecreation = CalculateAntallBeforeRecreation();

                return _AntallBeforeRecreation;
            }
            set
            {


                _AntallBeforeRecreation = CalculateAntallBeforeRecreation();
            }
        }

        public long CalculateAntallBeforeRecreation()
        {
            long Antall = 0;
            if (MemberLists != null)
            {
                foreach (UtvalgList l in MemberLists)
                    Antall += l.CalculateAntallBeforeRecreation();
            }
            foreach (Utvalg u in MemberUtvalgs)
            {
                if (u.WasRecreated())
                    Antall += u.AntallBeforeRecreation;
                //TODO : Need to check in future if this condition is used to calculate antal before recreatio
                //else
                //    Antall += u.TotalAntall;
            }
            return Antall;
        }

        public long CalculateAntall()
        {
            long Antall = 0;
            if (MemberLists != null)
            {
                foreach (UtvalgList l in MemberLists)
                    Antall += l.CalculateAntall();
            }
            foreach (Utvalg u in MemberUtvalgs)
                Antall += u.TotalAntall;
            return Antall;
        }

        public UtvalgCollection GetAllUtvalgs()
        {
            UtvalgCollection result = new UtvalgCollection();
            result.AddRange(this.MemberUtvalgs);
            foreach (UtvalgList list in this.MemberLists)
                result.AddRange(list.GetAllUtvalgs());
            return result;
        }

        public List<UtvalgList> GetAllLists()
        {
            List<UtvalgList> result = new List<UtvalgList>();
            result.AddRange(this.MemberLists);
            foreach (UtvalgList list in this.MemberLists)
                result.AddRange(list.GetAllLists());
            return result;
        }


        //Commented for now
        //public bool IsFullDistribution()
        //{
        //    if (MemberUtvalgs != null && MemberUtvalgs.Count > 0)
        //        return this.MemberUtvalgs.Item[0].IsFullDistribution;
        //    else
        //        throw new Exception("Utvalgslisten inneholder på tross av forretningsreglene ingen utvalg.");
        //}

        public ReolCollection GetDoubleCoverage(Utvalg utvalg)
        {
            ReolCollection reolColl = new ReolCollection();
            foreach (Utvalg u in this.GetAllUtvalgs())
            {
                if (u.UtvalgId != utvalg.UtvalgId)
                    reolColl.AddRangeUnique(utvalg.Reoler.GetDoubleReolcoverage(u.Reoler));
            }
            return reolColl;
        }

        public ReolCollection GetDoubleCoverage(List<Utvalg> utvalgs)
        {
            ReolCollection reolColl = new ReolCollection();
            foreach (Utvalg u in this.GetAllUtvalgs())
            {
                foreach (Utvalg utv in utvalgs)
                {
                    if (u.UtvalgId != utv.UtvalgId)
                        reolColl.AddRangeUnique(utv.Reoler.GetDoubleReolcoverage(u.Reoler));
                }
            }
            return reolColl;
        }

        public bool ContainUtvalgWithSkrivebeskyttelse()
        {
            foreach (Utvalg u in this.GetAllUtvalgs())
            {
                if (u.Skrivebeskyttet)
                    return true;
            }
            return false;
        }

        public bool ContainUtvalgWithReserved()
        {
            foreach (Utvalg u in this.GetAllUtvalgs())
            {
                if (u.HasReservedReceivers)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     ''' Sjekker om en liste har utvalg med dobbeldekning.
        ///     ''' nb: Kun memberutvalg, ikke memberlist sjekkes
        ///     ''' </summary>
        ///     ''' <param name="uc"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public bool ContainDobleReolcoverage(List<Utvalg> uc)
        {
            ReolCollection reolColl = new ReolCollection();
            Utvalg uBase;

            // create copylist. do not change original list
            List<Utvalg> newlist = new List<Utvalg>();
            foreach (Utvalg u in uc)
                newlist.Add(u);

            // remove first item in collection
            if (newlist.Count > 1)
            {
                uBase = newlist[0];
                newlist.RemoveAt(0);
            }
            else
                return false;

            // check if has dobbeldekning
            foreach (Utvalg u in newlist)
            {
                reolColl.AddRangeUnique(uBase.Reoler.GetDoubleReolcoverage(u.Reoler));
                if (reolColl.Count > 0)
                    return true;
            }

            if (newlist.Count > 1)
                return ContainDobleReolcoverage(newlist);

            return false;
        }



        /// <summary>
        ///     ''' Checks if any of the memberutvalgs in the list have changed
        ///     ''' or any of the memberutvalgs of the child list have changed.
        ///     ''' returns true if any of the above criterion is met, otherwise false
        ///     ''' Note: Whenever an utvalg is added or removed from a list then it is 
        ///     ''' instantly updated in the database fro mthe GUI so we dont need to check 
        ///     ''' the in-memory utvalglist for change in this case
        ///     ''' </summary>
        ///     ''' <returns>Boolean</returns>
        ///     ''' <remarks></remarks>
        public bool IsChanged()
        {
            if (this.MemberUtvalgs != null)
            {
                foreach (Utvalg memberUtvalg in this.MemberUtvalgs)
                {
                    if ((memberUtvalg.IsChanged()))
                    {
                        return true;
                    }
                }
            }

            if (this.MemberLists != null)
            {
                foreach (UtvalgList memberlist in this.MemberLists)
                {
                    if ((memberlist.IsChanged()))
                        return true;
                }
            }

            return false;
        }

        public void RemoveRuteIdsFromList(List<long> ruteIds)
        {
            foreach (long ruteId in ruteIds)
                RemoveRuteIdFromList(ruteId);
        }

        public void RemoveRuteIdFromList(long ruteId)
        {
            if (MemberUtvalgs != null)
            {
                foreach (Utvalg u in MemberUtvalgs)
                    u.Reoler.RemoveId(ruteId);
            }
            if (MemberLists != null)
            {
                foreach (UtvalgList ul in MemberLists)
                {
                    if (ul.MemberUtvalgs != null)
                    {
                        foreach (Utvalg u in ul.MemberUtvalgs)
                            u.Reoler.RemoveId(ruteId);
                    }
                }
            }
        }
    }
}
