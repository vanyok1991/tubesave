using System.Collections.Generic;

namespace AutoService.Core.Models
{
    public class Category
    {
        //Suspension,
        //CarWash,
        //TireFitting,
        //CarElectrician,
        //Engine,
        //Paining,
        //Exhaust
        public string Title { get; set; }

        public List<Service> Services { get; set; }
    }
}