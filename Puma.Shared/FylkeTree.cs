using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class FylkeTree
    {
        private List<FylkeTree> _Nodes = new List<FylkeTree>();

        public List<FylkeTree> Nodes
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

        private Kommune _LeafValue;

        public Kommune LeafValue
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

        public Kommune GetFirstLeafValue()
        {
            if (IsLeafNode)
                return LeafValue;
            if (Nodes.Count == 0)
                return null/* TODO Change to default(_) if this is not a reference type */;
            return Nodes[0].GetFirstLeafValue();
        }

        public void InsertByFylkeKommune(Kommune r)
        {
            foreach (FylkeTree node in Nodes)
            {
                Kommune leaf = node.GetFirstLeafValue();
                if (leaf != null)
                {
                    if (leaf.FylkeID == r.FylkeID)
                    {
                        node.InsertByKommune(r);
                        return;
                    }
                }
            }
            FylkeTree newNode = new FylkeTree();
            newNode.InsertByKommune(r);
            Nodes.Add(newNode);
        }

        public void InsertByKommune(Kommune r)
        {
            foreach (FylkeTree node in Nodes)
            {
                Kommune leaf = node.GetFirstLeafValue();
                if (leaf != null)
                {
                    if (leaf.KommuneID == r.KommuneID)
                    {
                        node.Insert(r);
                        return;
                    }
                }
            }
            FylkeTree newNode = new FylkeTree();
            newNode.Insert(r);
            Nodes.Add(newNode);
        }

        public void Insert(Kommune r)
        {
            FylkeTree newNode = new FylkeTree();
            newNode.LeafValue = r;
            Nodes.Add(newNode);
        }
    }
}
