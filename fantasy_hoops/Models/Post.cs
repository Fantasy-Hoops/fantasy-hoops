using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fantasy_hoops.Models
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string AuthorID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        [ForeignKey("AuthorID")]
        public virtual User Author { get; set; }
    }
}
