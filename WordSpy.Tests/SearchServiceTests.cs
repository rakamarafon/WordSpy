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
    }
}
