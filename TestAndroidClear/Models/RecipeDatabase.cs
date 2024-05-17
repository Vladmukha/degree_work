using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TestAndroidClear.Models
{
    public class RecipeDatabase
    {
        readonly SQLiteAsyncConnection database;

        public RecipeDatabase()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Liked.db3");
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<SavedRecipes>().Wait();
        }

        public async Task<bool> IsRecipeSavedAsync(SavedRecipes recipe)
        {
            try
            {
                var result = await database.Table<SavedRecipes>().Where(r => r.RecipeID == recipe.RecipeID).FirstOrDefaultAsync();
                return result != null;
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если они возникнут
                Console.WriteLine($"Error checking if recipe is saved: {ex.Message}");
                return false;
            }
        }

        public async Task SaveRecipeAsync(SavedRecipes recipe)
        {
            try
            {
                await database.InsertAsync(recipe);
                Console.WriteLine("Recipe saved successfully.");
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если они возникнут
                Console.WriteLine($"Error saving recipe: {ex.Message}");
            }
        }

        public async Task DeleteRecipeAsync(SavedRecipes recipe)
        {
            try
            {
                await database.DeleteAsync(recipe);
                Console.WriteLine("Recipe deleted successfully.");
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если они возникнут
                Console.WriteLine($"Error deleting recipe: {ex.Message}");
            }
        }

        // Метод для получения сохраненных рецептов
        public async Task<List<SavedRecipes>> GetSavedRecipesAsync()
        {
            try
            {
                return await database.Table<SavedRecipes>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving saved recipes: {ex.Message}");
                return new List<SavedRecipes>();
            }
        }

        // Новый метод для удаления всех записей из таблицы Recipes
        public async Task DeleteAllRecipesAsync()
        {
            try
            {
                await database.DeleteAllAsync<SavedRecipes>();
                Console.WriteLine("All recipes deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting all recipes: {ex.Message}");
            }
        }
    }
}
