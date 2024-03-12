

namespace BlogZavrsni.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string? ImagePath { get; set; }

        public ICollection<PostTag> PostTags { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public int Likes { get; set; }


    }
}
