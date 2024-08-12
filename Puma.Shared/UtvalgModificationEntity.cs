using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class UtvalgModificationEntity
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

        public long UtvalgId { get; set; }

        public string info { get; set; }
    }
}
