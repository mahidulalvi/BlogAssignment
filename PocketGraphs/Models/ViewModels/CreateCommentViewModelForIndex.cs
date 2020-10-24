using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketGraphs.Models.ViewModels
{
    public class CreateCommentViewModelForIndex
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string User { get; set; }
    }
}