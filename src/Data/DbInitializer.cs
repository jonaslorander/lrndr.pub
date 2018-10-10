﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using lrndrpub.Models;

namespace lrndrpub.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context, UserManager<AppUser> um)
        {
            if(context.Authors.Any())
            {
                return;
            }

            var users = new Author[]
            {
                new Author { Fullname = "Demo0", Email = "demo0@example.com", Slug = "demo-zero", CreatedAt = DateTime.Now, CreatedBy = 1, IsAdmin = true, IdentityId = um.Users.Single(u => u.Email.Equals("admin@example.com")).Id, UserName = um.Users.Single(u => u.Email.Equals("admin@example.com")).UserName },
                new Author { Fullname = "Demo1", Email = "demo1@example.com", Slug = "demo-one", CreatedAt = DateTime.Now, CreatedBy = 1, IdentityId = um.Users.Single(u => u.Email.Equals("user@example.com")).Id, UserName = um.Users.Single(u => u.Email.Equals("user@example.com")).UserName }
            };

            foreach (Author u in users)
                context.Authors.Add(u);

            context.SaveChanges();

            var settings = new SettingsValue[]
            {
                new SettingsValue {Key = "Title", Value = "lrndr.pub", Type = SettingType.Blog, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId },
                new SettingsValue {Key = "Description", Value = "asp.net core blogging platform", Type = SettingType.Blog, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId },
                new SettingsValue {Key = "PostsPerPage", Value = "5", Type = SettingType.Blog, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId },
                new SettingsValue {Key = "EnableComments", Value = "true", Type = SettingType.Blog, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId },
                new SettingsValue {Key = "CommentingDays", Value = "90", Type = SettingType.Blog, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId },
                new SettingsValue {Key = "ModerateComments", Value = "true", Type = SettingType.Blog, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId },
                new SettingsValue {Key = "Theme", Value = "Aarsta", Type = SettingType.Blog, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId }
            };

            foreach (SettingsValue s in settings)
                context.Settings.Add(s);
            context.SaveChanges();

            var posts = new Post[]
            {
                new Post {Title = "First post", Slug = "first-post", Content = "test content is going here.<p>testing paragrapg</p>", PubDate = DateTime.Now, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId },
                new Post {Title = "Second post", Slug = "second-post", Content = "test content is going here.<p>testing paragrapg</p>", PubDate = DateTime.Now, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo1")).AuthorId },
                new Post {Title = "Third post", Slug = "third-post", Content = "test content is going here.<p>testing paragrapg</p>", PubDate = DateTime.Now, CreatedAt = DateTime.Now, CreatedBy = context.Authors.FirstOrDefault(u => u.Fullname.Equals("Demo0")).AuthorId }
            };

            foreach (Post p in posts)
                context.Posts.Add(p);
            context.SaveChanges();

            var comments = new Comment[]
            {
                new Comment { Author = "Demo89", Content = "hej hej", Email = "demo89@example.com", IsOwner = false, IsPublished = true, PubDate = DateTime.Now, PostId = 1 }
            };

            foreach (Comment c in comments)
                context.Comments.Add(c);
            context.SaveChanges();
        }
    }
}
