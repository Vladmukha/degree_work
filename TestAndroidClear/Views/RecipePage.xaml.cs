using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using TestAndroidClear.Models;
using TestAndroidClear.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAndroidClear.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RecipePage : ContentPage
	{

        //Вспомогательные
        SqlConnection sqlConnection;

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
        public RecipePage ()
		{
			InitializeComponent ();
            DatabaseConnection dbConnection = new DatabaseConnection();
            if (dbConnection.OpenConnection())
            {
                // Получение объекта SqlConnection для выполнения запросов
                sqlConnection = dbConnection.GetConnection();
            }
            sqlConnection.Open();
            /*
            // Создание кнопки "Continue" и добавление ее в StackLayout
            Button button = new Button()
            {
                Text = "Continue",
            };
            Re.Children.Add(button);

            // Установка обработчика события Clicked для кнопки
            button.Clicked += Button_clickAsync;*/
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadRecipes();
        }

        //private async void Button_clickAsync(object sender, EventArgs e)
        private async void LoadRecipes()
        {
            try
            {
                Re.Children.Clear();
                // Создаем список категорий
                List<Recipes> recipe = new List<Recipes>();
                List<string> product = GlobalProductList.Products;
                // Запрос к базе данных для получения списка категорий
                string querryString = "SELECT r.ID, r.Title, r.Description, r.Product, r.Url, r.MaxReadyTime, r.Image " +
                    "FROM Recipe r where ";
                for (int i = 0; i < product.Count; i++)
                {
                    if (i + 1 != product.Count)
                    {
                        querryString += "r.Product like '%' + '" + product[i] + "' + '%' or ";
                    }
                    else
                    {
                        querryString += "r.Product like '%' + '" + product[i] + "' + '%'";
                    }
                }
                SqlCommand command = new SqlCommand(querryString, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                // Читаем результаты запроса и добавляем категории в список
                while (reader.Read())
                {
                    recipe.Add(new Recipes
                    {
                        RecipeID = Convert.ToInt32(reader["ID"]),
                        Title = Convert.ToString(reader["Title"]),
                        Product = Convert.ToString(reader["Product"]),
                        MaxReadyTime = Convert.ToInt32(reader["MaxReadyTime"]),
                        Image = (byte[])reader["Image"]
                    });
                }
                reader.Close();

                // Обработка каждого рецепта
                for (int i = 0; i < recipe.Count; i++)
                {
                    Heart = new ImageButton() 
                    {
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 5, 0),
                        HeightRequest = 26,
                        WidthRequest = 26,
                        BackgroundColor = Color.Red
                    };
                    RTitle = new Label() 
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label)),
                        TextColor = Color.Black,
                    };
                    RTime = new Label() 
                    {
                        FontSize = 12
                    };
                    RProd = new Label() 
                    { 
                        LineBreakMode = LineBreakMode.TailTruncation,
                        MaxLines = 1,
                        FontSize = 12
                    };
                    RImg = new Image() 
                    {
                        Aspect = Aspect.AspectFill
                    };
                    Recipes r = recipe[i];
                    if (r.Image != null && r.Image.Length > 0)
                    {
                        ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(r.Image));
                        RImg.Source = imageSource;
                    }
                    RProd.Text = Convert.ToString(r.Product);
                    RTime.Text = Convert.ToString(r.MaxReadyTime);
                    RTitle.Text = Convert.ToString(r.Title);
                    //RTitle.SetBinding(Label.TextProperty, r.Title);
                    //RProd.SetBinding(Label.TextProperty, r.Product);
                    //RTime.SetBinding(Label.TextProperty, Convert.ToString(r.MaxReadyTime));

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
                        Padding = new Thickness(12,10),
                        CornerRadius = 16,
                        Content = RSEntry
                    };
                    Re.Children.Add(RFrame);
                }
            }
            catch (Exception ex)
            {
                // В случае исключения выводим сообщение об ошибке
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "Ok");
                throw;
            } 
        }
    }
}