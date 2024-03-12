using BlogZavrsni.Data;
using BlogZavrsni.Models;
using BlogZavrsni.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BlogZavrsni.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {   

            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string searchTerm)
        {
            var postsQuery = _context.Post.Include(x=>x.PostTags).Include(x=>x.Comments).AsQueryable();
            
            if(!string.IsNullOrEmpty(searchTerm))
            {
                postsQuery = postsQuery.Where(x=>x.Title.Contains(searchTerm) || x.Content.Contains(searchTerm));
            }

            var posts = postsQuery.ToList();
            

            var tags = _context.Tag.ToList();

            var postViewModels = new List<PostViewModel>();

            foreach (var post in posts) 
            {
                var postViewModel = new PostViewModel
                {   
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    TagIds = new List<int>(),
                    Comments = new List<CommentViewModel>(),
                    ImagePath = post.ImagePath,
                    Likes = post.Likes

                };
                foreach (var postTag in post.PostTags) 
                {
                    postViewModel.TagIds.Add(postTag.TagId);
                }

                foreach(var comment in post.Comments)
                {
                    postViewModel.Comments.Add(new CommentViewModel { Text = comment.Text });
                }

                postViewModels.Add(postViewModel);
            }

            var likes = HttpContext.Session.GetString("LikedPosts") ?? "";
            var likedPosts = likes.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

            ViewBag.LikedPosts = likedPosts;
            ViewBag.Tags = tags;
            return View(postViewModels);
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
