using System.Collections.Generic;

namespace AutoService.Core.Models
{
    public class Category
    {
        public string Title { get; set; }

        public List<Service> Services { get; set; }
    }
}