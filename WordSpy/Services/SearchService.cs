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

        public Node AddChildToNode(Node node, List<string> links, int deep)
        {
            Node temp = node;
            int count = 0;
            foreach (var item in links)
            {
                if (count == deep) break;
                temp.isNodeOf(new Node(item));
                count++;
            }
            return temp;
        }

        public Node BuildGraph(int deep, string rootLink, List<string> links)
        {
            Node graph = new Node(rootLink);
            foreach (var item in links)
            {
                graph.isNodeOf(new Node(item));
            }
            //int count = graph.Nodes.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    var html = _service.GetHTML(graph.Nodes[i].Link);
            //    if (html == null) continue;
            //    var nodeLinks = _service.GetUrls(html).ToList();
            //    var linksCount = nodeLinks.Count;
            //    if (linksCount == 0) continue;
            //    for (int j = 0; j < deep; j++)
            //    {
            //        if (linksCount == j) break;
            //        graph.Nodes[i].isNodeOf(new Node(nodeLinks[j]));                    
            //    }
            //    nodeLinks.Clear();
            //}
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
                if (item.ToLower().Contains(textToSearch.ToLower())) list.Add(item);
            }
            return list;
        }
    }
}
