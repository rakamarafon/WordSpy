using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WordSpy.Interfaces;
using WordSpy.Services;

namespace WordSpy.Tests
{
    [TestFixture]
    public class DownloadServiceTests
    {
        private IDownload _download;

        [SetUp]
        public void Init()
        {
            _download = new DownloadService();
        }

        [TearDown]
        public void Dispose()
        {
            _download = null;
        }

        [Test]
        public void GetUrls_from_html_returns_expected_links()
        {
            string source = string.Format("<html> <head> <title>First case html</title> </head> <body> <div>some text some text <a href={0}>link</a></div> <div>another text <a href={1}>link</a> text text</div></body> </html>", @"""http://google.com""", @"""google.com""");
            List<string> expected = new List<string> { new string("http://google.com") };

            var result = _download.GetUrls(source).ToList().Count;
            int expectCount = expected.Count;

            Assert.AreEqual(expectCount, result);
        }
    }
}
