using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Utvalg
{
    public class ResponseGetUtlvagCampaignsByUtvalgId
    {
        private string _Name; // Name of Utvalg or Utvalgslist

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private long _ID; // UtvalgsID or UtvalgListID

        public long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        private bool _IsDisconnected;

        public bool IsDisconnected
        {
            get { return _IsDisconnected; }
            set { _IsDisconnected = value; }
        }
        private DateTime _DistributionDate;

        public DateTime DistributionDate
        {
            get { return _DistributionDate; }
            set { _DistributionDate = value; }
        }
        private PumaEnum.OrdreStatus _OrdreStatus;

        public PumaEnum.OrdreStatus OrdreStatus
        {
            get { return _OrdreStatus; }
            set { _OrdreStatus = value; }
        }
        private PumaEnum.OrdreType _OrdreType;

        public PumaEnum.OrdreType OrdreType
        {
            get { return _OrdreType; }
            set { _OrdreType = value; }
        }
    }
}
