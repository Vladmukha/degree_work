using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TestAndroidClear.Models;
using TestAndroidClear.Converters;
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

            OnAppearing();
        }

        // Метод OnAppearing() вызывается при появлении страницы на экране.
        // Вызывает метод Create_elements() для создания элементов.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Create_elements();
        }

        private async void Create_elements()
        {
            try
            {
                stack.Children.Clear();
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
                for (int i = 0; i < 15; i++)
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
                        var product = new Products
                        {
                            ID = Convert.ToInt32(reader1["ID"]),
                            IngName = Convert.ToString(reader1["IngName"]),
                            IsSelected = GlobalProductList.Products.Contains(Convert.ToString(reader1["IngName"]))
                        };
                        products.Add(product);
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
                    var gridLayout = new GridItemsLayout(orientation: ItemsLayoutOrientation.Vertical)
                    {
                        Span = 2
                    };
                    RelativeLayout relativeLayout = new RelativeLayout();
                    // Создаем и настраиваем CollectionView для отображения продуктов
                    collectionView = new CollectionView
                    {
                        ItemsSource = products,
                        ItemsLayout = gridLayout,
                        ItemTemplate = new DataTemplate(() =>
                        {
                            // Создаем кнопку для отображения продукта
                            Button PRButton = new Button();

                            // Привязываем текст кнопки к свойству IngName объекта Products
                            PRButton.SetBinding(Button.TextProperty, "IngName");
                            PRButton.SetBinding(Button.ClassIdProperty, "IngName");
                            PRButton.SetBinding(Button.BackgroundColorProperty, new Binding("IsSelected", converter: new BoolToColorConverter()));
                            PRButton.Clicked += PRButtonClicked;
                            PRButton.FontSize = 12;
                            return PRButton;
                        })
                    };

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

        private async void HeadBTNClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var layout = (StackLayout)button.Parent;
            var collectionView = (CollectionView)layout.Children[1];

            // Проверяем текущее состояние высоты Frame
            var frame = (Frame)layout.Parent;
            bool isCollapsed = frame.HeightRequest == 140;

            // Задаем начальную и конечную высоту для анимации
            double startHeight = frame.Height;

            double endHeight = isCollapsed ? CalculateExpandedHeight(collectionView) : 140;

            // Задаем продолжительность анимации
            uint animationDuration = 250;

            // Создаем анимацию
            var animation = new Animation(v => frame.HeightRequest = v, startHeight, endHeight);
            animation.Commit(this, "ExpandCollapseAnimation", 16, animationDuration, Easing.Linear);
        }

        // Метод для вычисления высоты CollectionView для развернутого состояния
        private double CalculateExpandedHeight(CollectionView collectionView)
        {
            // Определяем количество элементов
            int itemCount = 0;
            if (collectionView.ItemsSource != null)
            {
                itemCount = collectionView.ItemsSource.Cast<object>().Count();
            }

            // Определяем количество столбцов (предположим, что у вас есть свойство Span в ItemsLayout)
            int columnCount = 2;

            // Рассчитываем количество строк
            int rowCount = 10;

            // Высота одной строки (можете использовать значения по умолчанию или получить его из элемента)
            double rowHeight = 48; // Примерное значение, можно настроить

            // Вычисляем высоту содержимого
            double contentHeight = rowCount * rowHeight;

            // Добавляем высоту заголовка категории и некоторый запас
            double expandedHeight = contentHeight + 100; // Например, добавляем 50 пикселей для заголовка и отступов

            return expandedHeight;
        }

        private async void PRButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var product = (Products)button.BindingContext;

            if (product != null)
            {
                product.IsSelected = !product.IsSelected;

                // Если продукт выбран, добавляем его в глобальный список продуктов.
                if (product.IsSelected)
                {
                    if (!GlobalProductList.Products.Contains(product.IngName))
                    {
                        GlobalProductList.Products.Add(product.IngName);
                    }
                }
                // Если продукт не выбран, удаляем его из глобального списка продуктов.
                else
                {
                    GlobalProductList.Products.Remove(product.IngName);
                }

                // Обновляем цвета кнопок.
                UpdateButtonColors(product.IngName);
            }
        }
        private void UpdateButtonColors(string productName)
        {
            // Итерируем по дочерним элементам StackLayout.
            foreach (var product in stack.Children.OfType<Frame>()
                // Находим вложенные CollectionView.
                .SelectMany(frame => (frame.Content as StackLayout)?.Children.OfType<CollectionView>() ?? Enumerable.Empty<CollectionView>())
                // Извлекаем продукты из ItemsSource CollectionView.
                .SelectMany(view => view.ItemsSource.OfType<Products>())
                // Отбираем продукты с именем, соответствующим переданному имени.
                .Where(p => p.IngName == productName))
            {
                // Устанавливаем состояние выбора продукта на основе его наличия в глобальном списке продуктов.
                product.IsSelected = GlobalProductList.Products.Contains(productName);
            }
        }
    }
}