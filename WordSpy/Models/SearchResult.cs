using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordSpy.Models
{
    public class SearchResult
    {
        public string URL { get; set; }
        public List<string> Childs { get; set; }
        public List<string> Words { get; set; }
    }
}
