using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestAndroidClear.Models
{
    public class SavedRecipes
    {
        [PrimaryKey, AutoIncrement]
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string URL { get; set; }
        public int MaxReadyTime { get; set; }
        public byte[] Image { get; set; }
    }
}
