using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WordSpy.Interfaces;
using WordSpy.Models;

namespace WordSpy.Services
{
    public class WorkerPoolService : IWorker
    {
        private ISearch _service;
        private IDownload _download;
        private List<Task> _threads;
        private object _block = new object();

        public int Threads { get; set; }
        public string Word { get; set; }
        public bool isRun { get; set; }
        public Node Root { get; set; }
        public int Deep { get; set; }

        public volatile List<SearchResult> Results;
        volatile Semaphore semaphore;
        volatile int count;
        AutoResetEvent autoEvent = new AutoResetEvent(false);
        public WorkerPoolService(ISearch service, IDownload download)
        {
            _service = service;
            _download = download;
            _threads = new List<Task>();
            Results = new List<SearchResult>();
            isRun = false;
        }

        public int GetDonePersent()
        {
            return 0;
        }

        public List<SearchResult> GetResults()
        {
            return Results;
        }

        public void Init(Node root, int threads, string searchText, int deep)
        {
            Root = root;
            Threads = threads;
            Word = searchText;
            semaphore = new Semaphore(threads, threads);
            count = threads;
            Deep = deep;
        }

        public void Interrupt()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            foreach (var item in Root.Nodes)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Search), autoEvent);
            }                                   
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Wait()
        {
            autoEvent.WaitOne();
        }

        private void Search(object state)
        {
            Node node;
            while (count > 0)
            {
                semaphore.WaitOne();
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
                semaphore.Release();
                count--;
            }
        }
    }
}

