using BlogZavrsni.Models;
using System.ComponentModel.DataAnnotations;

namespace BlogZavrsni.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Naslov je obavezan!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Sadržaj je obavezan!")]
        public string Content { get; set; }

        public string? ImagePath { get; set; }
        [Required(ErrorMessage = "Tagovi su obavezni!")]


        public List<int>? TagIds { get; set; }

        public List<CommentViewModel>? Comments { get; set; }

        public int Likes { get; set; }
    }
}
