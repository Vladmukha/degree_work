using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TestAndroidClear.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace TestAndroidClear.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RLikePage : ContentPage
    {
        //Вспомогательные
        RecipeDatabase recipeDatabase = new RecipeDatabase();

        //Контейнеры 
        StackLayout RSETitleHeart;
        StackLayout RSEEntry;
        Frame RFImage;
        StackLayout RSEntry;
        Frame RFrame;

        //Контейнеры инфо-и
        ImageButton Heart;
        Label RTitle;
        Label RTime;
        Label RProd;
        Image RImg;

        public RLikePage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSaved();
        }

        private async void LoadSaved()
        {
            try 
            { 
                // Получаем сохраненные рецепты из базы данных
                List<SavedRecipes> savedRecipes = await recipeDatabase.GetSavedRecipesAsync();
                // Очищаем существующие элементы интерфейса
                Rl.Children.Clear();
                if (savedRecipes.Count == 0)
                {
                    Rl.VerticalOptions = LayoutOptions.CenterAndExpand;
                    Label label = new Label()
                    {
                        Text = "У вас нет сохраненных рецептов!",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 24,
                        TextColor = Color.Black,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                    RFrame = new Frame()
                    {
                        HeightRequest = 100,
                        Padding = new Thickness(12, 10),
                        CornerRadius = 16,
                        Content = label,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };
                    Rl.Children.Add(RFrame);
                }
                else
                {
                    // Обрабатываем каждый сохраненный рецепт
                    foreach (var recipe in savedRecipes)
                    {
                        Heart = new ImageButton()
                        {
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                            VerticalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, 0, 5, 0),
                            HeightRequest = 26,
                            WidthRequest = 26,
                            BackgroundColor = Color.Red,
                            BindingContext = recipe // Привязываем контекст кнопки к рецепту
                        };
                        Heart.BindingContext = recipe;
                        Heart.Clicked += heart_BtnClick;

                        RTitle = new Label()
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                            TextColor = Color.Black,
                            Text = recipe.Title
                        };

                        RTime = new Label()
                        {
                            FontSize = 12,
                            Text = recipe.MaxReadyTime.ToString()
                        };

                        RProd = new Label()
                        {
                            LineBreakMode = LineBreakMode.TailTruncation,
                            MaxLines = 1,
                            FontSize = 12,
                            Text = recipe.Product
                        };

                        RImg = new Image()
                        {
                            Aspect = Aspect.AspectFill
                        };

                        if (recipe.Image != null && recipe.Image.Length > 0)
                        {
                            ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(recipe.Image));
                            RImg.Source = imageSource;
                        }

                        RSETitleHeart = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                RTitle,
                                Heart
                            }
                        };

                        RSEEntry = new StackLayout()
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Children =
                            {
                                RSETitleHeart,
                                RProd,
                                RTime
                            }
                        };

                        RFImage = new Frame()
                        {
                            CornerRadius = 8,
                            Padding = 0,
                            WidthRequest = 160,
                            HeightRequest = 95,
                            VerticalOptions = LayoutOptions.Center,
                            Content = RImg
                        };

                        RSEntry = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                RFImage,
                                RSEEntry
                            }
                        };

                        RFrame = new Frame()
                        {
                            HeightRequest = 100,
                            Padding = new Thickness(12, 10),
                            CornerRadius = 16,
                            Content = RSEntry,
                            VerticalOptions = LayoutOptions.Start
                        };
                        Rl.VerticalOptions = LayoutOptions.Start;
                        Rl.Children.Add(RFrame);
                    }
                }
            }
            catch (Exception ex)
                {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async void heart_BtnClick(object sender, EventArgs e)
        {
            try
            {
                // Получаем рецепт, соответствующий кнопке, по ее контексту
                var button = sender as Xamarin.Forms.ImageButton;
                var savedRecipe = button.BindingContext as SavedRecipes;

                // Проверяем, сохранен ли рецепт
                bool isSaved = await recipeDatabase.IsRecipeSavedAsync(savedRecipe);

                if (isSaved)
                {
                    // Рецепт уже сохранен, удаляем его
                    await recipeDatabase.DeleteRecipeAsync(savedRecipe);
                }
                else
                {
                    // Рецепт не сохранен, сохраняем его
                    await recipeDatabase.SaveRecipeAsync(savedRecipe);
                }
                OnAppearing();
            }
            catch (Exception ex)
            {
                // В случае ошибки выводим сообщение об ошибке
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

    }
}
