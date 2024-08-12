using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{

    [Serializable()]
    public class UtvalgCriteria
    {
        private int _CriteriaId;
        public int CriteriaId
        {
            get
            {
                return _CriteriaId;
            }
            set
            {
                _CriteriaId = value;
            }
        }

        private string _Criteria;
        public string Criteria
        {
            get
            {
                return _Criteria;
            }
            set
            {
                _Criteria = value;
            }
        }

        private PumaEnum.CriteriaType _CriteriaType;
        public PumaEnum.CriteriaType CriteriaType
        {
            get
            {
                return _CriteriaType;
            }
            set
            {
                _CriteriaType = value;
            }
        }

        public string CriteriaTypeText
        {
            get
            {
                if (_CriteriaType == PumaEnum.CriteriaType.BAAnalysis)
                    return "BA analyse";
                if (_CriteriaType == PumaEnum.CriteriaType.Demography)
                    return "Demografi";
                if (_CriteriaType == PumaEnum.CriteriaType.Drivedistance)
                    return "Kjøreavstand";
                if (_CriteriaType == PumaEnum.CriteriaType.Drivethreshold)
                    return "Kjøreanalyse";
                if (_CriteriaType == PumaEnum.CriteriaType.Drivetime)
                    return "Kjøretid";
                if (_CriteriaType == PumaEnum.CriteriaType.FromAddressPoints)
                    return "Fra adressepunkt";
                if (_CriteriaType == PumaEnum.CriteriaType.FullDistrDistrict)
                    return "Fulldistribusjon bydel";
                if (_CriteriaType == PumaEnum.CriteriaType.FullDistrKommune)
                    return "Fulldistribusjon kommune";
                if (_CriteriaType == PumaEnum.CriteriaType.FullDistrPostalZone)
                    return "Fulldistribusjon postsone";
                if (_CriteriaType == PumaEnum.CriteriaType.Fylke)
                    return "Geografi fra fylke";
                if (_CriteriaType == PumaEnum.CriteriaType.Kommune)
                    return "Geografi fra kommune";
                if (_CriteriaType == PumaEnum.CriteriaType.PostalZone)
                    return "Geografi fra postsone";
                if (_CriteriaType == PumaEnum.CriteriaType.Postboks)
                    return "Geografi fra postboks";
                if (_CriteriaType == PumaEnum.CriteriaType.Segment)
                    return "Segment";
                if (_CriteriaType == PumaEnum.CriteriaType.SelectedInMap)
                    return "Valgt enkeltvis";
                if (_CriteriaType == PumaEnum.CriteriaType.SimpleThreshold)
                    return "Fastantall rundt adressepunkt";
                if (_CriteriaType == PumaEnum.CriteriaType.Team)
                    return "Geografi fra team";
                if (_CriteriaType == PumaEnum.CriteriaType.DataConversion)
                    return "Konvertert fra UAP";
                if (_CriteriaType == PumaEnum.CriteriaType.GeografiPlukkliste)
                    return "Geografiplukkliste";
                if (_CriteriaType == PumaEnum.CriteriaType.GeografiReol)
                    return "Geografi fra budrute";
                return "";
            }
        }
    }

    [Serializable()]
    public class UtvalgCriteriaList : List<UtvalgCriteria>
    {
        public bool ContainsCriteriaType(PumaEnum.CriteriaType type)
        {
            foreach (UtvalgCriteria item in this)
            {
                if (item.CriteriaType == type)
                    return true;
            }
            return false;
        }

        public void AddCriteria(PumaEnum.CriteriaType type, string criteria)
        {
            UtvalgCriteria item = new UtvalgCriteria();
            item.CriteriaType = type;
            item.Criteria = criteria;
            this.Add(item);
        }

        // Public Sub AddCriteriaIfNotPresent(ByVal type As CriteriaType, ByVal criteria As String)
        // If Me.ContainsCriteriaType(type) Then Return
        // Dim item As New UtvalgCriteria()
        // item.CriteriaType = type
        // item.Criteria = criteria
        // Me.Add(item)
        // End Sub

        //Commented for now
        //public void AddCriteriaIfNotSameAsLastAdded(PumaEnum.CriteriaType type, string criteria)
        //{
        //    if (this.Count == 0)
        //    {
        //        UtvalgCriteria item = new UtvalgCriteria();
        //        item.CriteriaType = type;
        //        item.Criteria = criteria;
        //        this.Add(item);
        //    }
        //    else if (this.Count > 0)
        //    {
        //        if (!this.Item[this.Count - 1].CriteriaType == PumaEnum.CriteriaType.SelectedInMap)
        //        {
        //            UtvalgCriteria item = new UtvalgCriteria();
        //            item.CriteriaType = type;
        //            item.Criteria = criteria;
        //            this.Add(item);
        //        }
        //    }
        //}
    }
}
