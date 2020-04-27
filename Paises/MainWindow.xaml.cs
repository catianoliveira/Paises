using Paises.Modelos;
using Paises.Services;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

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
        #endregion

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dataService = new DataService();
            LoadCountries();
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
                load = true;
            }

            if (Countries.Count == 0)
            {
                lblResult.Content = "There's no internet connection and the data wasn't loaded previously." +
                                   Environment.NewLine + "Try again later.";

                lblStatus.Content = "You need to have internet connection for the first boot.";
                return;
            }

            cbCountries.ItemsSource = Countries;
            cbCountries.DisplayMemberPath = "Name";


            progressBar.Value = 100;

            lblResult.Content = "Countries loaded!";

            if (load)
            {
                //TODO
                lblStatus.Content = string.Format($"Countries loaded in {DateTime.Today.DayOfWeek}");
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

            foreach (var country1 in Countries)
            {
                WebClient client = new WebClient();
                client.DownloadFile($"{country1.Flag}", $"Images/{country1.Name}.svg");
                client.Dispose();

                //string flagSvg = $@"Images/{country1.Name}.svg";

                //var svg = SvgDocument.Open(flagSvg);

                //Bitmap map = svg.Draw(400, 230);

                //string flagJpg = $@"Images/{country1.Name}.jpg";

                //map.Save(flagJpg);
            }
        }

        private void LoadLocalCountries()
        {
            Countries = dataService.GetData();
        }
        private void CbCountries_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Country country = Countries[cbCountries.SelectedIndex];

            lblCapital.Content = $"Capital: {country.Capital}";
            lblRegion.Content = $"Region: {country.Region}";
            lblSubregion.Content = $"Subregion: {country.Subregion}";
            lblPopulation.Content = $"Population: {country.Population}";
            lblGini.Content = $"Gini: {country.Gini}";

            lblDE.Content = $"German: {country.Translations.De}";
            lblES.Content = $"Spanish: {country.Translations.Es}";
            lblFR.Content = $"French: {country.Translations.Fr}";
            lblJA.Content = $"Japanese: {country.Translations.Ja}";
            lblIT.Content = $"Italian: {country.Translations.It}";
            lblBR.Content = $"Brazilian: {country.Translations.Br}";
            lblPT.Content = $"Portuguese: {country.Translations.Pt}";
            lblNL.Content = $"Dutch: {country.Translations.Nl}";
            lblHR.Content = $"Croatian: {country.Translations.Hr}";
            lblFA.Content = $"Arabian: {country.Translations.Fa}";

            //BitmapImage bitmap = new BitmapImage();

            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + $@"Images/{country.Name}.svg", UriKind.Absolute);
            //bitmap.EndInit();

            //imgFlag.Source = 

            string flagSvg = $@"Images/{country.Name}.svg";

            var svg = SvgDocument.Open(flagSvg);

            Bitmap map = svg.Draw(400, 230);

            string flagJpg = $@"Images/{country.Name}.jpg";

            map.Save(flagJpg);
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + flagJpg, UriKind.Absolute);
            bitmap.EndInit();

            imgFlag.Source = bitmap;
        }
    }
}
