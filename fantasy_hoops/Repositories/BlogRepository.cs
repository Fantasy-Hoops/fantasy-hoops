﻿using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly GameContext _context;

        public BlogRepository(GameContext context = null)
        {
            _context = context ?? new GameContext();
        }

        public List<BlogPostDto> GetApprovedPosts()
        {
            return _context.Posts
                .Where(post => post.Status == PostStatus.APPROVED)
                .Select(post => new BlogPostDto
                {
                    Id = post.PostID,
                    Title = post.Title,
                    Body = post.Body,
                    Author = new UserDto
                    {
                        UserId = post.Author.Id,
                        AvatarUrl = post.Author.AvatarURL,
                        Username = post.Author.UserName
                    },
                    CreatedAt = post.CreatedAt,
                    ModifiedAt = post.ModifiedAt
                })
                .OrderByDescending(post => post.CreatedAt)
                .ToList();
        }

        public List<BlogPostDto> GetUnapprovedPosts()
        {
            return _context.Posts
                .Where(post => post.Status != PostStatus.APPROVED)
                .Select(post => new BlogPostDto
                {
                    Id = post.PostID,
                    Title = post.Title,
                    Body = post.Body,
                    Author = new UserDto
                    {
                        UserId = post.Author.Id,
                        AvatarUrl = post.Author.AvatarURL,
                        Username = post.Author.UserName
                    },
                    CreatedAt = post.CreatedAt,
                    ModifiedAt = post.ModifiedAt
                })
                .OrderByDescending(post => post.CreatedAt)
                .ToList();
        }

        public BlogPostDto GetPostById(int postId)
        {
            Post post = _context.Posts.Find(postId);
            if (post == null)
            {
                return null;
            }

            return new BlogPostDto
            {
                Id = post.PostID,
                Title = post.Title,
                Body = post.Body,
                Author = new UserDto
                {
                    UserId = post.Author.Id,
                    AvatarUrl = post.Author.AvatarURL,
                    Username = post.Author.UserName
                },
                CreatedAt = post.CreatedAt,
                ModifiedAt = post.ModifiedAt
            };
        }

        public bool AddPost(SubmitPostViewModel model)
        {
            _context.Posts.Add(
                new Post
                {
                    Title = model.Title,
                    Body = model.Body,
                    AuthorID = model.AuthorID,
                    CreatedAt = CommonFunctions.Instance.EtcNow(),
                    ModifiedAt = CommonFunctions.Instance.EtcNow(),
                    Status = model.Status
                });
            return _context.SaveChanges() != 0;
        }

        public bool UpdatePost(SubmitPostViewModel model)
        {
            Post editablePost = _context.Posts.Find(model.Id);
            if (editablePost == null)
            {
                return false;
            }
            if (editablePost.Title.Equals(model.Title) && editablePost.Body.Equals(model.Body))
            {
                return true;
            }
            editablePost.Title = model.Title ?? editablePost.Title;
            editablePost.Body = model.Body ?? editablePost.Body;
            editablePost.ModifiedAt = CommonFunctions.Instance.EtcNow();
            return _context.SaveChanges() != 0;
        }

        public bool PostExists(int id)
        {
            return _context.Posts.Any(post => post.PostID == id);
        }

        public bool DeletePost(int id)
        {
            Post postToDelete = _context.Posts.Find(id);
            if (postToDelete == null)
            {
                return false;
            }

            _context.Posts.Remove(postToDelete);
            return _context.SaveChanges() != 0;
        }

        public bool ChangePostStatus(int postId, PostStatus status)
        {
            Post post = _context.Posts.Find(postId);
            if (post == null)
            {
                return false;
            }

            if (post.Status == status)
            {
                return true;
            }

            post.Status = PostStatus.APPROVED;
            post.ModifiedAt = CommonFunctions.Instance.UTCToEastern(DateTime.UtcNow);
            return _context.SaveChanges() != 0;
        }
    }
}