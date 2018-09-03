using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WordSpy.DTO_s;
using WordSpy.Interfaces;
using WordSpy.Models;

namespace WordSpy.Controllers
{
    public class MainController : Controller
    {
        private ISearch _service;
        private IDownload _download;
        private IWorker _worker;

        public MainController(ISearch service, IDownload download, IWorker worker)
        {
            _service = service;
            _download = download;
            _worker = worker;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult StartParamViewSubmit([FromForm] StartParamsDTO value)
        {
            var html = _download.GetHTML(value.URL);
            var links = _download.GetUrls(html);
            Node root = _service.BuildGraph(value.MaxScanURLs, value.URL, links.ToList());
            _worker.Root = root;
            _worker.Threads = value.MaxThreads;
            _worker.Word = value.TextToFind;
            _worker.Run();
            
            //SearchResult result = _service.Search(root, value.TextToFind);
            //if(result == null) { return BadRequest("Not Found"); }
            return View("ResultView", _worker.Results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
