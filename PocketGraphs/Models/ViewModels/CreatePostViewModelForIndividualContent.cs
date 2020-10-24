using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketGraphs.Models.ViewModels
{
    public class CreatePostViewModelForIndividualContent
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string MediaUrl { get; set; } = "no-img";
        public bool Published { get; set; }
    }
}