using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestAndroidClear.Tables;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

namespace TestAndroidClear.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        SqlConnection sqlConnection;
        Button TButton;
        Frame frame;
        FlexLayout flexLayout;
        StackLayout stackLayout;
        Button prbutton;
        Grid grid;
        CollectionView collectionView;
        public MainPage()
        {
            InitializeComponent();


            string srvrdbname = "Recipes";
            string srvrname = "192.168.2.30";
            string srvrusername = "vmukha";
            string srvrpasswoed = "123456";
            string sqlconn = $"Data Source={srvrname};Initial Catalog={srvrdbname}; User Id={srvrusername};Password={srvrpasswoed}";
            sqlConnection = new SqlConnection(sqlconn);
            sqlConnection.Open();

            Button button = new Button()
            {
                Text = "Continue",
            };
            stack.Children.Add(button);
            button.Clicked += Button_clickAsync;
        }

        private async void Button_clickAsync (object sender, EventArgs e)
        {
            try
            {
                List<Categories> categories = new List<Categories>();

                string querryString = "Select * from Recipes.dbo.Catigories";
                SqlCommand command = new SqlCommand(querryString, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

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
                for (int i = 0; i < categories.Count; i++)
                {
                    List<Products> products = new List<Products>();

                    string querryString1 = "Select * from Recipes.dbo." + categories[i].CategoryATBName;
                    SqlCommand command1 = new SqlCommand(querryString1, sqlConnection);
                    SqlDataReader reader1 = command1.ExecuteReader();

                    while (reader1.Read())
                    {
                        products.Add(new Products
                        {
                            ID = Convert.ToInt32(reader1["ID"]),
                            IngName = Convert.ToString(reader1["IngName"]),
                        });
                    }
                    reader1.Close();

                    frame = new Frame()
                    {
                         
                        HeightRequest = 140,
                        CornerRadius = 16,
                        Margin = new Thickness(0, 5, 0, 5),
                    };
                    stack.Children.Add(frame);
                    var gridLayout = new GridItemsLayout(orientation: ItemsLayoutOrientation.Vertical);
                    gridLayout.Span = 2;
                    collectionView = new CollectionView();
                    collectionView.ItemsSource = products;
                    collectionView.ItemsLayout = gridLayout;
                    collectionView.ItemTemplate = new DataTemplate(() =>
                    {
                        Button PRButton = new Button();

                        PRButton.SetBinding(Button.TextProperty, "IngName");
                        PRButton.FontSize = 12;

                        return PRButton;
                    });

                    string header = Convert.ToString(categories[i].CategoryTBName);

                    var headBTN = new Button
                    {
                        Text = header,
                        TextColor = Color.Black,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                        BackgroundColor = Color.White,
                    };
                    headBTN.Clicked += HeadBTNClick;

                    StackLayout q1 = new StackLayout()
                    {
                        Children =
                        {
                            headBTN,
                            collectionView
                        }
                    };


                    frame.Content = q1;

                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "Ok");
                throw;
            }
        }


        private void HeadBTNClick (object sender, EventArgs e)
        {
            if (frame.Height < 100)
            {
                frame.HeightRequest = Double.NaN;
            }
            else if (frame.Height > 140)
            {
                frame.HeightRequest = 140;
            }
        }

        // !
        private int CalculateSpanCount()
        {
            double screenWidth = Xamarin.Forms.Application.Current.MainPage.Width;
            double itemWidth = 100; // Ширина элемента (примерное значение)
            int minSpan = 1; // Минимальное количество элементов в строке

            // Вычисление количества элементов в строке (можно настроить логику по желанию)
            int spanCount = (int)(screenWidth / itemWidth); // Пример: один элемент в строке за 100 единиц ширины

            return Math.Max(spanCount, minSpan); // Возвращаем вычисленное значение, но не менее минимального
        }
    }
}