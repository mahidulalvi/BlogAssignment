using System;
using System.Collections.Generic;
using BlogAssignment.Models.Domain;
using BlogAssignment.Models.ViewModels;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BlogAssignment.Models;
using System.IO;

namespace BlogAssignment.Controllers
{
    public class PostsController : Controller
    {         
        private ApplicationDbContext DbContext;

        public PostsController()
        {
            DbContext = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var viewModel = DbContext.Posts
                .Select(
                post => new CreatePostViewModelForIndex
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body.Substring(0, 50),
                    Published = post.Published,
                    DateCreated = post.CreatedDate,
                    DateUpdated = post.DateUpdated,
                    MediaUrl = post.MediaUrl,
                    User = post.User.UserName
                }).ToList();


            return View(viewModel);
        }        

        [HttpGet]
        public ActionResult IndividualContentIndex(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var post = DbContext.Posts.FirstOrDefault(p =>
            p.Id == id);

            if (post == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var model = new CreatePostViewModelForIndividualContent();
            model.Id = post.Id;
            model.Title = post.Title;
            model.Body = post.Body;
            model.MediaUrl = post.MediaUrl;
            model.Published = post.Published;
            model.DateCreated = post.CreatedDate;
            model.DateUpdated = post.DateUpdated;

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateAPost()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateAPost(CreatePostViewModel formData)
        {

            return SavePost(null, formData);
        }




        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditAPost(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }
            
            var post = DbContext.Posts.FirstOrDefault(
                p => p.Id == id);

            if (post == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var model = new CreatePostViewModel();
            model.Title = post.Title;
            model.Published = post.Published;
            model.MediaUrl = post.MediaUrl;           
            model.Body = post.Body;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditAPost(string id, CreatePostViewModel formData)
        {
            return SavePost(id, formData);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAPost(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }
            
            var post = DbContext.Posts.FirstOrDefault(p => p.Id == id);

            if(post == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }            

            DbContext.Posts.Remove(post);

            DbContext.SaveChanges();            

            return RedirectToAction(nameof(PostsController.Index));
        }




        private ActionResult SavePost (string id, CreatePostViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if(DbContext.Posts.Any(p => (id == null || p.Id != id) &&
            p.Title == formData.Title))
            {
                ModelState.AddModelError(nameof(CreatePostViewModel.Title),
                    "Post title should be unique");

                return View();
            }

            if(formData.Body == null || formData.Body == "")
            {
                ModelState.AddModelError(nameof(CreatePostViewModel.Body),
                    "Post body should not be empty");
                return View();
            }

            string fileExtension;

            if (formData.Media != null)
            {
                fileExtension = Path.GetExtension(formData.Media.FileName);

                if (!Constants.AllowedFileExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "File extension is not allowed.");
                    return View();
                }
            }

            Post post;

            if (id == null)
            {
                post = new Post();
                post.UserId = User.Identity.GetUserId();
                DbContext.Posts.Add(post);
            }
            else
            {
                post = DbContext.Posts.FirstOrDefault(
                    p => p.Id == id);

                if (post == null)
                {
                    return RedirectToAction(nameof(PostsController.Index));
                }

                post.DateUpdated = DateTime.Now;
            }

            post.Title = formData.Title;
            post.Body = formData.Body;
            post.Published = formData.Published;

            //Handling file upload
            if (formData.Media != null)
            {
                if (!Directory.Exists(Constants.MappedUploadFolder))
                {
                    Directory.CreateDirectory(Constants.MappedUploadFolder);
                }

                var fileName = formData.Media.FileName;
                var fullPathWithName = Constants.MappedUploadFolder + fileName;

                formData.Media.SaveAs(fullPathWithName);                                
                post.MediaUrl = Constants.UploadFolder + fileName;
            }
            else
            {
                post.MediaUrl = "no-img";
            }

            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostsController.Index));
        }
    }
}