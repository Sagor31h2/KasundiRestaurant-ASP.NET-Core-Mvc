using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KasundiRestaurant.Models.ViewModels
{
    public class CategoryAndSubCategoryViewModel
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<SubCategory> SubCategoryList { get; set; }
    }
}
