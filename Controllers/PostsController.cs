using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AmadeusBlog.Data;
using AmadeusBlog.Models;
using AmadeusBlog.Services;
using Microsoft.Extensions.Configuration;
using X.PagedList;

namespace AmadeusBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly IBlogFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly BasicSlugService _slugService;
        private readonly SearchService _searchService;

        public PostsController(
            ApplicationDbContext context,
            IBlogFileService fileService,
            IConfiguration configuration,
            BasicSlugService slugService, 
            SearchService searchService)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
            _slugService = slugService;
            _searchService = searchService;
        }

        public async Task<ActionResult> BlogPostIndex(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = _context.Blog.Find(id);
            var blogPosts = await _context.Post.Where(p => p.BlogId == id).ToListAsync();

            ViewData["HeaderText"] = blog.Name;
            ViewData["SubText"] = blog.Description;
            ViewData["HeaderImage"] = _fileService.DecodeImage(blog.ImageData, blog.ContentType);

            return View(blogPosts);
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            ViewData["HeaderText"] = "The Post Index";
            ViewData["SubText"] = "Read all my glorious posts";

            var applicationDbContext = _context.Post.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        //// GET: Posts/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Post
        //        .Include(p => p.Blog)
        //        .Include(p => p.Comments)
        //        .ThenInclude(c => c.Author)
        //        .FirstOrDefaultAsync(m => m.Id == id);

        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    //_headerService.Set(post.ImageData, post.ContentType, post.Title, post.Abstract, post.Created);               
        //    return View(post);
        //}


        // GET: Posts/Details/5

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            //_headerService.Set(post.ImageData, post.ContentType, post.Title, post.Abstract, post.Created);               
            return View(post);
        }
        public IActionResult Create(int? blogId)
        {
            var post = new Post();
            if (blogId is null)
            {
                ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name");
            }
            else
            {
                post.BlogId = (int)blogId;
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchIndex(int? page, string searchString)
        {
            ViewData["SearchString"] = searchString;

            //step 1: need a set of results stemming from this search string
            var posts = _searchService.SearchContent(searchString);

            var pageNumber = page ?? 1;
            var pageSize = 2;
            

            return View( await posts.ToPagedListAsync(pageNumber, pageSize));
        }



        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,PublishState,ImageFile")] Post post/*, List<string> TagValues*/)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;

                post.ImageData = (await _fileService.EncodeFileAsync(post.ImageFile)) ??
                  await _fileService.EncodeFileAsync(_configuration["DefaultPostImage"]);

                post.ContentType = post.ImageFile is null ?
                                    _configuration["DefaultPostImage"].Split('.')[1] :
                                    _fileService.ContentType(post.ImageFile);

                //Slug stuff begins here
                var slug = _slugService.UrlFriendly(post.Title);
                if (!_slugService.IsUnique(slug))
                {
                    //I must now add a Model Error and inform the user of the problem
                    ModelState.AddModelError("Title", "There is an issue with the Title, please try again.");
                    return View(post);
                }

                post.Slug = slug;
                _context.Add(post);
                await _context.SaveChangesAsync();
                /*foreach()*/
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId });

            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Description", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Created,Slug,Title,Abstract,Content,ImageFile,ImageData,PublishState")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newSlug = _slugService.UrlFriendly(post.Title);
                    if (post.Slug != newSlug)
                    {
                        if (!_slugService.IsUnique(newSlug))
                        {
                            ModelState
                                .AddModelError(
                                    "Title",
                                    "There is an issue with the Title, please try again.");
                            return View(post);
                        }
                        post.Slug = newSlug;
                    }

                    if (post.ImageFile is not null)
                    {
                        post.ImageData = await _fileService.EncodeFileAsync(post.ImageFile);
                        post.ContentType = _fileService.ContentType(post.ImageFile);
                    }

                    post.Updated = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return View(post);
                //return RedirectToAction("BlogPostIndex.cshtml", post.Id);
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
