﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketGraphs.Models.ViewModels
{
    public class CreatePostViewModelForIndex
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string User { get; set; }
        public string MediaUrl { get; set; } = "no-img";
    }
}