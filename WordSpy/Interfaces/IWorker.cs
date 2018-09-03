using System.Collections.Generic;
using WordSpy.Models;

namespace WordSpy.Interfaces
{
    public interface IWorker
    {
        int Threads { get; set; }
        string Word { get; set; }
        bool isRun { get; set; }
        Node Root { get; set; }
        List<SearchResult> GetResults();
        int GetDonePersent();
        void Run();
        void Wait();
        void Interrupt();
        void Stop();        
    }
}
