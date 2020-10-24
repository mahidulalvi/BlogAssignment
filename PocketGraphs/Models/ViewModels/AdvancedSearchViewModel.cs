using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocketGraphs.Models.ViewModels
{
    public class AdvancedSearchViewModel
    {
        public string Body { get; set; }
        public List<string> CategoryFilter { get; set; }
        
        public AdvancedSearchViewModel()
        {
            CategoryFilter = new List<string>();
        }
    }
}