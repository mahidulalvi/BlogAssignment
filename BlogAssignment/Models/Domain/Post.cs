using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using BlogAssignment.Models;
//using Microsoft.AspNet.Identity;

namespace BlogAssignment.Models.Domain
{
    public class Post
    {
        public string Id {get; set;}
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DateUpdated { get; set; }        
        public bool Published { get; set; }
        public string MediaUrl { get; set; } = "no-img";

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public Post()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            DateUpdated = DateTime.Now;
        }
    }
}