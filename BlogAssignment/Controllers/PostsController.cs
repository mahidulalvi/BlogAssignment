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
using HtmlAgilityPack;

namespace BlogAssignment.Controllers
{
    public class PostsController : Controller
    {         
        private ApplicationDbContext DbContext;
        public Random RandomNumber { get; } = new Random();

        public PostsController()
        {
            DbContext = new ApplicationDbContext();
        }

        
        public ActionResult Index()
        {
            var viewModel = DbContext.Posts
                .Where(p => p.Published == true)
                .AsEnumerable()
                .Select(
                post => new CreatePostViewModelForIndex
                {
                    Id = post.Id,
                    Slug = post.Slug,
                    Title = post.Title,
                    Body = CutContent(post.Body, 5)/*post.Body.Substring(0, 50)*/,
                    Published = post.Published,
                    DateCreated = post.CreatedDate,
                    DateUpdated = post.DateUpdated,
                    MediaUrl = post.MediaUrl,
                    User = post.User.UserName
                }).ToList();

            ViewBag.IsASearchResult = false;

            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult IndexForAdmin()
        {
            var viewModel = DbContext.Posts                
                .AsEnumerable()
                .Select(
                post =>  new CreatePostViewModelForIndex
                {
                    Id = post.Id,
                    Slug = post.Slug,
                    Title = post.Title,
                    Body = CutContent(post.Body, 5)/*post.Body.Substring(0, 50)*/,
                    Published = post.Published,
                    DateCreated = post.CreatedDate,
                    DateUpdated = post.DateUpdated,
                    MediaUrl = post.MediaUrl,
                    User = post.User.UserName
                }).ToList();

            ViewBag.IsASearchResult = false;

            return View("Index", viewModel);
        }


        [HttpGet]
        public /*PartialView*/ActionResult _PostsResultPartial(AdvancedSearchViewModel result)
        {
            var viewModel = DbContext.Posts
                .Where(p => p.Title.Contains(result.Body) || p.Slug.Contains(result.Body) || p.Body.Contains(result.Body))
                .Select(
                    post => new CreatePostViewModelForIndex
                    {
                        Id = post.Id,
                        Slug = post.Slug,
                        Title = post.Title,
                        Body = post.Body,/*post.Body.Substring(0, 50)*/
                        Published = post.Published,
                        DateCreated = post.CreatedDate,
                        DateUpdated = post.DateUpdated,
                        MediaUrl = post.MediaUrl,
                        User = post.User.UserName
                    }).ToList();

            ViewBag.IsASearchResult = true;

            return /*Partial*/View("Index", viewModel);
        }

        
        public ActionResult _CommentsListPartial(string Id)
        {
            var viewModel = DbContext.Comments
                .Where(p => p.PostId == Id)
                .Select(
                comment => new CreateCommentViewModelForIndex
                {
                    Id = comment.Id,
                    Body = comment.Body,
                    DateCreated = comment.DateCreated,
                    DateUpdated = comment.DateUpdated,
                    User = comment.User.UserName
                }).ToList();

            return View(viewModel);
        }

        private string CutContent(string content, int limitValue)
        {
            var htmlContent = new HtmlDocument();
            htmlContent.LoadHtml(content);
            var allHtmlNodes = htmlContent.DocumentNode.ChildNodes;
            string noHtmlContent = "";
            foreach (var node in allHtmlNodes)
            {
                noHtmlContent += node.InnerText;
                noHtmlContent += " ";
            }
            int totalCharsCount = noHtmlContent.Length;
            int showingCharsCount = Convert.ToInt32(totalCharsCount / 4);
            //Gives 25% of the total content
            return noHtmlContent.Substring(0, showingCharsCount);
        }

        [HttpGet]
        [Route("blog/{slug}")]
        public ActionResult IndividualContentIndex(string slug)
        {
            if (slug == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var post = DbContext.Posts.FirstOrDefault(p =>
            p.Slug == slug);

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


        [HttpPost]
        [Authorize]
        public ActionResult CreateAComment(CreateCommentViewModel formData)
        {
            return SaveComment(null, formData);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult _EditACommentPartial(string id)
        {
            if(id == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var comment = DbContext.Comments.FirstOrDefault(
                p => p.Id == id);

            if(comment == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var model = new CreateCommentViewModel();
            model.Body = comment.Body;
            model.PostId = comment.PostId;
            model.CommentId = comment.Id;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult _EditACommentPartial(string id, CreateCommentViewModel formData)
        {
            return SaveComment(id, formData);
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

            var CorrespondingComments = DbContext.Comments
                .Where(p => p.PostId == id).ToList();
            CorrespondingComments.Clear();

            DbContext.Posts.Remove(post);

            DbContext.SaveChanges();            

            return RedirectToAction(nameof(PostsController.Index));
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteAComment(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var comment = DbContext.Comments.FirstOrDefault(p => p.Id == id);

            if (comment == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }
            

            DbContext.Comments.Remove(comment);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostsController.Index));
        }


        private ActionResult SaveComment(string id, CreateCommentViewModel formData)
        {
            Comment comment;

            if(id == null)
            {
                comment = new Comment();
                comment.UserID = User.Identity.GetUserId();
                comment.PostId = formData.PostId;

                var CurrentUser = DbContext.Users.First(p => p.Id == comment.UserID);
                CurrentUser.AllOfMyComments.Add(comment);

                var CurrentPost = DbContext.Posts.First(p => p.Id == comment.PostId);
                CurrentPost.Comments.Add(comment);

                DbContext.Comments.Add(comment);
            }
            else
            {
                comment = DbContext.Comments.FirstOrDefault(p => p.Id == id);
                comment.DateUpdated = DateTime.Now;
                comment.UpdatedDates.Add(comment.DateCreated);
                comment.UpdatedDates.Add(comment.DateUpdated);

                if (formData.UpdateReason == null || formData.UpdateReason == "")
                {
                    ModelState.AddModelError(nameof(CreatePostViewModel.Body),
                        "Update reason should not be empty");
                    return View();
                }

                comment.UpdatedReasons.Add(formData.UpdateReason);
            }

            comment.Body = formData.Body;
            comment.EditHistory.Add(formData.Body);
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

                var CurrentUser = DbContext.Users.First(p => p.Id == post.UserId);
                CurrentUser.AllOfMyPosts.Add(post);

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
            post.Slug = SlugGenerator(formData.Title);
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

        private string SlugGenerator(string givenTitle)
        {
            var modifiedTitle = "";
            foreach (char eachCharacter in givenTitle)
            {
                if ((eachCharacter >= '0' && eachCharacter <= '9') || (eachCharacter >= 'A' && eachCharacter <= 'Z') || (eachCharacter >= 'a' && eachCharacter <= 'z') || eachCharacter == '/' || eachCharacter == '_')
                {
                    modifiedTitle += eachCharacter;
                }
            }

            string modifiedTitleCopy = modifiedTitle;
            string number = "-";
            number += RandomNumber.Next(1, 5000).ToString();

            modifiedTitleCopy += number;

            if(!DbContext.Posts.Any(p => p.Slug == modifiedTitleCopy))
            {
                return modifiedTitleCopy;
            }
            else
            {
                number = RandomNumber.Next(1, 5000).ToString();
                return modifiedTitle += number;
            }
        }
    }
}