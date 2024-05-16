using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TestAndroidClear.Models;
using TestAndroidClear.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAndroidClear.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        SqlConnection sqlConnection;
        Frame frame;
        CollectionView collectionView;
        Button headBTN;
        
        public MainPage()
        {
            InitializeComponent();

            DatabaseConnection dbConnection = new DatabaseConnection();
            if (dbConnection.OpenConnection())
{
                // Получение объекта SqlConnection для выполнения запросов
                sqlConnection = dbConnection.GetConnection();
            }
            sqlConnection.Open();

            // Создание кнопки "Continue" и добавление ее в StackLayout
            Button button = new Button()
            {
                Text = "Continue",
            };
            stack.Children.Add(button);

            // Установка обработчика события Clicked для кнопки
            button.Clicked += Button_clickAsync;
        }

        private async void Button_clickAsync (object sender, EventArgs e)
        {
            try
            {
                // Создаем список категорий
                List<Categories> categories = new List<Categories>();

                // Запрос к базе данных для получения списка категорий
                string querryString = "Select * from Recipes.dbo.Categories";
                SqlCommand command = new SqlCommand(querryString, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                // Читаем результаты запроса и добавляем категории в список
                while (reader.Read())
                {
                    categories.Add(new Categories
                    {
                        CategoryID = Convert.ToInt32(reader["ID"]),
                        CategoryTBName = Convert.ToString(reader["TBName"]),
                        CategoryATBName = Convert.ToString(reader["ATBName"])
                    });
                }
                reader.Close();

                // Обработка каждой категории
                for (int i = 0; i < categories.Count; i++)
                {
                    // Создаем список продуктов для текущей категории
                    List<Products> products = new List<Products>();

                    // Запрос к базе данных для получения списка продуктов текущей категории
                    string querryString1 = "Select * from Recipes.dbo." + categories[i].CategoryATBName;
                    SqlCommand command1 = new SqlCommand(querryString1, sqlConnection);
                    SqlDataReader reader1 = command1.ExecuteReader();

                    // Читаем результаты запроса и добавляем продукты в список
                    while (reader1.Read())
                    {
                        products.Add(new Products
                        {
                            ID = Convert.ToInt32(reader1["ID"]),
                            IngName = Convert.ToString(reader1["IngName"]),
                        });
                    }
                    reader1.Close();

                    // Создаем новый Frame для текущей категории
                    frame = new Frame()
                    {
                        ClassId = "" + i,
                        HeightRequest = 140,
                        CornerRadius = 16,
                        Margin = new Thickness(0, 5, 0, 5),
                    };

                    // Добавляем Frame в StackLayout
                    stack.Children.Add(frame);

                    // Создаем макет для элементов CollectionView
                    var gridLayout = new GridItemsLayout(orientation: ItemsLayoutOrientation.Vertical);
                    gridLayout.Span = 2;

                    // Создаем и настраиваем CollectionView для отображения продуктов
                    collectionView = new CollectionView();
                    collectionView.ItemsSource = products;
                    collectionView.ItemsLayout = gridLayout;
                    collectionView.ItemTemplate = new DataTemplate(() =>
                    {
                        // Создаем кнопку для отображения продукта
                        Button PRButton = new Button();

                        // Привязываем текст кнопки к свойству IngName объекта Products
                        PRButton.SetBinding(Button.TextProperty, "IngName");
                        PRButton.SetBinding(Button.ClassIdProperty, "IngName");
                        PRButton.Clicked += PRButtonClicked;
                        PRButton.FontSize = 12;

                        return PRButton;
                    });

                    // Получаем название категории для использования в заголовке кнопки
                    string header = Convert.ToString(categories[i].CategoryTBName);

                    // Создаем кнопку с заголовком категории
                    headBTN = new Button
                    {
                        ClassId = "" + i,
                        Text = header,
                        TextColor = Color.Black,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                        BackgroundColor = Color.White,
                    };

                    // Устанавливаем обработчик события нажатия кнопки
                    headBTN.Clicked += HeadBTNClick;

                    // Создаем StackLayout для размещения кнопки и CollectionView
                    StackLayout q1 = new StackLayout()
                    {
                        Children =
                        {
                            headBTN,
                            collectionView
                        }
                    };

                    // Устанавливаем содержимое Frame
                    frame.Content = q1;
                }
            }
            catch (Exception ex)
            {
                // В случае исключения выводим сообщение об ошибке
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "Ok");
                throw;
            }
        }

        private async void PRButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            // Получаем ClassId кнопки, чтобы идентифицировать соответствующий Frame
            var productName = button.Text;
            if (GlobalProductList.Products.Contains(productName))
            {
                GlobalProductList.Products.Remove(productName);
                button.BackgroundColor = Color.Cyan;
            }
            else
            {
                GlobalProductList.Products.Add(productName);
                button.BackgroundColor = Color.Accent;
            }
        }

        private async void HeadBTNClick(object sender, EventArgs e)
        {
            // Получаем ссылку на кнопку, которая была нажата
            var button = (Button)sender;

            // Получаем ClassId кнопки, чтобы идентифицировать соответствующий Frame
            var btnclass = button.ClassId;

            // Находим Frame, соответствующий нажатой кнопке
            var fr = stack.Children.FirstOrDefault(child => child is Frame && ((Frame)child).ClassId == btnclass) as Frame;

            // Проверяем высоту Frame и выполняем соответствующее действие
            if (fr.HeightRequest > 181)
            {
                // Если высота больше 181, вызываем метод сворачивания Frame
                fr.HeightRequest = 600;
            }
            else if (fr.HeightRequest <= 180)
            {
                // Если высота меньше или равна 180, вызываем метод разворачивания Frame
                fr.HeightRequest = 180;
            }
        }
    }
}