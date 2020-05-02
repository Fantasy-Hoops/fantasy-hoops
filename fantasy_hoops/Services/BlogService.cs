using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }


        public bool ApprovePost(int postId)
        {
            return _blogRepository.ChangePostStatus(postId, PostStatus.APPROVED);
        }
    }
}
