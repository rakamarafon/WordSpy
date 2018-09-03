using System.Collections.Generic;

namespace WordSpy.Models
{
    public class Node
    {
        private List<Node> _nodesList = new List<Node>();
        public string Link { get; set; }
        public List<Node> Nodes
        {
            get
            {
                return _nodesList;
            }
        }

        public Node(string link)
        {
            Link = link;
        }
        public void isNodeOf(Node node)
        {
            _nodesList.Add(node);
        }

        public void isNodeOfRange(int index, List<string> links, int deep)
        {
            int nC = _nodesList.Count;
            for (int i = 0; i < nC; i++)
            {
                if (links.Count == 0) continue;
                for (int j = 0; j < deep; j++)
                {                   
                    _nodesList[i].Nodes.Add(new Node(links[j]));
                }
            }
        }
    }
}
