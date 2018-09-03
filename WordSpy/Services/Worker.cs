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
        private int _prcToDone;

        public int Threads { get; set; }
        public string Word { get; set; }
        public Node Root { get; set; }
        public bool isRun { get; set; }

        public volatile List<SearchResult> Results;
        
        public Worker(ISearch service, IDownload download)
        {            
            _service = service;
            _download = download;
            _threads = new List<Thread>();
            Results = new List<SearchResult>();
            isRun = false;
        }
        public void Init(Node root, int threads, string searchText)
        {
            Root = root;
            Threads = threads;
            Word = searchText;
        }
        public List<SearchResult> GetResults()
        {
            return Results;
        }
        public int GetDonePersent()
        {
            if (Root == null) return 0;
            int current = Root.Nodes.Count;
            return 0;
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
                foreach (var item in node.Nodes)
                {
                    SearchResult result = _service.Search(item, Word);
                    if (result != null) Results.Add(result);
                }                
            }
        }        
    }
}
