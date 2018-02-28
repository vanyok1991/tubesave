using System.Collections.Generic;

namespace AutoService.Core.Models
{
    public class Service
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public List<string> Phones { get; set; }

        public string ImageUrl { get; set; }
    }
}