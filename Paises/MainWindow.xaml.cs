using Newtonsoft.Json;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using Paises.Modelos;
using Paises.Services;
using Paises.Models;

namespace Paises
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Attributes

        private List<Country> Countries;
        private NetworkService networkService;
        private ApiService apiService;
        private DataService dataService;
        private DialogService dialogService;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        #endregion

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dataService = new DataService();
            LoadCountries();
            LoadAnthem();
            GetUserCountryByIp();
        }

        private async void LoadCountries()
        {
            dataService.CreateDatabase();
            dataService.CreateTranslations();
            dataService.CreateLanguages();

            bool load;

            lblResult.Content = "Loading Countries...";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                LoadLocalCountries();
                load = false;
            }

            else
            {
                await LoadApiCountries();
                FlagsDownload();
                load = true;
            }

            if (Countries.Count == 0)
            {
                lblResult.Content = "There's no internet connection and the data wasn't previously loaded." +
                                   Environment.NewLine + "Try again later.";

                lblStatus.Content = "You need to have internet connection for the first boot.";
                return;
            }

            cbCountries.ItemsSource = Countries;
            cbCountries.DisplayMemberPath = "Name";


            progressBar.Value = 100;

            lblResult.Content = "Countries loaded!";

            CultureInfo ci = new CultureInfo("en-EN");

            if (load)
            {
                lblStatus.Content = string.Format($"Countries loaded on {DateTime.Now.ToString("D", ci)}");
            }
            else
            {
                lblStatus.Content = "Countries loaded from the database.";
            }
            progressBar.Value = 100;
        }

        private async Task LoadApiCountries()
        {
            progressBar.Value = 0;

            var response = await apiService.GetCountries("http://restcountries.eu/rest/v2/", "all");

            Countries = (List<Country>)response.Result;
            dataService.DeleteData();
            dataService.SaveData(Countries);


        }
        private void FlagsDownload()
        {
            WebClient wc = new WebClient();

            foreach (var country in Countries)
            {
                try
                {
                    if (!File.Exists($@"Images_Jpg/{country.Alpha3Code}.jpg"))
                    {
                        wc.DownloadFile(country.Flag, $@"Images/{country.Alpha3Code}.svg");

                        ConvertSvgToJpg(country.Alpha3Code);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
            wc.Dispose();
        }
        private void ConvertSvgToJpg(string Name)
        {
            try
            {
                string flagSvg = $@"Images/{Name}.svg";

                var svg = SvgDocument.Open(flagSvg);

                Bitmap map = svg.Draw(400, 230);

                string flagJpg = $@"Images_Jpg/{Name}.jpg";

                map.Save(flagJpg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
        private void ShowFlags(Country flag)
        {
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + $@"Images_Jpg/{flag.Alpha3Code}.jpg", UriKind.Absolute);
            bitmap.EndInit();

            imgFlag.Source = bitmap;
        }

        private void LoadLocalCountries()
        {
            Countries = dataService.GetData();
        }
        private void LoadAnthem()
        {
            mediaPlayer.Open(new Uri($@"Audio/National Anthems/Afghanistan.mp3", UriKind.Relative));
        }


        public void GetUserCountryByIp()
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/");
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception)
            {
                ipInfo.Country = "Location \nnot found";
            }

            lblIP.Content = $"You're located \nin {ipInfo.Country}";

            cbCountries.SelectedIndex = 0;
        }

        

        private void cbCountries_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Country country = Countries[cbCountries.SelectedIndex];

            ShowFlags(country);

            lblCapital.Content = $"{country.Capital}";
            lblRegion.Content = $"{country.Region}";
            lblSubregion.Content = $"{country.Subregion}";
            lblPopulation.Content = $"{country.Population}";
            lblGini.Content = $"{country.Gini}";

            lblDE.Content = $"{country.Translations.De}";
            lblJA.Content = $"{country.Translations.Ja}";
            lblPT.Content = $"{country.Translations.Pt}";

            mediaPlayer.Open(new Uri($@"National Anthems/{country.Name}.mp3", UriKind.Relative));

            lblInfo.Content = $"Playing \n{country.Name}'s \nnational anthem";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);

            if (Storyboard.Contains("Show"))
            {
                btnHide.Visibility = System.Windows.Visibility.Visible;
                btnShow.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (Storyboard.Contains("Hide"))
            {
                btnHide.Visibility = System.Windows.Visibility.Hidden;
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void btnRightMenuShow_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbShowRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);
            mediaPlayer.Play();
        }

        private void btnRightMenuHide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);
            mediaPlayer.Pause();
        }

        private void ShowHideMenu2(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);

            if (Storyboard.Contains("Show"))
            {
                btnHide.Visibility = System.Windows.Visibility.Visible;
                btnShow.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (Storyboard.Contains("Hide"))
            {
                btnHide.Visibility = System.Windows.Visibility.Hidden;
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void btnRightMenuShow2_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu2("sbShowRightMenu2", btnRightMenuHide2, btnRightMenuShow2, pnlRightMenu2);
        }

        private void btnRightMenuHide2_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu2("sbHideRightMenu2", btnRightMenuHide2, btnRightMenuShow2, pnlRightMenu2);
        }
    }
}
