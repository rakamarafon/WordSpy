using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WordSpy.Interfaces;
using WordSpy.Models;

namespace WordSpy.Services
{
    public class Worker : IWorker
    {
        public int Threads { get; set; }
        public string Word { get; set; }
        public Node Root { get; set; }
        public List<SearchResult> Results { get; set; }
        private ISearch _service;
        private IDownload _download;
        private object _block = new object();

        public Worker(ISearch service, IDownload download)
        {
            Results = new List<SearchResult>();
            _service = service;
            _download = download;
        }
       
        public void Run()
        {
            for(int i = 0; i < Threads; i ++)
            {
                Thread thread = new Thread(Search);
                thread.Start();
            }
        }

        private void Search()
        {
            Node node;
            while (Root.Nodes.Count != 0)
            {
                lock (_block)
                {
                    node = Root.Nodes.FirstOrDefault();
                    if (node != null)
                    {
                        Root.Nodes.Remove(node);
                    }
                }
                SearchResult result = _service.Search(node, Word);
                if(result != null) Results.Add(result);
            }
        }
    }
}
