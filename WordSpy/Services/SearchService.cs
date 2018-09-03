using System.Collections.Generic;
using System.Linq;
using WordSpy.Interfaces;
using WordSpy.Models;

namespace WordSpy.Services
{
    public class SearchService : ISearch
    {
        private IDownload _service;
        public SearchService(IDownload service)
        {
            _service = service;
        }

        public void AddChildsToGraph(Node rootNode, List<string> childs)
        {            
            foreach (var item in childs)
            {
                rootNode.isNodeOf(new Node(item));
            }
        }

        public Node BuildGraph(int deep, string rootLink, List<string> links)
        {
            Node graph = new Node(rootLink);
            foreach (var item in links)
            {
                graph.isNodeOf(new Node(item));
            }
            int count = graph.Nodes.Count;
            string html;
            List<string> nodeLinks;
            for (int i = 0; i < count; i++)
            {
                html = _service.GetHTML(graph.Nodes[i].Link);
                if (html == null) continue;
                nodeLinks = _service.GetUrls(html).ToList();
                graph.isNodeOfRange(i, nodeLinks, deep);
                nodeLinks.Clear();
            }
            return graph;
        }

        public SearchResult Search(Node root, string textToSearch)
        {
            Queue<Node> Q = new Queue<Node>();
            HashSet<Node> S = new HashSet<Node>();
            Q.Enqueue(root);
            S.Add(root);

            while(Q.Count > 0)
            {
                Node n = Q.Dequeue();
                var html = _service.GetHTML(n.Link);
                if (html == null) continue;
                var links = _service.GetUrls(html);                
                var words = CheckHtmlForText(html, textToSearch);
                if(words.Count > 0) return new SearchResult { URL = n.Link, Childs = links.ToList(), Words = words};
                foreach (var item in n.Nodes)
                {
                    if(!S.Contains(item))
                    {
                        Q.Enqueue(item);
                        S.Add(item);
                    }
                }
            }
            return null;
        }

        private List<string> CheckHtmlForText(string html, string textToSearch)
        {
            List<string> list = new List<string>();
            var text = html.Split(' ');
            foreach (var item in text)
            {
                if (item == textToSearch) list.Add(item);
            }
            return list;
        }
    }
}
