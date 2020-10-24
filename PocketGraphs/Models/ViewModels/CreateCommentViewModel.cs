using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
//using System.Web.Mvc;

namespace PocketGraphs.Models.ViewModels
{
    public class CreateCommentViewModel
    {
        [Required(ErrorMessage = "Comment Body is required")]
        public string Body { get; set; }

        public string UpdateReason { get; set; }

        public string PostId { get; set; }

        public string CommentId { get; set; }
    }
}