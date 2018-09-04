using System.Collections.Generic;

namespace WordSpy.Models
{
    public class PauseResult
    {
        public List<SearchResult> Result { get; set; }
        public PauseResult(List<SearchResult> Result)
        {
            this.Result = Result;
        }
    }
}
