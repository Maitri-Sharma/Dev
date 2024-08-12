using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class TreeViewControlNode
    {
        private NewReolTree _newReolTree = null/* TODO Change to default(_) if this is not a reference type */;

        public TreeViewControlNode(NewReolTree tree)
        {
            this._newReolTree = tree;
        }

        public NewReolTree NewReolTree
        {
            get
            {
                return _newReolTree;
            }
        }

        public bool IsRootNode
        {
            get
            {
                return (this.Type == PumaEnum.NodeType.Root);
            }
        }

        private TreeViewControlNode _parentNode = null;

        public TreeViewControlNode ParentNode
        {
            get
            {
                return this._parentNode;
            }
            set
            {
                this._parentNode = value;
            }
        }

        public void SortChildNodes()
        {
            this.Nodes.Sort(new TreeViewControlNodeComparer());
        }

        private class TreeViewControlNodeComparer : IComparer<TreeViewControlNode>
        {
            public int Compare(TreeViewControlNode x, TreeViewControlNode y)
            {
                return System.String.Compare(x.Text, y.Text);
            }
        }


        private List<TreeViewControlNode> _nodes = new List<TreeViewControlNode>();

        public List<TreeViewControlNode> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        public bool HasTeamDifferentChildNodeSameId(TreeViewControlNode n)
        {
            if (n.Nodes.Count == 0)
                return false;
            if (n.Type != PumaEnum.NodeType.Team)
                throw new Exception("n is not a team-node!");

            if (n.Nodes[0].Type == PumaEnum.NodeType.Team)
            {
                foreach (TreeViewControlNode childNode in this.Nodes)
                {
                    if (childNode == n)
                        return false;
                    if (System.Convert.ToString(childNode.NewReolTree.Id) == System.Convert.ToString(n.NewReolTree.Id))
                        return true;
                }
            }

            return false;
        }

        private PumaEnum.NodeCheckStatus _nodeCheckStatus;

        public PumaEnum.NodeCheckStatus CheckStatus
        {
            get
            {
                return this._nodeCheckStatus;
            }
            set
            {
                this._nodeCheckStatus = value;
            }
        }

        public void UpdateNodeStatusForConnectedNodes()
        {
            this.RefreshCheckStatusForDescendants();
            this.RefreshCheckStatusForAnscestors();
        }

        private void SetAllDescendantsAsChecked(List<TreeViewControlNode> nodeCollection)
        {
            foreach (TreeViewControlNode node in nodeCollection)
            {
                node.CheckStatus = PumaEnum.NodeCheckStatus.NodeIsChecked;
                SetAllDescendantsAsChecked(node.Nodes);
            }
        }

        private void SetAllDescendantsAsNotChecked(List<TreeViewControlNode> nodeCollection)
        {
            foreach (TreeViewControlNode node in nodeCollection)
            {
                node.CheckStatus = PumaEnum.NodeCheckStatus.NodeAndChildNodesIsNotChecked;
                SetAllDescendantsAsNotChecked(node.Nodes);
            }
        }

        protected void RefreshCheckStatusForDescendants()
        {
            if (this.CheckStatus == PumaEnum.NodeCheckStatus.NodeAndChildNodesIsNotChecked)
                this.SetAllDescendantsAsNotChecked(this.Nodes);
            else if (this.CheckStatus == PumaEnum.NodeCheckStatus.NodeIsChecked)
                SetAllDescendantsAsChecked(this.Nodes);
        }

        protected void RefreshCheckStatusForAnscestors()
        {
            if (this.Nodes.Count > 0)
            {
                bool hasDescendantsRegisteredAsNotChecked = false;
                bool hasDescendantsRegisteredAsChecked = false;

                foreach (TreeViewControlNode node in this.Nodes)
                {
                    switch (node.CheckStatus)
                    {
                        case PumaEnum.NodeCheckStatus.NodeIsChecked:
                            {
                                hasDescendantsRegisteredAsChecked = true;
                                break;
                            }

                        case PumaEnum.NodeCheckStatus.NodeAndChildNodesIsNotChecked:
                            {
                                hasDescendantsRegisteredAsNotChecked = true;
                                break;
                            }

                        case PumaEnum.NodeCheckStatus.SomeChildNodesAreChecked:
                            {
                                hasDescendantsRegisteredAsNotChecked = true;
                                hasDescendantsRegisteredAsChecked = true;
                                break;
                            }

                        default:
                            {
                                throw new Exception("One node has unsettled checkstatus!");
                            }
                    }
                }

                if (!hasDescendantsRegisteredAsNotChecked)
                    this.CheckStatus = PumaEnum.NodeCheckStatus.NodeIsChecked;
                else if (hasDescendantsRegisteredAsChecked)
                    this.CheckStatus = PumaEnum.NodeCheckStatus.SomeChildNodesAreChecked;
                else
                    this.CheckStatus = PumaEnum.NodeCheckStatus.NodeAndChildNodesIsNotChecked;
            }

            if (this.ParentNode != null)
                this.ParentNode.RefreshCheckStatusForAnscestors();
        }

        private AntallInformation _antallInfo = null/* TODO Change to default(_) if this is not a reference type */;

        public AntallInformation AntallInformation
        {
            get
            {
                return this._antallInfo;
            }
            set
            {
                this._antallInfo = value;
            }
        }

        private Dictionary<int, AntallInformation> _antallInfoByZone = new Dictionary<int, AntallInformation>();

        public void SetAntallInfoByZone(int zone, AntallInformation antInfo)
        {
            this._antallInfoByZone[zone] = antInfo;
        }

        public AntallInformation GetAntallInfoByZone(int zone)
        {
            return _antallInfoByZone[zone];
        }

        private PumaEnum.NodeType _type = PumaEnum.NodeType.Unsettled;

        public PumaEnum.NodeType Type
        {
            get
            {
                return this._type;
            }
            set
            {
                if (value == PumaEnum.NodeType.Root)
                {
                    this._antallInfo = null;
                    this._antallInfoByZone.Clear();
                    this._nodeCheckStatus = PumaEnum.NodeCheckStatus.Unsettled;
                }

                this._type = value;
            }
        }

        public string Text
        {
            get
            {
                return this.NewReolTree.Text;
            }
        }

        private List<TreeViewControlNode> _teamNodesWithSameId = new List<TreeViewControlNode>();

        public List<TreeViewControlNode> TeamNodesWithSameId
        {
            get
            {
                return _teamNodesWithSameId;
            }
        }

    }
}
