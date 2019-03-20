using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogAssignment.Models.Domain
{
    public class Comment
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public List<string> UpdatedReasons { get; set; }
        public List<DateTime> UpdatedDates { get; set; }
        public List<string> EditHistory { get; set; }

        public virtual Post Post { get; set; }
        public string PostId { get; set; }          
        
        public virtual ApplicationUser User { get; set; }
        public string UserID { get; set; }

        public Comment()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateUpdated = DateTime.Now;
            UpdatedReasons = new List<string>();
            UpdatedDates = new List<DateTime>();
            EditHistory = new List<string>();
        }
    }
}