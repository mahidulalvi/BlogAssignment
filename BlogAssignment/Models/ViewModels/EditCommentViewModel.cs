using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BlogAssignment.Models.ViewModels
{
    public class EditCommentViewModel
    {
        [Required(ErrorMessage = "Comment Body is required")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Please provide a reason for updating the comment")]
        public string UpdateReason { get; set; }
    }
}