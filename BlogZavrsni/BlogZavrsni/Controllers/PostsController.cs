using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogZavrsni.Data;
using BlogZavrsni.Models;
using BlogZavrsni.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BlogZavrsni.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnviroment;

        public PostsController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnviroment)
        {
            _context = context;
            _hostingEnviroment = hostingEnviroment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
              return _context.Post != null ? 
                          View(await _context.Post.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Post'  is null.");
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }


        //[Authorize(Policy = "RequireSomeUser")]
        // GET: Posts/Create
        public IActionResult Create()
        {
            var tags = _context.Tag.ToList();
            ViewBag.Tags = tags;
            PostViewModel model = new PostViewModel();
            return View(model);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel model, [FromForm] IFormFile Image)
        {
            if (ModelState.IsValid)
            {

                if (Image != null && Image.Length > 0) { 
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Image.FileName;

                    var imagePath = Path.Combine(_hostingEnviroment.WebRootPath, "images", uniqueFileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create)) { 
                        await Image.CopyToAsync(stream);
                    }
                    model.ImagePath =Path.Combine("images", uniqueFileName);
                }





                Post post = new Post()
                {
                    Title = model.Title,
                    Content = model.Content,
                    ImagePath = model.ImagePath
                };

                if (model.Comments != null && model.TagIds.Any()) { 
                
                    List<Comment> comments = new List<Comment>();

                    foreach (var comment in model.Comments) { 
                    
                        comments.Add(new Comment  {Text = comment.Text });
                    }
                    post.Comments = comments;
                }
                


                _context.Add(post);

                if (model.TagIds != null && model.TagIds.Any()) { 
                
                    var selectedTags = _context.Tag.Where(x=> model.TagIds.Contains(x.Id)).ToList();
                    foreach (var tag in selectedTags) { 
                       _context.PostTag.Add(new PostTag { Post= post, Tag = tag });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Tags = _context.Tag.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(NewCommentViewModel model) 
        {
            var post = await _context.Post.Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == model.postId);
            if (post == null) { 
                return NotFound(); 
            }

            var comment = new Comment { Text = model.commentText };
            post.Comments.Add(comment);

             await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");


        }

        public IActionResult LikePost(int postId) {

           

            var likes = HttpContext.Session.GetString("LikedPosts") ?? "";
            var likedPosts = likes.Split(',').Where(x=> !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

            

            if (!likedPosts.Contains(postId))
            { 
                likedPosts.Add(postId);
                HttpContext.Session.SetString("LikedPosts", string.Join(",", likedPosts));
            }

            var post = _context.Post.Find(postId);
            if (post != null) {
                post.Likes++;
                _context.SaveChanges();
            }

            ViewBag.LikedPosts= likedPosts;
           

            return RedirectToAction("Index", "Home");
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
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
            if (_context.Post == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Post'  is null.");
            }
            var post = await _context.Post.FindAsync(id);
            if (post != null)
            {
                _context.Post.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return (_context.Post?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
