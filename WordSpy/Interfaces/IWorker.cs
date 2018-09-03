﻿using System.Collections.Generic;
using WordSpy.Models;

namespace WordSpy.Interfaces
{
    public interface IWorker
    {
        int Threads { get; set; }
        string Word { get; set; }
        Node Root { get; set; }

        List<SearchResult> Results { get; set; }
        void Run();
    }
}
