using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class NewReolTree
    {
        private PumaEnum.NodeTypeNewReolTree _type = PumaEnum.NodeTypeNewReolTree.Unsettled;

        public PumaEnum.NodeTypeNewReolTree Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }

        private List<NewReolTree> _Nodes = new List<NewReolTree>();

        public List<NewReolTree> Nodes
        {
            get
            {
                return _Nodes;
            }
            set
            {
                _Nodes = value;
            }
        }

        public bool IsLeafNode
        {
            get
            {
                return LeafValue != null;
            }
        }

        private Reol _LeafValue;

        public Reol LeafValue
        {
            get
            {
                return _LeafValue;
            }
            set
            {
                _LeafValue = value;
            }
        }

        public Reol GetFirstLeafValue()
        {
            if (IsLeafNode)
                return LeafValue;
            if (Nodes.Count == 0)
                return null/* TODO Change to default(_) if this is not a reference type */;
            return Nodes[0].GetFirstLeafValue();
        }

        public void InsertByFylkeKommune(Reol r)
        {
            foreach (NewReolTree node in Nodes)
            {
                Reol leaf = node.GetFirstLeafValue();
                if (leaf != null)
                {
                    if (leaf.FylkeId == r.FylkeId)
                    {
                        node.InsertByKommune(r);
                        return;
                    }
                }
            }
            NewReolTree newNode = new NewReolTree();
            newNode.InsertByKommune(r);
            newNode._id = r.FylkeId;
            newNode._text = r.Fylke;
            Nodes.Add(newNode);
        }

        public void InsertByFylkeKommuneTeam(Reol r)
        {
            foreach (NewReolTree node in Nodes)
            {
                Reol leaf = node.GetFirstLeafValue();
                if (leaf != null)
                {
                    if (leaf.FylkeId == r.FylkeId)
                    {
                        node.InsertByKommuneTeam(r);
                        return;
                    }
                }
            }
            NewReolTree newNode = new NewReolTree();
            newNode._id = r.FylkeId;
            newNode._text = r.Fylke;
            newNode.Type = PumaEnum.NodeTypeNewReolTree.Fylke;
            newNode.InsertByKommuneTeam(r);
            Nodes.Add(newNode);
        }

        public void InsertByKommuneTeam(Reol r)
        {
            foreach (NewReolTree node in Nodes)
            {
                Reol leaf = node.GetFirstLeafValue();
                if (leaf != null)
                {
                    if (leaf.KommuneId == r.KommuneId)
                    {
                        node.InsertByTeam(r);
                        return;
                    }
                }
            }
            NewReolTree newNode = new NewReolTree();
            newNode._id = r.KommuneId;
            newNode._text = r.Kommune;
            newNode.Type = PumaEnum.NodeTypeNewReolTree.Kommune;
            newNode.InsertByTeam(r);
            Nodes.Add(newNode);
        }

        public void InsertByKommune(Reol r)
        {
            foreach (NewReolTree node in Nodes)
            {
                Reol leaf = node.GetFirstLeafValue();
                if (leaf != null)
                {
                    if (leaf.KommuneId == r.KommuneId)
                    {
                        node.Insert(r);
                        return;
                    }
                }
            }
            NewReolTree newNode = new NewReolTree();
            newNode.Insert(r);
            Nodes.Add(newNode);
        }

        public void InsertByTeam(Reol r)
        {
            foreach (NewReolTree node in Nodes)
            {
                Reol leaf = node.GetFirstLeafValue();
                if (leaf != null)
                {
                    if (leaf.TeamName == r.TeamName)
                    {
                        node.Insert(r);
                        return;
                    }
                }
            }
            NewReolTree newNode = new NewReolTree();
            newNode._id = new TeamKommuneKey(r.TeamNumber, r.KommuneId);
            newNode._text = r.TeamName;
            newNode.Type = PumaEnum.NodeTypeNewReolTree.Team;
            newNode.Insert(r);
            Nodes.Add(newNode);
        }

        public void InsertByPostalZone(Reol r)
        {
            foreach (NewReolTree node in Nodes)
            {
                Reol leaf = node.GetFirstLeafValue();
                if (leaf != null)
                {
                    if (leaf.PostalZone == r.PostalZone)
                    {
                        node.Insert(r);
                        return;
                    }
                }
            }
            NewReolTree newNode = new NewReolTree();
            newNode.Insert(r);
            Nodes.Add(newNode);
        }

        public void Insert(Reol r)
        {
            NewReolTree newNode = new NewReolTree();
            newNode.LeafValue = r;
            newNode._text = r.Name;
            newNode.Type = PumaEnum.NodeTypeNewReolTree.Budrute;
            Nodes.Add(newNode);
        }

        private AntallInformation _antallInfo = null/* TODO Change to default(_) if this is not a reference type */;

        public AntallInformation FindAntall()
        {
            if (_antallInfo != null)
                return _antallInfo;

            if (this.IsLeafNode)
                this._antallInfo = this.LeafValue.Antall;
            else
            {
                AntallInformation sum = new AntallInformation();
                foreach (NewReolTree childNode in this.Nodes)
                    sum.Accumulate(childNode.FindAntall());

                this._antallInfo = sum;
            }

            return _antallInfo;
        }

        private Dictionary<int, AntallInformation> _antallInfoByZone = new Dictionary<int, AntallInformation>();

        public AntallInformation FindAntallprZone(int zone)
        {
            if (this._antallInfoByZone.ContainsKey(zone))
                return this._antallInfoByZone[zone];

            if (this.IsLeafNode)
            {
                if (this.LeafValue.PrisSone == zone)
                    return this.LeafValue.Antall;
            }

            AntallInformation sum = new AntallInformation();
            foreach (NewReolTree childNode in this.Nodes)
                sum.Accumulate(childNode.FindAntallprZone(zone));

            return sum;
        }

        public void CacheAllAntallInformation()
        {
            if (_antallInfo == null)
                _antallInfo = this.FindAntall();

            foreach (int zone in Zones)
            {
                if (!_antallInfoByZone.ContainsKey(zone))
                    this._antallInfoByZone[zone] = this.FindAntallprZone(zone);
            }
        }

        private object _id;

        public object Id
        {
            get
            {
                return this._id;
            }
        }

        private string _text = null;

        public string Text
        {
            get
            {
                return this._text;
            }
        }

        private List<int> _zones = null;

        public List<int> Zones
        {
            get
            {
                if (_zones != null)
                    return _zones;

                _zones = new List<int>();
                _zones.Add(0);
                _zones.Add(1);
                _zones.Add(2);

                return _zones;
            }
        }

        public bool IsNodeConnectedToUtvalgDetails(ReolCollection reoler)
        {
            switch (this.Type)
            {
                case PumaEnum.NodeTypeNewReolTree.Fylke:
                    {
                        return this.IsFylkeNodeConnectedToUtvalgsDetails(reoler);
                    }

                case PumaEnum.NodeTypeNewReolTree.Kommune:
                    {
                        return this.IsKommuneNodeConnectedToUtvalgsDetails(reoler);
                    }

                case PumaEnum.NodeTypeNewReolTree.Team:
                    {
                        return this.IsTeamNodeConnectedToUtvalgsDetails(reoler);
                    }

                case PumaEnum.NodeTypeNewReolTree.Budrute:
                    {
                        return this.IsReolNodeConnectedToUtvalgsDetails(reoler);
                    }

                default:
                    {
                        throw new Exception("Node type " + this.Type.ToString() + " could not be used for function IsNodeConnectedToUtvalgDetails");
                    }
            }
        }

        private bool IsFylkeNodeConnectedToUtvalgsDetails(ReolCollection reoler)
        {
            if (this.Type != PumaEnum.NodeTypeNewReolTree.Fylke)
                throw new Exception("IsFylkeNodeConnectedToUtvalgsDetails can not be called when node type is not fylke!");

            foreach (Reol r in reoler)
            {
                if (r.FylkeId == Convert.ToString(this.Id))
                    return true;
            }

            return false;
        }

        private bool IsKommuneNodeConnectedToUtvalgsDetails(ReolCollection reoler)
        {
            if (this.Type != PumaEnum.NodeTypeNewReolTree.Kommune)
                throw new Exception("IsKommuneNodeConnectedToUtvalgsDetails can not be called when node type is not kommune!");

            foreach (Reol r in reoler)
            {
                if (r.KommuneId == Convert.ToString(this.Id))
                    return true;
            }

            return false;
        }

        private bool IsTeamNodeConnectedToUtvalgsDetails(ReolCollection reoler)
        {
            if (this.Type != PumaEnum.NodeTypeNewReolTree.Team)
                throw new Exception("IsTeamNodeConnectedToUtvalgsDetails can not be called when node type is not team!");

            foreach (Reol r in reoler)
            {
                TeamKommuneKey key = new TeamKommuneKey(r.TeamNumber, r.KommuneId);
                if ((key.Equals(this.Id)))
                    return true;
            }

            return false;
        }

        private bool IsReolNodeConnectedToUtvalgsDetails(ReolCollection reoler)
        {
            if (this.Type != PumaEnum.NodeTypeNewReolTree.Budrute)
                throw new Exception("IsReolNodeConnectedToUtvalgsDetails can not be called when node type is not reol!");

            return reoler.ContainsId(this.LeafValue.ReolId);
        }

        public int GetUtvalgDetailsAntall(Utvalg utv)
        {
            switch (this.Type)
            {
                case PumaEnum.NodeTypeNewReolTree.Fylke:
                    {
                        return this.GetUtvalgDetailsFylkeAntall(utv);
                    }

                case PumaEnum.NodeTypeNewReolTree.Kommune:
                    {
                        return this.GetUtvalgDetailsKommuneAntall(utv);
                    }

                case PumaEnum.NodeTypeNewReolTree.Team:
                    {
                        return this.GetUtvalgDetailsTeamAntall(utv);
                    }

                case PumaEnum.NodeTypeNewReolTree.Budrute:
                    {
                        return this.GetUtvalgDetailsReolAntall(utv);
                    }

                default:
                    {
                        throw new Exception("Node type " + this.Type.ToString() + " is not legal for function GetUtvalgDetailsAntall");
                    }
            }
        }

        public int GetUtvalgDetailsAntall(Utvalg utv, UtvalgReceiver receiver)
        {
            switch (this.Type)
            {
                case PumaEnum.NodeTypeNewReolTree.Fylke:
                    {
                        return this.GetUtvalgDetailsFylkeAntall(utv, receiver);
                    }

                case PumaEnum.NodeTypeNewReolTree.Kommune:
                    {
                        return this.GetUtvalgDetailsKommuneAntall(utv, receiver);
                    }

                case PumaEnum.NodeTypeNewReolTree.Team:
                    {
                        return this.GetUtvalgDetailsTeamAntall(utv, receiver);
                    }

                case PumaEnum.NodeTypeNewReolTree.Budrute:
                    {
                        return this.GetUtvalgDetailsReolAntall(receiver);
                    }

                default:
                    {
                        throw new Exception("Node type " + this.Type.ToString() + " is not legal for function GetUtvalgDetailsAntall");
                    }
            }
        }

        public int GetUtvalgDetailsAntall(Utvalg utv, int zone)
        {
            switch (this.Type)
            {
                case PumaEnum.NodeTypeNewReolTree.Fylke:
                    {
                        return this.GetUtvalgDetailsFylkeAntall(utv, zone);
                    }

                case PumaEnum.NodeTypeNewReolTree.Kommune:
                    {
                        return this.GetUtvalgDetailsKommuneAntall(utv, zone);
                    }

                case PumaEnum.NodeTypeNewReolTree.Team:
                    {
                        return this.GetUtvalgDetailsTeamAntall(utv, zone);
                    }

                case PumaEnum.NodeTypeNewReolTree.Budrute:
                    {
                        return this.GetUtvalgDetailsReolAntall(utv, zone);
                    }

                default:
                    {
                        throw new Exception("Node type " + this.Type.ToString() + " is not legal for function GetUtvalgDetailsAntall");
                    }
            }
        }

        private int GetUtvalgDetailsFylkeAntall(Utvalg utv)
        {
            int result = 0;

            foreach (Reol r in utv.Reoler)
            {
                if (r.FylkeId == Convert.ToString(this.Id))
                    result += (int)r.Antall.GetTotalAntall(utv.Receivers);
            }

            return result;
        }

        private int GetUtvalgDetailsFylkeAntall(Utvalg utv, UtvalgReceiver receiver)
        {
            int result = 0;

            UtvalgReceiverList receiverList = new UtvalgReceiverList(receiver);

            foreach (Reol r in utv.Reoler)
            {
                if (r.FylkeId == Convert.ToString(this.Id))
                    result += (int)r.Antall.GetTotalAntall(receiverList);
            }

            return result;
        }

        private int GetUtvalgDetailsFylkeAntall(Utvalg utv, int zone)
        {
            int result = 0;

            foreach (Reol r in utv.Reoler)
            {
                if (r.FylkeId == Convert.ToString(this.Id) && r.PrisSone == zone)
                    result += (int)r.Antall.GetTotalAntall(utv.Receivers);
            }

            return result;
        }

        private int GetUtvalgDetailsKommuneAntall(Utvalg utv)
        {
            int result = 0;

            foreach (Reol r in utv.Reoler)
            {
                if (r.KommuneId == Convert.ToString(this.Id))
                    result += (int)r.Antall.GetTotalAntall(utv.Receivers);
            }

            return result;
        }

        private int GetUtvalgDetailsKommuneAntall(Utvalg utv, UtvalgReceiver receiver)
        {
            int result = 0;

            UtvalgReceiverList receiverList = new UtvalgReceiverList(receiver);

            foreach (Reol r in utv.Reoler)
            {
                if (r.KommuneId == (string)this.Id)
                    result += (int)r.Antall.GetTotalAntall(receiverList);
            }

            return result;
        }

        private int GetUtvalgDetailsKommuneAntall(Utvalg utv, int zone)
        {
            int result = 0;

            foreach (Reol r in utv.Reoler)
            {
                if (r.KommuneId == (string)this.Id && r.PrisSone == zone)
                    result += (int)r.Antall.GetTotalAntall(utv.Receivers);
            }

            return result;
        }

        private int GetUtvalgDetailsTeamAntall(Utvalg utv)
        {
            int result = 0;

            foreach (Reol r in utv.Reoler)
            {
                TeamKommuneKey key = (TeamKommuneKey)this.Id;
                if (r.TeamNumber == key.TeamId && r.KommuneId == key.KommuneId)
                    result += (int)r.Antall.GetTotalAntall(utv.Receivers);
            }

            return result;
        }

        public int GetUtvalgDetailsMergedTeamAntall(Utvalg utv)
        {
            int result = 0;

            foreach (Reol r in utv.Reoler)
            {
                TeamKommuneKey key = (TeamKommuneKey)this.Id;
                if (r.TeamNumber == key.TeamId)
                    result += (int)r.Antall.GetTotalAntall(utv.Receivers);
            }

            return result;
        }

        private int GetUtvalgDetailsTeamAntall(Utvalg utv, UtvalgReceiver receiver)
        {
            int result = 0;

            UtvalgReceiverList receiverList = new UtvalgReceiverList(receiver);
            TeamKommuneKey key = (TeamKommuneKey)this.Id;

            foreach (Reol r in utv.Reoler)
            {
                if (r.TeamNumber == key.TeamId && r.KommuneId == key.KommuneId)
                    result += (int)r.Antall.GetTotalAntall(receiverList);
            }

            return result;
        }

        private int GetUtvalgDetailsTeamAntall(Utvalg utv, int zone)
        {
            int result = 0;

            TeamKommuneKey key = (TeamKommuneKey)this.Id;

            foreach (Reol r in utv.Reoler)
            {
                if (r.TeamNumber == key.TeamId && r.KommuneId == key.KommuneId && r.PrisSone == zone)
                    result += (int)r.Antall.GetTotalAntall(utv.Receivers);
            }

            return result;
        }

        private int GetUtvalgDetailsReolAntall(Utvalg utv)
        {
            return (int)this.LeafValue.Antall.GetTotalAntall(utv.Receivers);
        }

        private int GetUtvalgDetailsReolAntall(UtvalgReceiver receiver)
        {
            return (int)this.LeafValue.Antall.GetTotalAntall(new UtvalgReceiverList(receiver));
        }

        private int GetUtvalgDetailsReolAntall(Utvalg utv, int zone)
        {
            if (this.LeafValue.PrisSone == zone)
                return (int)this.LeafValue.Antall.GetTotalAntall(utv.Receivers);
            return 0;
        }

    }
}
