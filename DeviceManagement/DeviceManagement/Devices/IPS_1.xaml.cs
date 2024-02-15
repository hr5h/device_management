using PCLStorage;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Xamarin.Forms.Xaml;
using FileSystem = PCLStorage.FileSystem;

namespace DeviceManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IPS_1 : CarouselPage
    {

        private readonly IDevice connectedDevice;
        private ICharacteristic sendCharacteristic;
        private ICharacteristic receiveCharacteristic;
        private int intBattery = 2;
        public IPS_1(IDevice _connectedDevice)
        {
            InitializeComponent();
            connectedDevice = _connectedDevice;
            Title = "Подключено к ИПС-1";
            //Title = "Подключено " + connectedDevice.Name;
            GattConnect();
            CarouselView.ItemsSource = new List<string>() { "IPS_14.png", "IPS_12.png", "IPS_13.png" };
            RepeatFunction();
        }

        private async void RepeatFunction()
        {
            while (true)
            {
                int currentPosition = CarouselView.Position;
                currentPosition++;
                if (currentPosition == 3) { currentPosition = 0; }
                CarouselView.ScrollTo(currentPosition);
                await Task.Delay(4000);
            }
        }

        private async void GattConnect()
        {
            try
            {
                var service = await connectedDevice.GetServiceAsync(GattIdentifiers.UartGattServiceId);
                if (service != null)
                {
                    sendCharacteristic = await service.GetCharacteristicAsync(GattIdentifiers.UartGattCharacteristicSendId);
                    receiveCharacteristic = await service.GetCharacteristicAsync(GattIdentifiers.UartGattCharacteristicReceiveId);
                    if (receiveCharacteristic != null)
                    {
                        receiveCharacteristic.ValueUpdated += (o, args) =>
                        {
                            var receivedBytes = args.Characteristic.Value;
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                string str = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                                //Output.Text += str;
                                if (str.Length > 0) {
                                    var strN = str.Split('\n', '\r');
                                    foreach (var s in strN)
                                    {
                                        if (s.Length > 0)
                                        {
                                            string firstChar = s.Substring(0, 1);
                                            string leftStr = s.Substring(1);
                                            //leftStr = leftStr.Replace("\r", string.Empty);
                                            switch (firstChar)
                                            {
                                                case "t":
                                                    temperatureLabel.Text = leftStr + " C°";
                                                    break;
                                                case "h":
                                                    humidityLabel.Text = leftStr + " %";
                                                    break;
                                                case "b":
                                                    int intBattery = int.Parse(leftStr)/33;
                                                    var sourceImage = ((BatteryImages.BatteryCharge)intBattery).ToString() + ".png";
                                                    batteryImage.Source = sourceImage;
                                                    break;
                                                case "v":
                                                    lightningLabel.Text = leftStr + "V";
                                                    break;
                                                case "s":
                                                    StartTest();
                                                    break;
                                                case "r":
                                                    if (leftStr[0] == 'R')
                                                    {
                                                        resistorLabel.TextColor = Color.FromRgb(227, 38, 54);
                                                    }
                                                    leftStr = leftStr.Replace("*", "\u00d7");
                                                    int indDegree = leftStr.IndexOf("^");
                                                    string newStr = leftStr.Substring(indDegree + 1);
                                                    leftStr = leftStr.Substring(0, indDegree);
                                                    foreach (var item in newStr)
                                                    {
                                                        leftStr += ConvertToUnicode(item.ToString());
                                                    }
                                                    resistorLabel.Text = leftStr + " Ом";
                                                    StartTestButton.IsEnabled = true;
                                                    HistoryButton.IsEnabled = true;
                                                    IsTestIndicator.IsRunning = false;
                                                    SaveResult.IsVisible = true;
                                                    //ProgressTest.Progress = 0;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                            });
                        };
                        await receiveCharacteristic.StartUpdatesAsync();
                    }
                }
            }
            catch
            {
            }
        }

        private string ConvertToUnicode(string c)
        {
            switch (c) {
                case "0":
                    return "\u2070";
                case "1":
                    return "\u00b9";
                case "2":
                    return "\u00b2";
                case "3":
                    return "\u00b3";
                case "4":
                    return "\u2074";
                case "5":
                    return "\u2075";
                case "6":
                    return "\u2076";
                case "7":
                    return "\u2077";
                case "8":
                    return "\u2078";
                case "9":
                    return "\u2079";
                default : 
                    break;
            }
            return "";
        }

        private void ChangeButton_Clicked(object sender, EventArgs e)
        {
            var sourceImage = ((BatteryImages.BatteryCharge)intBattery).ToString() + ".png";
            batteryImage.Source = sourceImage;
            intBattery--;
            if (intBattery < 0) intBattery = 4;
            /*
            try
            {
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.ASCII.GetBytes("change"));
                }
            }
            catch
            {

            }*/
        }

        private void StartTest()
        {
            resistorLabel.TextColor = Color.Black;
            resistorLabel.Text = "0 Ом";
            StartTestButton.IsEnabled = false;
            HistoryButton.IsEnabled = false;
            IsTestIndicator.IsRunning = true;
            SaveResult.IsVisible = false;
        }

        private async void StartTestButton_Clicked(object sender, EventArgs e)
        {
            StartTest();

            //StopTestButton.IsEnabled = true;
            //ProgressTest.ProgressTo(1, 14000, Easing.Linear);
            try
            {
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.ASCII.GetBytes("1"));
                }
            }
            catch
            {

            }
        }
        private async void StopTestButton_Clicked(object sender, EventArgs e)
        {
            //StopTestButton.IsEnabled = false;
            try
            {
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.ASCII.GetBytes("0"));
                }
            }
            catch
            {

            }
        }
        private void HideButton_Clicked(object sender, EventArgs e)
        {
            //OutputFrame.IsVisible = !OutputFrame.IsVisible;
        }

        private void ThemeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            /*
            if (ThemeSwitch.IsToggled)
            {
                BackgroundColor = Color.Black;
                HeaderLabel.TextColor = Color.White;
                DescriptionLabel.TextColor = Color.White;
                IndicatorView.SelectedIndicatorColor = Color.White;
                ThemeSwitch.ThumbColor = Color.White;
                BoxView1.Color = Color.White;
                BoxView2.Color = Color.White;
            }
            else
            {
                //BackgroundColor = Color.FromRgb(245, 245, 245);
                BackgroundColor = Color.White;
                HeaderLabel.TextColor = Color.Black;
                DescriptionLabel.TextColor= Color.Black;
                IndicatorView.SelectedIndicatorColor = Color.Black;
                ThemeSwitch.ThumbColor = Color.Black;
                BoxView1.Color = Color.Black;
                BoxView2.Color = Color.Black;
            }
            */
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var folders = await rootFolder.GetFoldersAsync();
            foreach (var f in folders)
            {
                Console.WriteLine(f.Name);
            }
            IFolder folder = await rootFolder.CreateFolderAsync("SaveFolder", CreationCollisionOption.OpenIfExists);
            var ff = await folder.GetFilesAsync();
            foreach (var f in ff)
            {
                Console.WriteLine(f.Name);
            }
            IFile file = await folder.CreateFileAsync("IPS_1.txt", CreationCollisionOption.OpenIfExists);
            string str = await file.ReadAllTextAsync();
            string name = SaveEntry.Text;
            if (name == "" || name.Trim() == string.Empty) name = "Без имени";
            await file.WriteAllTextAsync(DateTime.Now.ToString("dd.MM.yyyy_HH:mm") + "_" + name + "_" + lightningLabel.Text + "_" + temperatureLabel.Text + "_" + humidityLabel.Text + "_" + resistorLabel.Text + Environment.NewLine + str);
            SaveEntry.Text = "";

            DependencyService.Get<IMessage>().ShortAlert("Результат сохранен");
            SaveResult.IsVisible = false;
        }

        private async void HistoryButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPageIPS_1());
        }

    }
}