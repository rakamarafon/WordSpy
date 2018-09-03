using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            if (_worker.isRun == true) return View("Index");
            _worker.isRun = true;
            var html = _download.GetHTML(value.URL);
            var links = _download.GetUrls(html);
            Node root = _service.BuildGraph(value.MaxScanURLs, value.URL, links.ToList());
            ConfigurateWorker(root, value.MaxThreads, value.TextToFind);
            _worker.Run();
            _worker.Wait();
            return View("ResultView", _worker.GetResults());
        }

        public IActionResult PauseSearch()
        {
            if (_worker.isRun == false) return View("Index");
            _worker.Interrupt();
            PauseResult pauseResult = new PauseResult(_worker.GetResults());
            pauseResult.Percent = _worker.GetDonePersent();
            return View("PausedView", pauseResult);
        }
        public IActionResult StopSearch()
        {
            if (_worker.isRun == false) return View("Index");
            _worker.Stop();
            return View("ResultView", _worker.GetResults());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void ConfigurateWorker(Node root, int threads, string textToFind)
        {
            _worker.Root = root;
            _worker.Threads = threads;
            _worker.Word = textToFind;
        }
    }
}
