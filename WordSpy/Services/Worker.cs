using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WordSpy.Interfaces;
using WordSpy.Models;

namespace WordSpy.Services
{
    public class Worker : IWorker
    {
        private ISearch _service;
        private IDownload _download;
        private object _block = new object();
        private List<Thread> _threads;
        private int _maxPrc;

        public int Threads { get; set; }
        public string Word { get; set; }
        public Node Root { get; set; }
        public bool isRun { get; set; }
        public int Deep { get; set; }

        public volatile List<SearchResult> Results;
        
        public Worker(ISearch service, IDownload download)
        {            
            _service = service;
            _download = download;
            _threads = new List<Thread>();
            Results = new List<SearchResult>();
            isRun = false;
        }
        public void Init(Node root, int threads, string searchText, int deep)
        {
            Root = root;
            Threads = threads;
            Word = searchText;
            Deep = deep;
            _maxPrc = Root.Nodes.Count;
        }
        public List<SearchResult> GetResults()
        {
            return Results;
        }
        public void Run()
        {
            for(int i = 0; i < Threads; i ++)
            {
                Thread thread = new Thread(Search);
                _threads.Add(thread);
                thread.Start();
            }
        }

        public void Wait()
        {
            foreach (var item in _threads)
            {
                item.Join(); 
            }
        }

        public void Resume()
        {
            isRun = true;
            lock (_threads)
            {
                Monitor.PulseAll(_threads);
            }
        }
        public void Interrupt()
        {
            isRun = false;
            foreach (var item in _threads)
            {
                item.Interrupt();
            }
        }

        public void Stop()
        {
            isRun = false;
            foreach (var item in _threads)
            {
                item.Abort();
            }
            _threads.Clear();
        }

        private void Search(object state)
        {
            Node node;
            Thread.Sleep(10);
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
                var html = _download.GetHTML(node.Link);
                if (html == null) continue;
                var links = _download.GetUrls(html).ToList();
                var temp = _service.AddChildToNode(node, links, Deep);
                SearchResult result = _service.Search(temp, Word);
                if (result != null) Results.Add(result);
            }
        }        
    }
}
