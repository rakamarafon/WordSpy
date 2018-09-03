using System.Collections.Generic;

namespace WordSpy.Interfaces
{
    public interface IDownload
    {
        string GetHTML(string url);
        IEnumerable<string> GetUrls(string html);
    }
}
