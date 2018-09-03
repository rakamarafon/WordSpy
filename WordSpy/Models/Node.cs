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
    }
}
