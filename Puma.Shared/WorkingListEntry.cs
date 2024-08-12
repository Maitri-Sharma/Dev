using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.Shared
{
    public class WorkingListEntry
    {

        //public Indentation GetInnrykkEnum()
        //{
        //    return (Indentation)this.Innrykk;
        //}

        //public string GetWarningText()
        //{
        //    if (!this.ShouldShowWarningWhenClosing())
        //        return "";

        //    System.Text.StringBuilder result = new System.Text.StringBuilder();

        //    if (this.IsUtvalg)
        //    {
        //        result.Append(KSPUMessages.infoMsgWarningCloseUtvalg + this.Utvalg.Name + " ");
        //    }
        //    else
        //    {
        //        result.Append(KSPUMessages.infoMsgWarningCloseUtvalgene);

        //        foreach (WorkingListEntry entry in this.List)
        //        {
        //            if (entry.BelongsToListRecursive(this.UtvalgList) && entry.ShouldShowWarningWhenClosing())
        //            {
        //                result.Append(entry.Utvalg.Name + ", ");
        //            }
        //        }

        //        result.Remove(result.Length - 1, 1);
        //        result.AppendLine(KSPUMessages.infoMsgWarningCloseList + this.UtvalgList.Name + " ");
        //    }
        //    result.Append(KSPUMessages.infoMsgWarningClose);

        //    return result.ToString();
        //}

        //public bool ShouldShowWarningWhenClosing()
        //{
        //    if (this.IsUtvalg)
        //    {
        //        return !KSPU.BusinessLogic.SearchUtvalg.IsUtvalgEqualInDB(Utvalg);
        //    }
        //    else if (this.IsUtvalgsListe)
        //    {
        //        foreach (WorkingListEntry entry in this.List)
        //        {
        //            if (entry.BelongsToListRecursive(this.UtvalgList) && entry.ShouldShowWarningWhenClosing())
        //                return true;
        //        }
        //        return false;
        //    }
        //    else
        //    {
        //        throw new Exception("what type has exactly this entry?");
        //    }
        //}

        //public bool IsSavable(KSPU.WebLogic.Site site)
        //{
        //    if (this.IsUtvalg)
        //    {
        //        return UtvalgSaver.IsUtvalgSavable(this.Utvalg, site);
        //    }
        //    else if (this.IsUtvalgsListe)
        //    {
        //        return UtvalgSaver.IsUtvalgListSavable(this.UtvalgList, site);
        //    }
        //    return false;
        //}

        //public bool IsCloseableWithoutWarning(KSPU.WebLogic.Site site)
        //{
        //    if (this.IsUtvalg)
        //        return !UtvalgSaver.IsUtvalgSavableAndChanged(this.Utvalg, site);
        //    else if (this.IsUtvalgsListe)
        //        return UtvalgSaver.IsUtvalgListCloseableWithoutWarning(this.UtvalgList, site);
        //    return false;
        //}

        //private WorkingListEntryCollection _list = null;
        //public WorkingListEntryCollection List
        //{
        //    get { return _list; }
        //    set { this._list = value; }
        //}

        //public bool BelongsToListRecursive(Puma.Shared.UtvalgList utvList)
        //{
        //    if (this.IsUtvalg)
        //    {
        //        if (this.Utvalg.List == null)
        //            return false;
        //        if (this.Utvalg.List.ListId == utvList.ListId)
        //            return true;
        //        if (this.Utvalg.List.ParentList == null)
        //            return true;
        //        if (this.Utvalg.List.ParentList.ListId == utvList.ListId)
        //            return true;

        //        return false;
        //    }
        //    else if (this.IsUtvalgsListe)
        //    {
        //        if (this.UtvalgList.ParentList == null)
        //            return false;
        //        return this.UtvalgList.ParentList.ListId == utvList.ListId;
        //    }
        //    else
        //    {
        //        throw new Exception("what type has exactly this entry?");
        //    }
        //}

        public bool IsUtvalg
        {
            get { return TypeUtvalg.ToLower() == UtvalgType.Utvalg.ToString().ToLower(); }
        }

        public bool IsUtvalgsListe
        {
            get { return !this.IsUtvalg; }
        }

        private bool _CurrentlyActive = false;
        public bool CurrentlyActive
        {
            get { return _CurrentlyActive; }
            set { _CurrentlyActive = value; }
        }

        public string ExpandURL
        {
            get
            {
                if ((UtvalgList != null))
                {
                    if ((UtvalgList.BasedOn > 0))
                        return "~/images/square.gif";
                    else  //Normal list behavior
                    {
                        if (Expanded)
                            return "~/images/minus.gif";
                        else
                            return "~/images/plus.gif";
                    }
                }
                return "~/images/white.gif";
            }
        }

        private bool _Expanded = false;
        public bool Expanded
        {
            get { return _Expanded; }
            set { _Expanded = value; }
        }

        private Puma.Shared.Utvalg _Utvalg;
        public Puma.Shared.Utvalg Utvalg
        {
            get { return _Utvalg; }
            set { _Utvalg = value; }
        }

        private Puma.Shared.UtvalgList _UtvalgList;
        public Puma.Shared.UtvalgList UtvalgList
        {
            get { return _UtvalgList; }
            set { _UtvalgList = value; }
        }

        public int UtvalgListId
        {
            get;set;
        }

        public string Name
        {
            get
            {
                if ((Utvalg != null))
                    return Utvalg.Name;
                if ((UtvalgList != null))
                    return UtvalgList.Name;
                return "";
            }
        }

        private int _Innrykk = 1;
        public int Innrykk
        {
            get { return _Innrykk; }
            set { _Innrykk = value; }
        }

        //public void DecreaseInnrykkLevel()
        //{
        //    Indentation newInnrykk = Indentation.NedersteNiva;

        //    switch ((Indentation)this.Innrykk)
        //    {
        //        case Indentation.OversteNiva:
        //            newInnrykk = Indentation.MellomNiva;
        //            break;
        //        case Indentation.MellomNiva:
        //            newInnrykk = Indentation.NedersteNiva;
        //            break;
        //        case Indentation.NedersteNiva:
        //            break;
        //            //let it be
        //    }

        //    this.Innrykk = (int)newInnrykk;
        //}

        //public void IncreaseInnrykkLevel()
        //{
        //    Indentation newInnrykk = Indentation.OversteNiva;

        //    switch ((Indentation)this.Innrykk)
        //    {
        //        case Indentation.NedersteNiva:
        //            newInnrykk = Indentation.MellomNiva;
        //            break;
        //        case Indentation.MellomNiva:
        //            newInnrykk = Indentation.OversteNiva;
        //            break;
        //        case Indentation.OversteNiva:
        //            break;
        //            //let it be
        //    }

        //    this.Innrykk = (int)newInnrykk;
        //}

        //public string SwatchUtvalgSymbol
        //{
        //    get
        //    {
        //        if ((Utvalg != null))
        //        {
        //            return UtvalgOpener.GetUtvalgImageUrl(Utvalg);
        //        }
        //        else if ((UtvalgList != null))
        //        {
        //            return UtvalgOpener.GetListImageUrl(UtvalgList);
        //        }
        //        else
        //        {
        //            return "~/images/white.gif";
        //        }
        //    }
        //}

        private string _SwatchMapSymbol;
        public string SwatchMapSymbol
        {
            get
            {
                if (_SwatchMapSymbol == null)
                    return "~/images/white.gif";
                return "~/images/symboler/" + _SwatchMapSymbol;
            }
            set { _SwatchMapSymbol = value; }
        }

        public string TypeUtvalg
        {
            get
            {
                if ((Utvalg != null))
                    return UtvalgType.Utvalg.ToString();
                if ((UtvalgList != null))
                    return UtvalgType.UtvalgList.ToString();
                return "mangler verdi";
            }
        }

        public string AntallBeforeRecreation
        {
            get
            {
                if ((Utvalg != null))
                {
                    if (Utvalg.WasRecreated())
                    {
                        return Convert.ToString(Utvalg.AntallBeforeRecreation);
                    }
                    else
                    {
                        return "";
                    }
                }
                if ((UtvalgList != null))
                {
                    if (UtvalgList.ContainsRecreatedUtvalgs())
                    {
                        return Convert.ToString(UtvalgList.CalculateAntallBeforeRecreation());
                    }
                    else
                    {
                        return "";
                    }
                }
                return "mangler verdi";
            }
        }

        public string Antall
        {
            get
            {
                if ((Utvalg != null))
                {
                    return Convert.ToString(Utvalg.TotalAntall);
                }
                if ((UtvalgList != null))
                {
                    return Convert.ToString(UtvalgList.Antall);
                }
                return "mangler verdi";
            }
        }

        public string Id
        {
            get
            {
                if ((Utvalg != null))
                    return Utvalg.UtvalgId.ToString();
                if ((UtvalgList != null))
                    return UtvalgList.ListId.ToString();
                return "mangler verdi";
            }
        }

        public string SistEndret
        {
            get
            {
                if ((Utvalg != null))
                {
                    if (Utvalg.Modifications.Count > 0)
                    {
                        return Utvalg.Modifications.Max().ModificationTime.ToString("dd.MM.yy");
                    }
                    else
                    {
                        return "Ikke lagret";
                    }
                }
                if ((UtvalgList != null))
                    return "";
                return "mangler verdi";
            }
        }
    }
}

