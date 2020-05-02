using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IBlogRepository
    {
        List<BlogPostDto> GetApprovedPosts();
        List<BlogPostDto> GetUnapprovedPosts();
        BlogPostDto GetPostById(int postId);
        bool PostExists(int id);
        bool AddPost(SubmitPostViewModel model);
        bool UpdatePost(SubmitPostViewModel model);
        bool DeletePost(int id);
        bool ChangePostStatus(int postId, PostStatus status);
    }
}
