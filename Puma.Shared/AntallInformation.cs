using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class AntallInformation
    {
        public long GetTotalAntall(UtvalgReceiverList receivers)
        {
            long result = 0;
            foreach (UtvalgReceiver r in receivers)
            {
                if ((r.ReceiverId == PumaEnum.ReceiverType.Farmers | r.ReceiverId == PumaEnum.ReceiverType.Houses))
                {
                    // These numbers should only be included if Households are not selected
                    if (!receivers.ContainsReceiver(PumaEnum.ReceiverType.Households))
                        result += this.GetAntall(r.ReceiverId);
                }
                else if ((r.ReceiverId == PumaEnum.ReceiverType.FarmersReserved | r.ReceiverId == PumaEnum.ReceiverType.HousesReserved))
                {
                    // These numbers should only be included if HouseholdsReserved are not selected
                    if (!receivers.ContainsReceiver(PumaEnum.ReceiverType.HouseholdsReserved))
                        result += this.GetAntall(r.ReceiverId);
                }
                else
                    result += this.GetAntall(r.ReceiverId);
            }
            return result;
        }

        public long GetTotalAntallReserved(UtvalgReceiverList receivers)
        {
            long result = 0;
            foreach (UtvalgReceiver r in receivers)
            {
                if ((r.ReceiverId == PumaEnum.ReceiverType.HouseholdsReserved))
                    result += this.GetAntall(r.ReceiverId);
                else if ((r.ReceiverId == PumaEnum.ReceiverType.FarmersReserved | r.ReceiverId == PumaEnum.ReceiverType.HousesReserved))
                {
                    // These numbers should only be included if HouseholdsReserved are not selected
                    if (!receivers.ContainsReceiver(PumaEnum.ReceiverType.HouseholdsReserved))
                        result += this.GetAntall(r.ReceiverId);
                }
            }
            return result;
        }

        public int GetAntall(PumaEnum.ReceiverType recType)
        {
            if (recType == PumaEnum.ReceiverType.Businesses)
                return this.Businesses;
            if (recType == PumaEnum.ReceiverType.Farmers)
                return this.Farmers;
            if (recType == PumaEnum.ReceiverType.FarmersReserved)
                return this.FarmersReserved;
            if (recType == PumaEnum.ReceiverType.Households)
                return this.Households;
            if (recType == PumaEnum.ReceiverType.HouseholdsReserved)
                return this.HouseholdsReserved;
            if (recType == PumaEnum.ReceiverType.Houses)
                return this.Houses;
            if (recType == PumaEnum.ReceiverType.HousesReserved)
                return this.HousesReserved;
            throw new Exception("Ukjent ReceiverType " + recType);
        }

        public int GetAntallReserved(PumaEnum.ReceiverType recType)
        {
            if (recType == PumaEnum.ReceiverType.Businesses)
                return 0;
            if (recType == PumaEnum.ReceiverType.Farmers)
                return this.FarmersReserved;
            if (recType == PumaEnum.ReceiverType.FarmersReserved)
                return this.FarmersReserved;
            if (recType == PumaEnum.ReceiverType.Households)
                return this.HouseholdsReserved;
            if (recType == PumaEnum.ReceiverType.HouseholdsReserved)
                return this.HouseholdsReserved;
            if (recType == PumaEnum.ReceiverType.Houses)
                return this.HousesReserved;
            if (recType == PumaEnum.ReceiverType.HousesReserved)
                return this.HousesReserved;
            throw new Exception("Ukjent ReceiverType " + recType);
        }

        public bool IsEqualTo(AntallInformation ai, PumaEnum.ReceiverType[] compareReceiverTypes)
        {
            foreach (PumaEnum.ReceiverType recType in compareReceiverTypes)
            {
                if (this.GetAntall(recType) != ai.GetAntall(recType))
                    return false;
            }
            return true;
        }

        private int _Households = 0;

        public int Households
        {
            get
            {
                return _Households;
            }
            set
            {
                _Households = value;
            }
        }

        private int _HouseholdsReserved = 0;

        public int HouseholdsReserved
        {
            get
            {
                return _HouseholdsReserved;
            }
            set
            {
                _HouseholdsReserved = value;
            }
        }

        private int _Farmers = 0;

        public int Farmers
        {
            get
            {
                return _Farmers;
            }
            set
            {
                _Farmers = value;
            }
        }

        private int _FarmersReserved = 0;

        public int FarmersReserved
        {
            get
            {
                return _FarmersReserved;
            }
            set
            {
                _FarmersReserved = value;
            }
        }

        private int _Houses = 0;

        public int Houses
        {
            get
            {
                return _Houses;
            }
            set
            {
                _Houses = value;
            }
        }

        private int _HousesReserved = 0;

        public int HousesReserved
        {
            get
            {
                return _HousesReserved;
            }
            set
            {
                _HousesReserved = value;
            }
        }

        private int _IncludeHousesReserved = 0;

        public int IncludeHousesReserved
        {
            get
            {
                return _IncludeHousesReserved;
            }
            set
            {
                _IncludeHousesReserved = value;
            }
        }

        private int _Businesses = 0;

        public int Businesses
        {
            get
            {
                return _Businesses;
            }
            set
            {
                _Businesses = value;
            }
        }

        public int TotalReserved
        {
            get
            {
                return this.HouseholdsReserved; // Includes HousesReserved and FarmersReserved
            }
        }

        public void Accumulate(AntallInformation antall)
        {
            this.Businesses += antall.Businesses;
            this.Farmers += antall.Farmers;
            this.FarmersReserved += antall.FarmersReserved;
            this.Households += antall.Households;
            this.HouseholdsReserved += antall.HouseholdsReserved;
            this.Houses += antall.Houses;
            this.HousesReserved += antall.HousesReserved;
        }
        private int _PriorityHouseholdsReserved = 0;
        public int PriorityHouseholdsReserved
        {
            get
            {
                return _PriorityHouseholdsReserved;
            }
            set
            {
                _PriorityHouseholdsReserved = value;
            }
        }
        private int _NonPriorityHouseholdsReserved = 0;
        public int NonPriorityHouseholdsReserved
        {
            get
            {
                return _NonPriorityHouseholdsReserved;
            }
            set
            {
                _NonPriorityHouseholdsReserved = value;
            }
        }
        private int _PriorityBusinessReserved = 0;
        public int PriorityBusinessReserved
        {
            get
            {
                return _PriorityBusinessReserved;
            }
            set
            {
                _PriorityBusinessReserved = value;
            }
        }
        private int _NonPriorityBusinessReserved = 0;
        public int NonPriorityBusinessReserved
        {
            get
            {
                return _NonPriorityBusinessReserved;
            }
            set
            {
                _NonPriorityBusinessReserved = value;
            }
        }
    }
}
