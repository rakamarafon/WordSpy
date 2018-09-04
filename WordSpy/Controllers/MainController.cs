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
            ViewData["Message"] = "How to use WordSpy:";

            return View();
        }

        public IActionResult Contact()
        {
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
            if (html == null) return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            var links = _download.GetUrls(html);
            Node root = _service.BuildGraph(value.URL, links.ToList());
            _worker.Init(root, value.MaxThreads, value.TextToFind, value.MaxScanURLs);
            _worker.Run();
            _worker.Wait();
            _worker.isRun = false;
            return View("ResultView", _worker.GetResults().OrderByDescending(x => x.Words.Count).Distinct().ToList());
        }

        public IActionResult PauseSearch()
        {
            if (_worker.isRun == false) return View("Index");
            _worker.Interrupt();
            PauseResult pauseResult = new PauseResult(_worker.GetResults().OrderByDescending(x => x.Words.Count).Distinct().ToList());
            return View("PausedView", pauseResult);
        }
        public IActionResult ResumeSearch()
        {
            _worker.Resume();
            //_worker.Wait();
            return View("ResultView", _worker.GetResults().OrderByDescending(x => x.Words.Count).Distinct().ToList());
        }
        public IActionResult StopSearch()
        {
            if (_worker.isRun == false) return View("Index");
            _worker.Stop();
            return View("ResultView", _worker.GetResults().OrderByDescending(x => x.Words.Count).Distinct().ToList());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
