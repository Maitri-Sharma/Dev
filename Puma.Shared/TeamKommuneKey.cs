using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class TeamKommuneKey : IEqualityComparer
    {
        public TeamKommuneKey(string teamId, string kommuneId)
        {
            if (teamId == null)
                throw new Exception("teamId can not be null");
            if (kommuneId == null)
                throw new Exception("kommundeId");

            this._teamId = teamId;
            this._kommuneId = kommuneId;
        }

        private string _kommuneId = null;

        public string KommuneId
        {
            get
            {
                return this._kommuneId;
            }
        }

        private string _teamId = null;

        public string TeamId
        {
            get
            {
                return this._teamId;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not TeamKommuneKey)
                return false;

            TeamKommuneKey compObj = (TeamKommuneKey)obj;
            return (compObj.KommuneId == this.KommuneId && compObj.TeamId == this.TeamId);
        }

        public new bool Equals(object x, object y)
        {
            if (x is not TeamKommuneKey)
                return false;
            if (y is not TeamKommuneKey)
                return false;
            return x.Equals(y);
        }

        public override int GetHashCode()
        {
            return (this.KommuneId + this.TeamId).GetHashCode();
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}
