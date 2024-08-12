using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class ReolTree
    {
        private List<ReolTree> _Nodes = new List<ReolTree>();

        public List<ReolTree> Nodes
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
            foreach (ReolTree node in Nodes)
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
            ReolTree newNode = new ReolTree();
            newNode.InsertByKommune(r);
            Nodes.Add(newNode);
        }

        public void InsertByFylkeKommuneTeam(Reol r)
        {
            foreach (ReolTree node in Nodes)
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
            ReolTree newNode = new ReolTree();
            newNode.InsertByKommuneTeam(r);
            Nodes.Add(newNode);
        }

        public void InsertByKommuneTeam(Reol r)
        {
            foreach (ReolTree node in Nodes)
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
            ReolTree newNode = new ReolTree();
            newNode.InsertByTeam(r);
            Nodes.Add(newNode);
        }

        public void InsertByKommune(Reol r)
        {
            foreach (ReolTree node in Nodes)
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
            ReolTree newNode = new ReolTree();
            newNode.Insert(r);
            Nodes.Add(newNode);
        }

        public void InsertByTeam(Reol r)
        {
            foreach (ReolTree node in Nodes)
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
            ReolTree newNode = new ReolTree();
            newNode.Insert(r);
            Nodes.Add(newNode);
        }

        public void InsertByPostalZone(Reol r)
        {
            foreach (ReolTree node in Nodes)
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
            ReolTree newNode = new ReolTree();
            newNode.Insert(r);
            Nodes.Add(newNode);
        }

        public void Insert(Reol r)
        {
            ReolTree newNode = new ReolTree();
            newNode.LeafValue = r;
            Nodes.Add(newNode);
        }

        public AntallInformation FindAntall()
        {
            if (this.IsLeafNode)
                return this.LeafValue.Antall;
            AntallInformation sum = new AntallInformation();
            foreach (ReolTree childNode in this.Nodes)
                sum.Accumulate(childNode.FindAntall());
            return sum;
        }

        public AntallInformation FindAntallprZone(int zone)
        {
            if (this.IsLeafNode)
            {
                if (this.LeafValue.PrisSone == zone)
                    return this.LeafValue.Antall;
            }

            AntallInformation sum = new AntallInformation();
            foreach (ReolTree childNode in this.Nodes)
                sum.Accumulate(childNode.FindAntallprZone(zone));
            return sum;
        }
    }
}
