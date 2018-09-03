using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using WordSpy.Interfaces;
using WordSpy.Models;
using WordSpy.Services;

namespace WordSpy.Tests
{
    [TestFixture]
    public class SearchServiceTests
    {
        private ISearch _service;
        private IDownload _fakeDownload;

        public SearchServiceTests()
        {
            _fakeDownload = A.Fake<IDownload>();      
        }

        [SetUp]
        public void Init()
        {
            _service = new SearchService(_fakeDownload);
        }

        [TearDown]
        public void Dispose()
        {
            _service = null;
        }

        [Test]
        public void BuildGraph_with_deep_rootLink_childLinks_expect_valid_Graph()
        {
            #region Average 
            int deep = 2;
            string rootLink = "http://rootLink.com";
            List<string> childs = new List<string> { "http://child.com", "http://child.com", "http://child.com", "http://child.com", "http://child.com", "http://child.com", "http://child.com", "http://child.com", "http://child.com", "http://child.com" };
            List<string> fakeLinks = new List<string> { "http://fake.com" , "http://fake.com" , "http://fake.com" , "http://fake.com" , "http://fake.com" , "http://fake.com" , "http://fake.com" , "http://fake.com" , "http://fake.com" , "http://fake.com" };

            A.CallTo(_fakeDownload)
                .Where(call => call.Method.Name == "GetHTML")
                .WithReturnType<string>()
                .Returns("fake html");

            A.CallTo(_fakeDownload)
                .Where(call => call.Method.Name == "GetUrls")
                .WithReturnType<IEnumerable<string>>()
                .Returns(fakeLinks);
            #endregion

            #region Act
            Node result = _service.BuildGraph(2, rootLink, childs);

            int expectChilds = childs.Count;
            int actualChilds = result.Nodes.Count;

            int expectNodesInChilds = deep;
            int actualNodesInChilds = result.Nodes[deep].Nodes.Count;
            #endregion

            #region Assert
            Assert.AreEqual(expectChilds, actualChilds);
            Assert.AreEqual(expectNodesInChilds, actualNodesInChilds);
            #endregion
        }

        [Test]
        public void Search_takes_graph_and_textToSearch_returns_valid_result()
        {
            string text = "testSearch";
            string url = "Fake";
            List<string> words = new List<string> { text, text, text, text, text };
            List<string> childs = new List<string>();            
            Node fakeNode = new Node(url);

            A.CallTo(_fakeDownload)
                 .Where(call => call.Method.Name == "GetHTML")
                 .WithReturnType<string>()
                 .Returns(string.Format("<html> <head> <title>First case html</title> </head> <body> <div>some{0} text some text {1} <a href=>link</a></div> {2} <div>another text {3} <a href=>link</a> text text</div>{4}</body> </html>", text, text, text, text, text));

            A.CallTo(_fakeDownload)
                .Where(call => call.Method.Name == "GetUrls")
                .WithReturnType<IEnumerable<string>>()
                .Returns(childs);

            SearchResult expect = new SearchResult { URL = url, Childs = childs, Words = words };
            SearchResult result = _service.Search(fakeNode, text);

            Assert.IsTrue(expect.Equals(result));
        }        
    }
}
