using DeviceManagement.Devices;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Internals.Profile;

namespace DeviceManagement
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryPageIPS_1 : ContentPage
	{

		private string filePath;
		private IFolder rootFolder;
		private IFolder folder;
		private IFile file;
        private Dictionary<int, string> text = new Dictionary<int, string>();
        public HistoryPageIPS_1 ()
		{
			InitializeComponent();
			filePath = "IPS_1.txt";
			string name = "IPS_1";
			Title = "История " + DeviceNames.DeviceNamesList[name];
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoaderIndicator.IsRunning = true;
            ResultsStack.IsVisible = false;

            await Task.Delay(500);
            await LoadData();

            LoaderIndicator.IsRunning = false;
            ResultsStack.IsVisible = true;
        }

        private async Task LoadData()
		{
            rootFolder = PCLStorage.FileSystem.Current.LocalStorage;
            folder = await rootFolder.CreateFolderAsync("SaveFolder", CreationCollisionOption.OpenIfExists);
            file = await folder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
            string data = await file.ReadAllTextAsync();
			var str = data.Split('\n');
            int ind = 0;
            string dateStr = DateTime.Now.ToShortDateString();
            DateTime date = DateTime.ParseExact(dateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            foreach (var s in str)
			{
                var ss = s.Split('_');
				if (ss.Count() == 7)
				{
                    DateTime dateOld = DateTime.ParseExact(ss[0], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    TimeSpan difference = date - dateOld;
                    if (difference.Days < 31 && text.Count() < 100)
                    {
                        text[ind] = s;
                        //ResultsGrid.RowDefinitions.Add(new RowDefinition() { Height = 50 });
                        //ResultsGrid.RowDefinitions.Add(new RowDefinition() { Height = 50 });
                        Grid grid = new Grid() { };
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = 1 });
                        grid.RowDefinitions.Add(new RowDefinition() { Height = 30 });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 1 });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 1 });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 1 });
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                        // Label с Датой
                        Label label1 = new Label()
                        {
                            Text = ss[0],
                            TextColor = Color.Black,
                            FontSize = 13,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                        };
                        //ResultsGrid.Children.Add(label1, 0, rowIndex);
                        grid.Children.Add(label1, 0, 0);
                        Grid.SetColumnSpan(label1, 3);
                        // Labels с Временем и Названием
                        for (int i = 1; i < 3; i++)
                        {
                            Label label = new Label()
                            {
                                Text = ss[i],
                                TextColor = Color.Black,
                                FontSize = 13,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                            };
                            //ResultsGrid.Children.Add(label, i+1, rowIndex);
                            grid.Children.Add(label, 2 * (i + 1), 0);
                        }
                        // BoxViews для разметок
                        BoxView box4 = new BoxView()
                        {
                            HeightRequest = 1,
                            Color = Color.Black
                        };
                        grid.Children.Add(box4, 3, 0);
                        BoxView box5 = new BoxView()
                        {
                            HeightRequest = 1,
                            Color = Color.Black
                        };
                        grid.Children.Add(box5, 5, 0);
                        BoxView box = new BoxView()
                        {
                            WidthRequest = 350,
                            Color = Color.Black
                        };
                        grid.Children.Add(box, 0, 1);
                        Grid.SetColumnSpan(box, 7);
                        BoxView box1 = new BoxView()
                        {
                            HeightRequest = 1,
                            Color = Color.Black
                        };
                        grid.Children.Add(box1, 1, 2);
                        BoxView box2 = new BoxView()
                        {
                            HeightRequest = 1,
                            Color = Color.Black
                        };
                        grid.Children.Add(box2, 3, 2);
                        BoxView box3 = new BoxView()
                        {
                            HeightRequest = 1,
                            Color = Color.Black
                        };
                        grid.Children.Add(box3, 5, 2);
                        // Labels с результатами тестирований
                        for (int i = 3; i < ss.Length; i++)
                        {
                            Label label = new Label()
                            {
                                Text = ss[i],
                                TextColor = Color.Black,
                                FontSize = 13,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                            };
                            //ResultsGrid.Children.Add(label, i-3, rowIndex);
                            grid.Children.Add(label, 2 * (i - 3), 2);
                        }
                        // Основной Frame с данными
                        Frame frame = new Frame()
                        {
                            BackgroundColor = Color.White,
                            CornerRadius = 30,
                            HasShadow = false,
                        };
                        frame.Content = grid;
                        // SwipeItems

                        // Swipeview для удаления записи
                        var swipeView = new SwipeView();
                        var swipeItems = new SwipeItems();
                        var swipeItemView = new SwipeItemView();

                        // Frame для иконки удаления
                        Frame frame1 = new Frame()
                        {
                            CornerRadius = 30,
                            BackgroundColor = Color.White,
                            HasShadow = false,
                            WidthRequest = 50,
                            Margin = new Thickness(10, 0, 0, 0),
                        };
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        int t = text.Count() - 1;
                        tapGestureRecognizer.Tapped += (sender, ev) => {
                            ImageButtonClicked(sender, ev, swipeView, t);
                        };
                        frame1.GestureRecognizers.Add(tapGestureRecognizer);
                        StackLayout stackLayout = new StackLayout()
                        {
                            VerticalOptions = LayoutOptions.Center,
                        };
                        Image image = new Image()
                        {
                            Source = "delete.png",
                            HeightRequest = 30,
                            WidthRequest = 30,
                        };
                        stackLayout.Children.Add(image);
                        frame1.Content = stackLayout;

                        swipeItemView.Content = frame1;
                        swipeItems.Add(swipeItemView);
                        swipeView.RightItems = swipeItems;
                        swipeView.Content = frame;

                        ResultsStack.Children.Add(swipeView);
                        ind++;
                    }
                }
            }
            string newStr = "";
            foreach (var t in text.Keys)
            {
                newStr += text[t] + Environment.NewLine;
            }
            await file.WriteAllTextAsync(newStr);
        }

        private async void ImageButtonClicked(object sender, EventArgs e, SwipeView swipeView, int ind)
        {
            Frame frame = sender as Frame;
            frame.BackgroundColor = Color.FromRgb(204, 213, 244);
            await Task.Delay(250);
            text[ind] = "";
            string newStr = "";
            foreach (var t in text.Keys)
            {
                newStr += text[t] + Environment.NewLine;
            }
            await file.WriteAllTextAsync(newStr);
            ResultsStack.Children.Remove(swipeView);
        }

        private async void ResultsClearButton_Clicked(object sender, EventArgs e)
        {
			await file.DeleteAsync();
            ResultsStack.Children.Clear();
            ResultsClearButton.IsEnabled = false;
        }
    }
}