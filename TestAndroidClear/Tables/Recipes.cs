using System;
using System.Collections.Generic;
using System.Text;

namespace TestAndroidClear.Tables
{
    internal class Recipes
    {
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string WebURL { get; set; }
        public string TypeOfDish { get; set; }
        public int ReadyTyme { get; set; }
        public string Description { get; set; }
    }
}
