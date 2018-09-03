using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WordSpy.Interfaces;
using WordSpy.Models;

namespace WordSpy.Services
{
    public class Worker : IWorker
    {
        public int Threads { get; set; }
        public string Word { get; set; }
        public Node Root { get; set; }
        public static List<SearchResult> Results { get; set; }
        private ISearch _service;
        private IDownload _download;
        private object _block = new object();
        private List<Task> _threads;

        static Worker()
        {
            Results = new List<SearchResult>();
        }
        public Worker(ISearch service, IDownload download)
        {            
            _service = service;
            _download = download;
            _threads = new List<Task>();
        }
       
        public void Run()
        {
            for(int i = 0; i < Threads; i ++)
            {
                Task thread = new Task(Search);
                _threads.Add(thread);
                thread.Start();
            }
        }

        public void Wait()
        {
            foreach (var item in _threads)
            {
                item.Wait();
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

        public List<SearchResult> GetResults()
        {
            return Results;
        }       
    }
}
