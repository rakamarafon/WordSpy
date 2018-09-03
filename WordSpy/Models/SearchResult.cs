using System;
using System.Collections.Generic;
using System.Linq;

namespace WordSpy.Models
{
    public class SearchResult
    {
        public string URL { get; set; }
        public List<string> Childs { get; set; }
        public List<string> Words { get; set; }

        private bool EqualsHelper(SearchResult first, SearchResult second) =>
          first.URL == second.URL                   &&
          first.Childs.SequenceEqual(second.Childs) &&
          first.Words.SequenceEqual(second.Words);

        public override bool Equals(object obj)
        {
            if ((object)this == obj)
                return true;

            var other = obj as SearchResult;

            if (other == null)
                return false;

            return EqualsHelper(this, other);
        }
        public override int GetHashCode() =>
           URL.GetHashCode()    ^
           Childs.GetHashCode() ^
           Words.GetHashCode();
    }
}
