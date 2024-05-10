using System;
using System.Collections.Generic;
using System.Text;

namespace TestAndroidClear.Models
{
    internal class Recipes
    {
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string URL { get; set; }
        public int MaxReadyTime { get; set; }
        public byte[] Image { get; set; }
    }
}
