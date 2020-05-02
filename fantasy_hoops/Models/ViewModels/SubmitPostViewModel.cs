using fantasy_hoops.Models.Enums;

namespace fantasy_hoops.Models.ViewModels
{
    public class SubmitPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string AuthorID { get; set; }
        public PostStatus Status { get; set; }
    }
}
