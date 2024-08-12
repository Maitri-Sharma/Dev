using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Utvalg
{
    public class UtvalgCriteriaEntity
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

        public int CriteriaType
        {
            get;set;
        }

        public long UtvalgId { get; set; }

       
    }
}
