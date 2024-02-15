using DeviceManagement.Devices;
using PCLStorage;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DeviceManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {

        private IFolder rootFolder;
        private IFolder folder;
        private List<string> deviceList = new List<string>();
        public HistoryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //await Task.Delay(500);
            await LoadData();
        }

        private async Task LoadData()
        {
            rootFolder = FileSystem.Current.LocalStorage;
            folder = await rootFolder.CreateFolderAsync("SaveFolder", CreationCollisionOption.OpenIfExists);
            var files = await folder.GetFilesAsync();
            deviceList.Clear();
            foreach ( var file in files)
            {
                string name = file.Name.Replace(".txt", "");
                deviceList.Add(name);
            }
            Grid.Children.Clear();
            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
            Frame.IsVisible = false;
            CreateGrid();
            Frame.IsVisible = true;
        }

        private void CreateGrid()
        {
            int rowIndex = -1;
            int columnIndex = 1;

            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 172.5 });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 172.5 });

            foreach (var device in deviceList)
            {
                if (columnIndex == 0)
                {
                    columnIndex++;
                }
                else
                {
                    Grid.RowDefinitions.Add(new RowDefinition() { Height = 230 });
                    columnIndex--;
                    rowIndex++;
                }
                Frame frame = new Frame()
                {
                    WidthRequest = 172.5,
                    HeightRequest = 230,
                    //BorderColor = Color.FromRgb(0, 127, 255),
                    CornerRadius = 30,
                    HasShadow = false,
                };
                Image image = new Image();
                image.HeightRequest = 160;
                if (DeviceNames.DeviceNamesList.ContainsKey(device))
                {
                    image.Source = device + "1.png";
                }
                else
                {
                    image.Source = "noimg2.png";
                }
                //image.Source = "IPS_1.png";
                Label label = new Label()
                {
                    Text = DeviceNames.DeviceNamesList[device],
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = 14,
                    TextColor = Color.Black
                };
                StackLayout stackLayout = new StackLayout();
                stackLayout.Children.Add(image);
                stackLayout.Children.Add(label);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, ev) => {
                    DevicesListView_ItemTapped(s, ev);
                };
                frame.GestureRecognizers.Add(tapGestureRecognizer);
                frame.Content = stackLayout;
                Grid.Children.Add(frame, columnIndex, rowIndex);
            }

            if (Grid.Children.Count() == 0)
            {
                Grid.RowDefinitions.Clear();
                Grid.ColumnDefinitions.Clear();
                Label label = new Label()
                {
                    Text = "На данный момент нет доступных сохранений для устройств",
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = 16,
                    TextColor = Color.Black
                };
                Grid.Children.Add(label);
            }
        }

        private async void DevicesListView_ItemTapped(object sender, EventArgs e)
        {
            Frame frame = (sender as Frame);
            frame.BackgroundColor = Color.FromRgb(204, 213, 244);
            await Navigation.PushAsync(new HistoryPageIPS_1());
        }
    }
}