using System.Collections.Generic;
using WordSpy.Models;

namespace WordSpy.Interfaces
{
    public interface ISearch
    {
        SearchResult Search(Node root, string textToSearch);
        Node BuildGraph(int deep, string rootLink, List<string> links);
        void AddChildsToGraph(Node rootNode, List<string> childs);
    }
}
