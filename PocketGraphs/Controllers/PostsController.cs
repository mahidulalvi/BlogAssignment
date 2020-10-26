using System;
using System.Collections.Generic;
using PocketGraphs.Models.Domain;
using PocketGraphs.Models.ViewModels;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PocketGraphs.Models;
using System.IO;
using HtmlAgilityPack;

namespace PocketGraphs.Controllers
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
                post => new CreatePostViewModelForIndex
                {
                    Id = post.Id,
                    Slug = post.Slug,
                    Title = post.Title,
                    Body = CutContent(post.Body, 5),
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
        public ActionResult _PostsResultPartial(AdvancedSearchViewModel result)
        {
            var viewModel = DbContext.Posts
                .Where(p => p.Title.Contains(result.Body) || p.Slug.Contains(result.Body) || p.Body.Contains(result.Body))
                .Select(
                    post => new CreatePostViewModelForIndex
                    {
                        Id = post.Id,
                        Slug = post.Slug,
                        Title = post.Title,
                        Body = post.Body,
                        Published = post.Published,
                        DateCreated = post.CreatedDate,
                        DateUpdated = post.DateUpdated,
                        MediaUrl = post.MediaUrl,
                        User = post.User.UserName
                    }).ToList();

            ViewBag.IsASearchResult = true;

            return View("Index", viewModel);
        }

        [ChildActionOnly]
        public PartialViewResult _CommentsListPartial(string Id)
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

            return PartialView(viewModel);
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
        [Route("pocket/{slug}")]
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



        //Backend Validation Start
            if ((TempData["SelectedCommentId"] != null && TempData["SelectedCommentEditTimeError"] != null) || TempData["SelectedCommentCreateTimeError"] != null)
            {
                if (TempData["SelectedCommentId"] != null)
                {
                    ViewBag.SelectedCommentId = TempData["SelectedCommentId"].ToString();
                    TempData["SelectedCommentId"] = TempData["SelectedCommentId"];
                }
                ViewBag.SelectedCommentEditTimeError = TempData["SelectedCommentEditTimeError"];

                if (TempData["ErrorOnBody"] != null && TempData["ErrorOnBody"].ToString() == "true" && TempData["SelectedCommentCreateTimeError"] == null)
                {
                    TempData["BodyErrorMessage"] = "Body should not be empty";
                }
                else if (TempData["SelectedCommentCreateTimeError"] != null)
                {
                    TempData["BodyErrorMessageForCreateComment"] = "Body should not be empty";
                }

                if (TempData["ErrorOnUpdateReason"] != null && TempData["ErrorOnUpdateReason"].ToString() == "true")
                {
                    TempData["UpdateReasonErrorMessage"] = "Update reason should not be empty";
                }
            }
        //Backend Validation End

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
        public ActionResult _CreateCommentPartial(CreateCommentViewModel formData)
        {
            return SaveComment(formData);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult _EditACommentPartial(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var comment = DbContext.Comments.FirstOrDefault(
                p => p.Id == id);

            if (comment == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            var model = new EditCommentViewModel();
            model.Body = comment.Body;
            model.PostId = comment.PostId;
            model.CommentId = comment.Id;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult _EditACommentPartial(string id, EditCommentViewModel formData)
        {
            return SaveCommentForEdit(id, formData);
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

            if (post == null)
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

            var post = DbContext.Posts.FirstOrDefault(p => p.Id == comment.PostId);

            if (post == null)
            {
                return RedirectToAction(nameof(PostsController.Index));
            }

            DbContext.Comments.Remove(comment);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostsController.IndividualContentIndex), new { Slug = post.Slug });
        }


        private ActionResult SaveComment(CreateCommentViewModel formData)
        {

            //if (!ModelState.IsValid)
            //{
            //    return PartialView(formData);
            //}

            Comment comment;

            comment = new Comment();
            comment.UserID = User.Identity.GetUserId();
            comment.PostId = formData.PostId;

            var CurrentUser = DbContext.Users.First(p => p.Id == comment.UserID);
            CurrentUser.AllOfMyComments.Add(comment);

            var CurrentPost = DbContext.Posts.FirstOrDefault(p => p.Id == comment.PostId);
            CurrentPost.Comments.Add(comment);

            DbContext.Comments.Add(comment);

        //Backend Validation Start
            if (string.IsNullOrEmpty(formData.Body))
            {
                //var postOfCreatingComment = DbContext.Posts.FirstOrDefault(p => p.Id == comment.PostId);

                if (CurrentPost == null)
                {
                    return RedirectToAction(nameof(PostsController.Index));
                }

                if (string.IsNullOrEmpty(formData.Body))
                {
                    TempData["ErrorOnBody"] = "true";
                }
                else
                {
                    TempData["ErrorOnBody"] = "false";
                }


                TempData["SelectedCommentId"] = comment.Id;
                TempData["SelectedCommentEditTimeError"] = false;
                TempData["SelectedCommentCreateTimeError"] = true;


                return RedirectToAction("IndividualContentIndex", "Posts", new { Slug = CurrentPost.Slug });

            }
        //Backend Validation End

            comment.Body = formData.Body;

            comment.EditHistory.Add(formData.Body);            
            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostsController.IndividualContentIndex), new { Slug = CurrentPost.Slug });
        }


        private ActionResult SaveCommentForEdit(string id, EditCommentViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            Comment comment;

            comment = DbContext.Comments.FirstOrDefault(p => p.Id == id);
            comment.DateUpdated = DateTime.Now;
            comment.UpdatedDates.Add(comment.DateCreated);
            comment.UpdatedDates.Add(comment.DateUpdated);

            var editingPost = DbContext.Posts.FirstOrDefault(p => p.Id == comment.PostId);

        //Backend Validation Start
            if (string.IsNullOrEmpty(formData.Body) || string.IsNullOrEmpty(formData.UpdateReason))
            {

                if (editingPost == null)
                {
                    return RedirectToAction(nameof(PostsController.Index));
                }

                if (string.IsNullOrEmpty(formData.Body))
                {
                    TempData["ErrorOnBody"] = "true";
                }
                else
                {
                    TempData["ErrorOnBody"] = "false";
                }


                if (string.IsNullOrEmpty(formData.UpdateReason))
                {
                    TempData["ErrorOnUpdateReason"] = "true";
                }
                else
                {
                    TempData["ErrorOnUpdateReason"] = "false";
                }


                TempData["SelectedCommentId"] = comment.Id;
                TempData["SelectedCommentEditTimeError"] = true;
                return RedirectToAction("IndividualContentIndex", "Posts", new { Slug = editingPost.Slug });
            }
        //Backend Validation End


            comment.UpdatedReasons.Add(formData.UpdateReason);

            comment.Body = formData.Body;
            comment.EditHistory.Add(formData.Body);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostsController.IndividualContentIndex), new { Slug = editingPost.Slug });
        }


        private ActionResult SavePost(string id, CreatePostViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (DbContext.Posts.Any(p => (id == null || p.Id != id) &&
             p.Title == formData.Title))
            {
                ModelState.AddModelError(nameof(CreatePostViewModel.Title),
                    "Post title should be unique");

                return View();
            }

            if (formData.Body == null || formData.Body == "")
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

            if (!DbContext.Posts.Any(p => p.Slug == modifiedTitleCopy))
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