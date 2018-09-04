using System.Collections.Generic;
using WordSpy.Models;

namespace WordSpy.Interfaces
{
    public interface ISearch
    {
        SearchResult Search(Node root, string textToSearch);
        Node BuildGraph(string rootLink, List<string> links);
        Node AddChildToNode(Node node, List<string> links, int deep);
    }
}
