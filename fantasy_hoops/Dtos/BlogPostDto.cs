using System;
using fantasy_hoops.Models.Enums;

namespace fantasy_hoops.Dtos
{
    public class BlogPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public UserDto Author { get; set; }    
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public PostStatus Status { get; set; }
    }
}