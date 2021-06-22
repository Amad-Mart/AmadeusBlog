using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AmadeusBlog.Data;
using AmadeusBlog.Models;
using AmadeusBlog.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AmadeusBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;    
        private readonly IBlogFileService _fileService;

        public HomeController(
            ILogger<HomeController> logger, 
            ApplicationDbContext context, 
            IBlogFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(int? page = 1)
        {
            ViewData["HeaderText"] = "The Landing Page";
            ViewData["SubText"] = "Welcome to my landing page";
            //var imageData = _fileservice.EncodeFileAsync("home-bg.jpg");
            //ViewData["HeaderImage"] = _fileSerivce.DecodeImage(imageData, "jpg");

            var pageSize = 1;


            var allBlogs = await _context.Blog
                                    .OrderByDescending(b => b.Created)
                                    .ToPagedListAsync(page, pageSize);
            return View(allBlogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
