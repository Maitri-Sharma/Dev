using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class Team
    {
        private string _TeamNr;
        public string TeamNr
        {
            get
            {
                return _TeamNr;
            }
            set
            {
                _TeamNr = value;
            }
        }

        private string _TeamName;
        public string TeamName
        {
            get
            {
                return _TeamName;
            }
            set
            {
                _TeamName = value;
            }
        }

        public Team()
        {
        }

        public Team(string id, string name)
        {
            TeamNr = id;
            TeamName = name;
        }
    }

    public class TeamCollection : System.Collections.Generic.List<Team>
    {
    }

}
