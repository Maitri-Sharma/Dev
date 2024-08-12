using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class DemographyOptions
    {
        //public long MaxAntall = -1;
        //public string SQLWhereClause = "";
        //public string SQLOrderby = "";
        //public string SQLWhereClauseGeography = "";
        //public System.Collections.ArrayList IndexFieldSelected = null;
        

        public long MaxAntall { get; set; } = -1;
        //private List<string> sqlwhereclause;
        public string SQLWhereClause { get; set; } = "";
        //public List<string> SQLWhereClause 
        //{
        //    get { return sqlwhereclause; }
        //    set { sqlwhereclause = value; }
        //}
        public string SQLOrderby { get; set; } = "";
        public string SQLWhereClauseGeography { get; set; } = "";
        //private List<string> sqlwhereclausegeography;
        //public List<string> SQLWhereClauseGeography
        //{
        //    get { return sqlwhereclausegeography; }
        //    set { sqlwhereclausegeography = value; }
        //}
        public System.Collections.ArrayList IndexFieldSelected { get; set; } = null;
    }
}
