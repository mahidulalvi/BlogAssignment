using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PocketGraphs.Models.ViewModels
{
    public class CreatePostViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Body is required")]
        [AllowHtml]
        public string Body { get; set; }

        [Required]
        public bool Published { get; set; }

        public HttpPostedFileBase Media { get; set; }

        public string MediaUrl { get; set; } = "no-img";
    }
}