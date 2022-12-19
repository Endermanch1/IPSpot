using Microsoft.Win32;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IPSpot
{
    public partial class MainWindow : Window
    {
        //int working0;
        int working1;
        async void MoveUp()
        {
            while (working1 != 60)
            {
                this.Left = this.Left - 7;
                this.Width = this.Width + 14;
                await Task.Delay(7);
                working1++;
            }
            WebBrowserInput.Visibility = Visibility.Visible;
        }
        void Russian()
        {
            IPText.Text = "Введите IP: ";
            SearchBtn.Content = "Поиск";
            CityText.Text = "Город: ";
            RegionText.Text = "Регион: ";
            CountryText.Text = "Страна: ";
            CurrencyText.Text = "Валюта: ";
            InternetText.Text = "Провайдер: ";
            RussianRadioBtn.IsChecked = true;
        }
        void English()
        {
            IPText.Text = "Enter IP: ";
            SearchBtn.Content = "Search";
            CityText.Text = "City: ";
            RegionText.Text = "Region: ";
            CountryText.Text = "Country: ";
            CurrencyText.Text = "Currency: ";
            InternetText.Text = "Internet Provider: ";
            EnglishRadioBtn.IsChecked = true;
        }
        public MainWindow()
        {
            InitializeComponent();

            if (Registry.CurrentUser.OpenSubKey(@"Software\IPSpot") == null)
            {
                Registry.CurrentUser.CreateSubKey(@"Software\IPSpot").SetValue("Language", "Russian");
            }

            if (Registry.CurrentUser.OpenSubKey(@"Software\IPSpot").GetValue("Language").ToString() == "Russian")
                Russian();
            else
                English();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Opacity = 0;
            //working0 = 0;

            //while (working0 != 10)
            //{
            //    this.Opacity += 0.1;
            //    await Task.Delay(7);
            //    working0++;
            //}

            this.MouseDown += (MouseButtonEventHandler)((sender2, e2) => { this.DragMove(); });
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            //working0 = 0;
            //while (working0 != 10)
            //{
            //    this.Opacity -= 0.1;
            //    await Task.Delay(7);
            //    working0++;
            //}
            this.Close();
        }

        private void RussianRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            Russian();
            Registry.CurrentUser.CreateSubKey(@"Software\IPSpot").SetValue("Language", "Russian");

        }

        private void EnglishRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            English();
            Registry.CurrentUser.CreateSubKey(@"Software\IPSpot").SetValue("Language", "English");
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == "")
            {
                if (RussianRadioBtn.IsChecked == true)
                    MessageBox.Show("IP введён не верно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("IP is entered incorrectly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                SearchBtn.IsEnabled = false;
                TextBox.IsEnabled = false;

                WebClient webip = new WebClient();
                string line = webip.DownloadString($"http://free.ipwhois.io/xml/{TextBox.Text}");

                if (!line.Contains("invalid"))
                {
                    Match match = Regex.Match(line, "<country>(.*?)</country>(.*?)<region>(.*?)</region>(.*?)<city>(.*?)</city>(.*?)<org>(.*?)</org>(.*?)<currency>(.*?)</currency>");
                    Match match2 = Regex.Match(line, "<latitude>(.*?)</latitude>(.*?)<longitude>(.*?)</longitude>");
                    CountryTextInput.Text = match.Groups[1].ToString();
                    RegionTextInput.Text = match.Groups[3].ToString();
                    CityTextInput.Text = match.Groups[5].ToString();
                    InternetTextInput.Text = match.Groups[7].ToString();
                    CurrencyTextInput.Text = match.Groups[9].ToString();
                    string maps1 = match2.Groups[1].ToString();
                    string maps2 = match2.Groups[3].ToString();
                    MoveUp();
                    WebBrowserInput.Source = new Uri($"https://www.google.com/maps/search/?api=1&query={maps1},{maps2}");
                }
                else
                {
                    if (RussianRadioBtn.IsChecked == true)
                        MessageBox.Show("IP введён не верно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("IP is entered incorrectly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                SearchBtn.IsEnabled = true;
                TextBox.IsEnabled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(TextBox.Text, "[^0-9-.]"))
                TextBox.Text = TextBox.Text.Remove(TextBox.Text.Length - 1);
            TextBox.SelectionStart = TextBox.Text.Length;
        }
    }
}
